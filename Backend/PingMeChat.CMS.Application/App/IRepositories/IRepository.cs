using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.IRepositories
{
    public interface IRepository<T> where T : class
    {
        Task<T> Add(T entity);
        Task<IEnumerable<T>> AddRange(IEnumerable<T> entities);
        Task Update(T entity);
        Task<IEnumerable<T>> UpdateRange(IEnumerable<T> entities);
        
        Task<T> Delete(T entity);
        Task<T> Delete(string id);
        Task<T> SoftDelete(T entity);
        Task<T> SoftDelete(string id);
        Task<IEnumerable<T>> GetAll();
        Task<int> Count(Expression<Func<T, bool>> predicate);
        Task<T?> Find(Expression<Func<T, bool>> match);
        Task<T?> Find(Expression<Func<T, bool>> match, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
        Task<T?> Find( Expression<Func<T, bool>>? match = null,
                         Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                         Expression<Func<T, T>>? selector = null,
                          Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                         bool ignoreQueryFilters = false,
                         bool asNoTracking = true,
                         bool asSplitQuery = true);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> match, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> match, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null);
        Task<T> FindById(string id);
        Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> FindAllSelect<TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> selector);
        IQueryable<T> GetAllInclude(params Expression<Func<T, object>>[] includeProperties);
        Task<bool> AnyAsync(Expression<Func<T, bool>> match);
        Task<List<T>> Pagination(out int total, int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, Expression<Func<T, T>>? selector = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool disableTracking = true,
            bool ignoreQueryFilters = false);

        Task<(List<TResult>, int total)> PaginationAsync<TResult>(
                                     int pageNumber,
                                     int pageSize,
                                     Expression<Func<T, bool>>? predicate = null,
                                     Expression<Func<T, TResult>>? selector = null,
                                     Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                                     Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                     bool disableTracking = true,
                                     bool ignoreQueryFilters = false);
        Task<bool> RemoveRange(IEnumerable<T> entities);
    }
}
