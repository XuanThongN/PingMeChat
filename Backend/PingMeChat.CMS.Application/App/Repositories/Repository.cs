using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using NPOI.SS.Formula.Functions;
using PingMeChat.CMS.Application.App.IRepositories;
using PingMeChat.CMS.EntityFrameworkCore.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PingMeChat.CMS.Application.App.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, new()
    {
        private readonly AppDBContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(AppDBContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<T> Add(T entity)
        {
            var result = await _dbSet.AddAsync(entity);

            return entity;
        }

        public async Task<IEnumerable<T>> AddRange(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
            return entities;
        }

        public async Task<IEnumerable<T>> UpdateRange(IEnumerable<T> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException(nameof(entities));
            }

            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            }

            return entities; // Không cần await ở đây vì SaveChanges sẽ được gọi ở UnitOfWork

        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> match)
        {
            return await _dbSet.AnyAsync(match);
        }

        public async Task<int> Count(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.CountAsync(predicate);
        }

        public Task<T> Delete(T entity)
        {
            return Task.Factory.StartNew(() => _dbSet.Remove(entity).Entity);
        }

        public async Task<T> Delete(string id)
        {
            var entity = await _dbSet.FindAsync(id);
            return _dbSet.Remove(entity).Entity;
        }

        public async Task<T> SoftDelete(string id)
        {
            var entity = await _dbSet.FindAsync(id);
            _context.Entry(entity).Property("IsDeleted").CurrentValue = true;
            return entity;
        }

        public async Task<T> SoftDelete(T entity)
        {
            _context.Entry(entity).Property("IsDeleted").CurrentValue = true;
            return entity;
        }

        public async Task<T?> Find(Expression<Func<T, bool>> match)
        {
            T? result = await _dbSet.Where(match).FirstOrDefaultAsync();
            return result;
        }

        public async Task<T?> Find(Expression<Func<T, bool>> match, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            //  T result = await _dbSet.Where(match).Include(include).FirstOrDefaultAsync();
            IQueryable<T> query = _dbSet;
            if (include != null)
            {
                query = include(query);
            }
            if (match != null)
            {
                query = query.Where(match);
            }
            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> match, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.AsNoTracking(); // Không theo dõi đối tượng nào được trả về từ truy vấn
            if (include != null)
            {
                query = include(query);
            }
            if (match != null)
            {
                query = query.Where(match);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> FindAll(Expression<Func<T, bool>> match,
                                         Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                         Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null)
        {
            IQueryable<T> query = _dbSet;
            query = query.AsNoTracking(); // Không theo dõi đối tượng nào được trả về từ truy vấn
            if (include != null)
            {
                query = include(query);
            }
            if (match != null)
            {
                query = query.Where(match);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<TResult>> FindAllSelect<TResult>(
            Expression<Func<T, bool>> predicate,
            Expression<Func<T, TResult>> selector)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            return await query.Select(selector).ToListAsync();
        }


        public async Task<T> FindById(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }

        public IQueryable<T> GetAllInclude(params Expression<Func<T, object>>[] includeProperties)
        {
            IQueryable<T> queryable = _context.Set<T>();
            foreach (Expression<Func<T, object>> includeProperty in includeProperties)
            {

                queryable = queryable.Include(includeProperty);
            }

            return queryable;
        }

        public Task<List<T>> Pagination(out int total, int pageNumber, int pageSize, Expression<Func<T, bool>>? predicate = null, Expression<Func<T, T>>? selector = null, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, bool disableTracking = true, bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = _dbSet;

            if (ignoreQueryFilters)
            {
                query = query.IgnoreQueryFilters(); // ignoreQueryFilters = true bỏ qua bộ lọc truy vấn được định nghĩa trên dbSet
            }

            if (disableTracking)
            {
                query = query.AsNoTracking(); // Không theo dõi đối tượng nào được trả về từ truy vấn
            }

            if (include != null)
            {
                query = include(query);
            }

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            total = query.Count();

            query = query.Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize);

            if (selector != null)
            {
                query = query.Select(selector);
            }

            return query.ToListAsync();
        }

        public Task Update(T entity)
        {
            return Task.Factory.StartNew(() =>
            {
                _dbSet.Attach(entity);
                _context.Entry(entity).State = EntityState.Modified;
            });
        }
        public async Task<T?> Find(
     Expression<Func<T, bool>>? match = null,
     Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
     Expression<Func<T, T>>? selector = null,
      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
     bool ignoreQueryFilters = false,
     bool asNoTracking = true,
     bool asSplitQuery = true)
        {
            IQueryable<T> query = _dbSet;

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (asNoTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (match != null)
                query = query.Where(match);

            if (selector != null)
                query = query.Select(selector);

            if (orderBy != null)
                query = orderBy(query);

            if (asSplitQuery)
                query = query.AsSplitQuery();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<(List<TResult>, int total)> PaginationAsync<TResult>(
      int pageNumber,
      int pageSize,
      Expression<Func<T, bool>>? predicate = null,
      Expression<Func<T, TResult>>? selector = null,
      Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
      Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
      bool disableTracking = true,
      bool ignoreQueryFilters = false)
        {
            IQueryable<T> query = _dbSet;

            if (ignoreQueryFilters)
                query = query.IgnoreQueryFilters();

            if (disableTracking)
                query = query.AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            // Đếm tổng số mục trước khi áp dụng phân trang
            int total = await query.CountAsync();

            if (orderBy != null)
                query = orderBy(query);

            query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

            if (selector != null)
                return (await query.Select(selector).ToListAsync(), total);
            else
                return (await query.Cast<TResult>().ToListAsync(), total);
        }
        public async Task<bool> RemoveRange(IEnumerable<T> entities)
        {
            try
            {
                _dbSet.RemoveRange(entities);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }

}
