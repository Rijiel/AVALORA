using System.Linq.Expressions;

namespace AVALORA.Core.ServiceContracts;

public interface IGenericService<TModel, TAddDto, TUpdateDto, TResponseDto>
    where TModel : class
    where TAddDto : class
    where TUpdateDto : class
    where TResponseDto : class
{
    /// <summary>
    /// This method retrieves all instances of the generic model type.
    /// </summary>
    /// <param name="filter">An optional filter expression to filter the results.</param>
    /// <param name="includes">An optional list of navigation properties to include in the results.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains a list of the generic model 
    /// type.</returns>
    Task<List<TResponseDto>> GetAllAsync(Expression<Func<TModel, bool>>? filter = null, 
        CancellationToken cancellationToken = default, params string[] includes);

    /// <summary>
    /// Retrieves a single instance of the generic model type based on the provided filter.
    /// </summary>
    /// <param name="filter">The filter expression to find the model.</param>
    /// <param name="includes">An optional list of navigation properties to include in the result.</param>
    /// <param name="tracked">Whether to track the model in the context. Defaults to false.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the found model or null if not 
    /// found.</returns>
    Task<TResponseDto?> GetAsync(Expression<Func<TModel, bool>> filter, bool tracked = false, 
        CancellationToken cancellationToken = default, params string[] includes);

    /// <summary>
    /// Retrieves a single instance of the generic model type based on the provided ID.
    /// </summary>
    /// <param name="id">The ID of the model to retrieve.</param>
    /// <param name="includes">An optional list of navigation properties to include in the result.</param>
    /// <param name="tracked">Whether to track the model in the context. Defaults to false.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the found model or null if 
    /// not found.</returns>
    Task<TResponseDto?> GetByIdAsync(object? id, bool tracked = false, CancellationToken cancellationToken = default, 
        params string[] includes);

    /// <summary>
    /// Asynchronously adds a new instance of the generic model type to the repository.
    /// </summary>
    /// <param name="addDto">The data transfer object containing the data for the new instance.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the added instance of the 
    /// generic model type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="addDto"/> parameter is null.</exception>
    Task<TResponseDto> AddAsync(TAddDto? addDto);

    /// <summary>
    /// Asynchronously adds a list of new instances of the generic model type to the repository.
    /// </summary>
    /// <param name="addDtos">The list of data transfer objects containing the data for the new instances.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains a list of the added instances 
    /// of the generic model type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="addDtos"/> parameter is null.</exception>
    Task<List<TResponseDto>> AddRangeAsync(List<TAddDto> addDtos);

    /// <summary>
    /// Asynchronously updates an existing instance of the generic model type in the repository.
    /// </summary>
    /// <param name="updateDto">The data transfer object containing the data for the updated instance.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the updated instance of the 
    /// generic model type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="updateDto"/> parameter is null.</exception>
    Task<TResponseDto> UpdateAsync(TUpdateDto? updateDto);

    /// <summary>
    /// Asynchronously updates a partial instance of the generic model type in the repository.
    /// </summary>
    /// <param name="updateDto">The data transfer object containing the data for the updated instance.</param>
    /// <param name="propertyNames">The names of the properties to update. All other properties will remain unchanged.</param>
    /// <returns>A Task that represents the asynchronous operation. The task result contains the updated instance of the 
    /// generic model type.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="updateDto"/> parameter is null.</exception>
    /// <exception cref="ArgumentException">Thrown when the <paramref name="propertyNames"/> parameter is empty.</exception>
    Task<TResponseDto> UpdatePartialAsync(TUpdateDto? updateDto, params string[] propertyNames);

    /// <summary>
    /// Asynchronously removes a model instance from the repository based on the provided ID.
    /// </summary>
    /// <param name="id">The ID of the model to remove.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="id"/> parameter is null.</exception>
    Task RemoveAsync(object? id);


    /// <summary>
    /// Asynchronously removes a range of model instances from the repository based on the provided response DTOs.
    /// </summary>
    /// <param name="responseDtos">The response DTOs containing the IDs of the models to remove.</param>
    /// <returns>A Task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="responseDtos"/> parameter is null.</exception>
    Task RemoveRangeAsync(IEnumerable<TResponseDto>? responseDtos);
}

