using AVALORA.Core.Dto.OrderHeaderDtos;

namespace AVALORA.Core.ServiceContracts.FacadeServiceContracts;

public interface IOrderFacade
{
	/// <summary>
	/// Retrieves the current user's order header add request asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token that can be used by 
	/// other objects or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation,
	/// containing the current user's order header add request.</returns>
	Task<OrderHeaderAddRequest> GetCurrentUserOrderHeaderAddRequest(CancellationToken cancellationToken = default);
}

