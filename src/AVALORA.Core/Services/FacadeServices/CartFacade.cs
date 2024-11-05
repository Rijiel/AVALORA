﻿using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AVALORA.Core.Services.FacadeServices;

public class CartFacade : BaseFacade<CartFacade>, ICartFacade
{
	private readonly IHttpContextAccessor _contextAccessor;

	public CartFacade(IHttpContextAccessor contextAccessor, IServiceProvider serviceProvider) : base(serviceProvider)
	{
		_contextAccessor = contextAccessor;
	}

	public async Task AddToCartAsync(CartItemAddRequest? cartItemAddRequest, Controller controller)
	{
		if (cartItemAddRequest == null)
		{
			Logger.LogWarning("Cart item not found");
			throw new ArgumentNullException(nameof(cartItemAddRequest));
		}

		// Add current user id to cart item
		string userId = UserHelper.GetCurrentUserId(_contextAccessor)!;
		cartItemAddRequest.ApplicationUserId = userId;

		// Retrieve associated product with the cart item to use for checking existing cart item
		ProductResponse? productResponse = await ServiceUnitOfWork.ProductService.GetByIdAsync(cartItemAddRequest.ProductId);
		if (productResponse == null)
		{
			Logger.LogWarning($"Product not found: {cartItemAddRequest.ProductId}");
			throw new KeyNotFoundException("Product not found");
		}

		// Only add count if item is already in cart and of the same color
		CartItemResponse? existingCartItemResponse = await ServiceUnitOfWork.CartItemService
			.GetAsync(c => c.ApplicationUserId == userId
			&& c.ProductId == productResponse.Id
			&& c.Color == cartItemAddRequest.Color);
		if (existingCartItemResponse != null)
		{
			await UpdateCartItemQuantityAsync(existingCartItemResponse.Id, cartItemAddRequest.Count, controller, true);
			return;
		}

		await ServiceUnitOfWork.CartItemService.AddAsync(cartItemAddRequest);
		Logger.LogInformation($"Added product: {cartItemAddRequest.ProductId} to cart");
		controller.TempData[SD.TEMPDATA_SUCCESS] = "Item added to cart";

		UpdateCartSessionCount(controller, +1);
	}

	public async Task UpdateCartItemQuantityAsync(int? cartItemId, int quantity, Controller controller, bool showMessage)
	{
		CartItemResponse? cartItemResponse = await ServiceUnitOfWork.CartItemService.GetByIdAsync(cartItemId);
		if (cartItemResponse == null)
		{
			Logger.LogWarning($"Cart item not found: {cartItemId}");
			throw new KeyNotFoundException("Cart item not found");
		}

		CartItemUpdateRequest cartItemUpdateRequest = Mapper.Map<CartItemUpdateRequest>(cartItemResponse);
		cartItemUpdateRequest.Count += quantity;

		// Remove cart item if count is 0
		if (cartItemUpdateRequest.Count <= 0)
		{
			await ServiceUnitOfWork.CartItemService.RemoveAsync(cartItemResponse.Id);
			Logger.LogInformation($"Removed cart item: {cartItemResponse.Id}");
			controller.TempData[SD.TEMPDATA_SUCCESS] = "Item removed from cart";

			UpdateCartSessionCount(controller, -1);

			return;
		}

		Logger.LogInformation($"Updated cart item {cartItemResponse.Id} quantity: {quantity}");

		if (showMessage)
			controller.TempData[SD.TEMPDATA_SUCCESS] = "Item quantity updated";

		await ServiceUnitOfWork.CartItemService.UpdateAsync(cartItemUpdateRequest);
	}

	public async Task<List<CartItemResponse>> GetCurrentUserCartItemsAsync(bool includeImages = false, CancellationToken cancellationToken = default)
	{
		string userId = UserHelper.GetCurrentUserId(_contextAccessor)!;
		IEnumerable<CartItemResponse> cartItemResponses =
			await ServiceUnitOfWork.CartItemService.GetAllAsync(c => c.ApplicationUserId == userId, cancellationToken: cancellationToken);

		// Retrieve product images if requested
		if (includeImages)
		{
			IEnumerable<ProductImageResponse> productImageResponses = await ServiceUnitOfWork.ProductImageService
				.GetAllAsync(cancellationToken: cancellationToken);
			var productImages = Mapper.Map<IEnumerable<ProductImage>>(productImageResponses);

			foreach (var cartItemResponse in cartItemResponses)
				cartItemResponse.Product.ProductImages = productImages
					.Where(p => p.ProductId == cartItemResponse.ProductId).ToList();
		}

		Logger.LogInformation($"Retrieved cart items for user: {userId}");
		return cartItemResponses.ToList();
	}

	public async Task ClearCartItemsAsync(Controller controller, CancellationToken cancellationToken = default)
	{
		IEnumerable<CartItemResponse> cartItemResponses = await GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken);

		// Remove auto included products to prevent database error
		foreach (var cartItemResponse in cartItemResponses)
			cartItemResponse.Product = null!;

		await ServiceUnitOfWork.CartItemService.RemoveRangeAsync(cartItemResponses);

		Logger.LogInformation($"Cleared cart for user: {UserHelper.GetCurrentUserId(_contextAccessor)}");

		UpdateCartSessionCount(controller, 0);
	}

	public void UpdateCartSessionCount(Controller controller, int quantity)
	{
		if (controller.HttpContext.Session.GetInt32(SD.SESSION_CART).HasValue)
		{
			int currentCount = controller.HttpContext.Session.GetInt32(SD.SESSION_CART)!.Value;
			controller.HttpContext.Session.SetInt32(SD.SESSION_CART, currentCount + quantity);
		}
	}
}

