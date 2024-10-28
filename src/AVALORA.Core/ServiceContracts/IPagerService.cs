using AVALORA.Core.Services;

namespace AVALORA.Core.ServiceContracts;

/// <summary>
/// Provides pagination functionality for a list of entities.
/// </summary>
public interface IPagerService
{
	public int TotalItems { get; }
	public int CurrentPage { get; }
	public int PageSize { get; }
	public int TotalPages { get; }
	public int StartPage { get; }
	public int EndPage { get; }

	/// <summary>
	/// Sets the values for the pager service.
	/// </summary>
	/// <param name="totalItems">The total number of items.</param>
	/// <param name="page">The current page number.</param>
	/// <param name="pageSize">The number of items per page (default is 10).</param>
	void SetValues(int totalItems, int page, int pageSize = 10);

	/// <summary>
	/// Retrieves a paged list of items from the provided list.
	/// </summary>
	/// <typeparam name="TModel">The type of the model.</typeparam>
	/// <param name="items">The list of items to page.</param>
	/// <param name="page">The current page number.</param>
	/// <param name="pageSize">The number of items per page (default is 10).</param>
	/// <returns>A list of paged items.</returns>
	List<TModel> GetPagedItems<TModel>(List<TModel> items, int page, int pageSize = 10)
		where TModel : class;
}

