using AutoMapper;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.Application.Common.Helpers;
using PingMeChat.CMS.Application.Common.Pagination;
using PingMeChat.CMS.Application.Domain.Dto;
using PingMeChat.CMS.Application.Lib;
using PingMeChat.CMS.Application.Service.IRepositories;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace PingMeChat.CMS.Application.Feature.Service
{
    public interface IServiceBase<TEntity, TCreateDto, TUpdateDto, TReadDto, TRepository>
        where TEntity : class
        where TCreateDto : class
        where TUpdateDto : class
        where TReadDto : class
        where TRepository : IRepository<TEntity>
    {
        Task<TReadDto> Add(TCreateDto dto);
        Task<TReadDto> Update(TUpdateDto dto);
        Task<TReadDto> Delete(string id);
        Task<IEnumerable<TReadDto>> GetAll();
        Task<int> Count(Expression<Func<TEntity, bool>> predicate);
        Task<TReadDto> Find(Expression<Func<TEntity, bool>> match);
        Task<TReadDto> Find(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null);
        Task<TReadDto> FindById(string id);
        Task<IEnumerable<TReadDto>> FindAll(Expression<Func<TEntity, bool>> predicate);
        Task<IEnumerable<TReadDto>> GetAllInclude(params Expression<Func<TEntity, object>>[] includeProperties);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> match);
        Task<PagedResponse<List<TReadDto>>> Pagination(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool disableTracking = true, bool ignoreQueryFilters = false, string route = null);
    }
    public abstract class ServiceBase<TEntity, TCreateDto, TUpdateDto, TReadDto, TRepository> : IServiceBase<TEntity, TCreateDto, TUpdateDto, TReadDto, TRepository>
     where TEntity : class
     where TCreateDto : class
     where TUpdateDto : class
     where TReadDto : class
     where TRepository : IRepository<TEntity>
    {
        protected readonly TRepository _repository;
        protected readonly IUnitOfWork _unitOfWork;
        protected readonly IMapper _mapper;
        protected readonly IUriService _uriService;

        public ServiceBase(TRepository repository, IUnitOfWork unitOfWork, IMapper mapper, IUriService uriService)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _uriService = uriService;
        }

        public virtual async Task<TReadDto> Add(TCreateDto dto)
        {
            try
            {
                TEntity entityToAdd = _mapper.Map<TEntity>(dto);
                await _repository.Add(entityToAdd);
                await _unitOfWork.SaveChangeAsync();
                return _mapper.Map<TReadDto>(entityToAdd);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while adding entity.", ex);
            }
        }

        public virtual async Task<TReadDto> Update(TUpdateDto dto)
        {
            try
            {
                // Assume TUpdateDto has an Id property
                var id = (string)dto.GetType().GetProperty("Id").GetValue(dto, null);

                // Get the existing entity
                var existingEntity = await _repository.FindById(id);
                if (existingEntity == null)
                {
                    throw new Exception($"Entity with id {id} not found");
                }

                // Update the existing entity with dto values, ignoring null values
                _mapper.Map(dto, existingEntity);
                existingEntity.GetType().GetProperty("UpdatedDate").SetValue(existingEntity, DateTime.Now);

                await _repository.Update(existingEntity);
                await _unitOfWork.SaveChangeAsync();
                return _mapper.Map<TReadDto>(existingEntity);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while updating entity.", ex);
            }
        }


        public virtual async Task<TReadDto> Delete(string id)
        { 
            try
            {
                TEntity entityToDelete = await _repository.FindById(id);
                if (entityToDelete == null)
                {
                    return null;
                }
                entityToDelete.GetType().GetProperty("IsDeleted").SetValue(entityToDelete, true);
                await _repository.Update(entityToDelete);
                await _unitOfWork.SaveChangeAsync();
                return _mapper.Map<TReadDto>(entityToDelete);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while deleting entity.", ex);
            }
        }

        public virtual async Task<IEnumerable<TReadDto>> GetAll()
        {
            try
            {
                IEnumerable<TEntity> entities = await _repository.GetAll();
                return _mapper.Map<IEnumerable<TReadDto>>(entities);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while retrieving entities.", ex);
            }
        }

        public virtual async Task<int> Count(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                return await _repository.Count(predicate);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while counting entities.", ex);
            }
        }

        public virtual async Task<TReadDto> Find(Expression<Func<TEntity, bool>> match)
        {
            try
            {
                TEntity entity = await _repository.Find(match);
                return _mapper.Map<TReadDto>(entity);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while finding entity.", ex);
            }
        }

        public async Task<TReadDto> Find(Expression<Func<TEntity, bool>> match, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null)
        {
            var data = await _repository.Find(match: match, include: include);
            if (data == null) throw new ApplicationException("Table not found");
            var result = _mapper.Map<TReadDto>(data);
            return result;

        }

        public virtual async Task<TReadDto> FindById(string id)
        {
            try
            {
                TEntity entity = await _repository.FindById(id);
                return _mapper.Map<TReadDto>(entity);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while finding entity by ID.", ex);
            }
        }

        public virtual async Task<IEnumerable<TReadDto>> FindAll(Expression<Func<TEntity, bool>> predicate)
        {
            try
            {
                IEnumerable<TEntity> entities = await _repository.FindAll(predicate);
                return _mapper.Map<IEnumerable<TReadDto>>(entities);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while finding entities.", ex);
            }
        }

        public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> match)
        {
            try
            {
                return await _repository.AnyAsync(match);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while checking existence of entity.", ex);
            }
        }

        public virtual async Task<PagedResponse<List<TReadDto>>> Pagination(int pageNumber, int pageSize, Expression<Func<TEntity, bool>>? predicate = null, Expression<Func<TEntity, TEntity>>? selector = null, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>? include = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, bool disableTracking = true, bool ignoreQueryFilters = false, string route = null)
        {
            try
            {
                var entities = await _repository.Pagination(out int total, pageNumber, pageSize, predicate, selector, include, orderBy, disableTracking, ignoreQueryFilters);
                if (entities == null)
                {
                    return null;
                }

                var data = _mapper.Map<IEnumerable<TReadDto>>(entities).ToList();
                return PaginationHelper.CreatePagedReponse(data, new PaginationQuery(pageNumber, pageSize), total, _uriService, route);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while paginating entities.", ex);
            }
        }

        public async virtual Task<IEnumerable<TReadDto>> GetAllInclude(params Expression<Func<TEntity, object>>[] includeProperties)
        {
            try
            {
                var query = _repository.GetAllInclude(includeProperties);
                if (query == null) return null;
                var data = _mapper.Map<IEnumerable<TReadDto>>(query).ToList();
                return await Task.FromResult(data);
            }
            catch (Exception ex)
            {
                // Handle exceptions or rethrow as needed
                throw new Exception("Error occurred while finding entities.", ex);
            }
        }
    }
}
