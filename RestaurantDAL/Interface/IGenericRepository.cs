using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RestaurantDAL.Interface
{
    public interface IGenericRepository<TEntity>
    {
        DbContext Context { get; set; }
        bool SkipPolicy { get; set; }

        Expression<Func<TEntity, bool>> ApplyAuthPolicyFilter();

        Task<TEntity> GetNonQueryWithIncludeAsync(Expression<Func<TEntity, bool>> predicate, params string[] include);
        void Insert(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entityToDelete);
        void Update(TEntity entityToUpdate);
        Task<IEnumerable<TEntity>> GetNonQueryAllWithIncludeAsync(params string[] include);
        IEnumerable<TEntity> GetMany(Func<TEntity, bool> where);
        Task<IEnumerable<TEntity>> GetAllAsync();
        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include);
        bool Exists(object primaryKey);
        Task<TEntity> GetFirstAsync(System.Linq.Expressions.Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
        void RemoveRange(IQueryable<TEntity> entityToDeleteList);
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
        Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetFirstOrDefaultAsyncAsNoTracking(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity> GetFirstOrDefaultAsyncAsNoTrackingWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include);
        Task<List<TEntity>> GetAllAsyncAsNoTracking(Expression<Func<TEntity, bool>> predicate);
        void UpdateRange(List<TEntity> entities);
        void AddRange(List<TEntity> entities);
        void RemoveRange(List<TEntity> entities);
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate);

        List<TEntity> GetQueryWithIncludes(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params string[] include);

        IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> filter,
                                                 //int skip = 0,
                                                 //int take = int.MaxValue,
                                                 Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                 params string[] include);
    }
}
