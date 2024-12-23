﻿using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;

namespace AVALORA.Infrastructure.Repositories;

/// <summary>
/// Represents a unit of work for the repository layer, encapsulating access to various repositories.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public ICategoryRepository Categories { get; private set; }
    public IProductRepository Products { get; private set; }
    public IProductImageRepository ProductImages { get; private set; }
    public IApplicationUserRepository ApplicationUsers { get; private set; }
    public ICartItemRepository CartItems { get; private set; }
    public IOrderHeaderRepository OrderHeaders { get; private set; }
    public IOrderSummaryRepository OrderSummaries { get; private set; }
    public IOrderSummaryItemRepository OrderSummaryItems { get; private set; }
    public IProductReviewRepository ProductReviews { get; private set; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        Categories = new CategoryRepository(_dbContext);
        Products = new ProductRepository(_dbContext);
        ProductImages = new ProductImageRepository(_dbContext);
        ApplicationUsers = new ApplicationUserRepository(_dbContext);
        CartItems = new CartItemRepository(_dbContext);
        OrderHeaders = new OrderHeaderRepository(_dbContext);
		OrderSummaries = new OrderSummaryRepository(_dbContext);
		OrderSummaryItems = new OrderSummaryItemRepository(_dbContext);
		ProductReviews = new ProductReviewRepository(_dbContext);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}

