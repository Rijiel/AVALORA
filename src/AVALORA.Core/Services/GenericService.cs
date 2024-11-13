using AutoMapper;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.ServiceContracts;
using SerilogTimings;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;

namespace AVALORA.Core.Services;

public class GenericService<TModel, TAddDto, TUpdateDto, TResponseDto> 
    : IGenericService<TModel, TAddDto, TUpdateDto, TResponseDto>
	where TModel : class
	where TAddDto : class
	where TUpdateDto : class
	where TResponseDto : class
{
	private readonly IGenericRepository<TModel> _repository;
	private readonly IMapper _mapper;
	private readonly IUnitOfWork _unitOfWork;

	public GenericService(IGenericRepository<TModel> repository, IMapper mapper, IUnitOfWork unitOfWork)
	{
		_repository = repository;
		_mapper = mapper;
		_unitOfWork = unitOfWork;
	}

	protected IMapper Mapper => _mapper;

	/// <summary>
	/// Represents a unit of work for the repository layer, encapsulating access to various repositories.
	/// </summary>
	protected IUnitOfWork UnitOfWork => _unitOfWork;

	public async Task<List<TResponseDto>> GetAllAsync(Expression<Func<TModel, bool>>? filter = null, 
        CancellationToken cancellationToken = default, params string[] includes)
	{
        IEnumerable<TModel> models = [];

        using (Operation.Time("Get all {model}", typeof(TModel).Name))
        {
            models = await _repository.GetAllAsync(filter, includes);
            cancellationToken.ThrowIfCancellationRequested();
        }		

		return Mapper.Map<List<TResponseDto>>(models);
	}

	public async Task<TResponseDto?> GetAsync(Expression<Func<TModel, bool>> filter, bool tracked = false, 
        CancellationToken cancellationToken = default, params string[] includes)
	{
		TModel? model = await _repository.GetAsync(filter, tracked, includes);
		cancellationToken.ThrowIfCancellationRequested();

		return Mapper.Map<TResponseDto>(model);
	}

	public async Task<TResponseDto?> GetByIdAsync(object? id, bool tracked = false, 
        CancellationToken cancellationToken = default, params string[] includes)
	{
		if (id == null)
			return null;

		TModel? model = await _repository.GetByIdAsync(id, tracked, includes);
		cancellationToken.ThrowIfCancellationRequested();

		return Mapper.Map<TResponseDto?>(model);
	}

	public async Task<TResponseDto> AddAsync(TAddDto? addDto)
	{
		ArgumentNullException.ThrowIfNull(addDto);

		TModel model = Mapper.Map<TModel>(addDto);

		_repository.Add(model);
		await UnitOfWork.SaveAsync();

		return Mapper.Map<TResponseDto>(model);
	}

	public async Task<List<TResponseDto>> AddRangeAsync(List<TAddDto> addDtos)
	{
		List<TModel> models = Mapper.Map<List<TModel>>(addDtos);

		_repository.AddRange(models);
		await _unitOfWork.SaveAsync();

		return Mapper.Map<List<TResponseDto>>(models);
	}

	public async Task<TResponseDto> UpdateAsync(TUpdateDto? updateDto)
	{
		ArgumentNullException.ThrowIfNull(updateDto);

		PropertyInfo? propertyInfo = updateDto.GetType().GetProperty("Id")
			?? throw new KeyNotFoundException($"{nameof(TModel)} does not have an Id property");
		object id = propertyInfo.GetValue(updateDto)!;

		TModel? modelFromDb = await _repository.GetByIdAsync(id, tracked: true)
			?? throw new KeyNotFoundException($"{nameof(TModel)} with id {id} not found");

		Mapper.Map(updateDto, modelFromDb);

		await _unitOfWork.SaveAsync();

		return Mapper.Map<TResponseDto>(modelFromDb);
	}

	public async Task<TResponseDto> UpdatePartialAsync(TUpdateDto? updateDto, params string[] propertyNames)
	{
		ArgumentNullException.ThrowIfNull(updateDto);

		if (propertyNames.Length == 0)
			throw new ArgumentException("At least one property name must be provided");

		PropertyInfo? propertyInfo = updateDto.GetType().GetProperty("Id")
			?? throw new ArgumentException($"{nameof(TModel)} does not have an Id property");
		object id = propertyInfo.GetValue(updateDto)!;

		TModel? modelFromDb = await _repository.GetByIdAsync(id, tracked: true)
			?? throw new KeyNotFoundException($"{nameof(TModel)} with id {id} not found");

		foreach (string propertyName in propertyNames)
		{
			PropertyInfo? modelFromDbProperty = modelFromDb.GetType().GetProperty(propertyName);
			if (modelFromDbProperty != null)
			{
				PropertyInfo? updateDtoProperty = updateDto.GetType().GetProperty(propertyName);
				if (updateDtoProperty != null)
					modelFromDbProperty.SetValue(modelFromDb, updateDtoProperty.GetValue(updateDto));
			}
		}

		await _unitOfWork.SaveAsync();

		return Mapper.Map<TResponseDto>(modelFromDb);
	}

	public async Task RemoveAsync(object? id)
	{
		ArgumentNullException.ThrowIfNull(id);

		TModel? modelFromDb = await _repository.GetByIdAsync(id)
			?? throw new KeyNotFoundException($"{nameof(TModel)} with id {id} not found");

		_repository.Remove(modelFromDb);
		await _unitOfWork.SaveAsync();
	}

	public async Task RemoveRangeAsync(IEnumerable<TResponseDto>? responseDtos)
	{
		ArgumentNullException.ThrowIfNull(responseDtos);

		IEnumerable<TModel> models = Mapper.Map<IEnumerable<TModel>>(responseDtos);

		_repository.RemoveRange(models);
		await _unitOfWork.SaveAsync();
	}
}

