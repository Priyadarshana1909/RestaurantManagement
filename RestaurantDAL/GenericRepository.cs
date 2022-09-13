using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RestaurantDAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace RestaurantDAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private DbContext _context;
        internal DbSet<TEntity> DbSet;
       

        public bool SkipPolicy { get; set; }
        public DbContext Context { get { return _context; } set { _context = value; DbSet = value.Set<TEntity>(); } }

        public GenericRepository()
        {
      
        }

        public virtual List<TEntity> GetQueryWithIncludes(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params string[] include)
        {
            try
            {
                IQueryable<TEntity> query = DbSet;

                if (include != null && include.Any())
                    query = include.Aggregate(query, (current, inc) => current.Include(inc));

                if (filter != null)
                    query = query.Where(filter);

                if (orderBy != null)
                    query = orderBy(query);

                return query.ToList();
            }
            catch (Exception ex)
            {

                return null;
            }

        }

        public virtual IQueryable<TEntity> Filter(Expression<Func<TEntity, bool>> filter,
                                                //int skip = 0,
                                                //int take = int.MaxValue,
                                                Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                                                params string[] include)
        {
            var _resetSet = filter != null ? DbSet.AsNoTracking().Where(filter).AsQueryable() : DbSet.AsNoTracking().AsQueryable();

            if (include != null && include.Any())
                _resetSet = include.Aggregate(_resetSet, (current, inc) => current.Include(inc));

            if (orderBy != null)
            {
                _resetSet = orderBy(_resetSet).AsQueryable();
            }
            //_resetSet = skip == 0 ? _resetSet.Take(take) : _resetSet.Skip(skip).Take(take);

            return _resetSet.AsQueryable();
        }

        public virtual IEnumerable<TEntity> GetMany(Func<TEntity, bool> where)
        {
            return DbSet.Where(ApplyAuthPolicyFilter()).Where(where).ToList();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAllAsync()
        {
            return await DbSet.Where(ApplyAuthPolicyFilter()).ToListAsync();
        }

        public virtual IQueryable<TEntity> GetAll()
        {
            return DbSet.Where(ApplyAuthPolicyFilter()).AsQueryable();
        }

        public virtual IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate)
        {
            return DbSet.Where(predicate);
        }

        public async Task<TEntity> GetNonQueryWithIncludeAsync(Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            return await DbSet.Where(ApplyAuthPolicyFilter()).Where(predicate).Include(string.Join(".", include)).SingleOrDefaultAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> GetNonQueryAllWithIncludeAsync(params string[] include)
        {
            return await DbSet.Where(ApplyAuthPolicyFilter()).Include(string.Join(".", include)).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<List<TEntity>> GetAllAsyncAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(ApplyAuthPolicyFilter()).Where(predicate).ToListAsync();
        }

        public IQueryable<TEntity> GetWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            IQueryable<TEntity> query = DbSet.Where(ApplyAuthPolicyFilter());
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate);
        }

        public async Task<TEntity> GetFirstAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(ApplyAuthPolicyFilter()).FirstAsync(predicate);
        }

        public async Task<TEntity> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.Where(ApplyAuthPolicyFilter()).FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity> GetFirstOrDefaultAsyncAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AsNoTracking().Where(ApplyAuthPolicyFilter()).FirstOrDefaultAsync(predicate);
        }

        public async Task<TEntity> GetFirstOrDefaultAsyncAsNoTrackingWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            IQueryable<TEntity> query = DbSet.AsNoTracking().Where(ApplyAuthPolicyFilter());
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return await query.FirstOrDefaultAsync(predicate);
        }

        public virtual void Insert(TEntity entity)
        {
            _applyAuthPolicySecurity(entity);
            DbSet.Add(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _applyAuthPolicySecurity(entityToUpdate);
            DbSet.Update(entityToUpdate);
        }

        public void UpdateRange(List<TEntity> entities)
        {
            _applyAuthPolicySecurity(entities);
            DbSet.UpdateRange(entities);
        }

        public virtual void Delete(object id)
        {
            TEntity entiryToDelete = DbSet.Find(id);
            Delete(entiryToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            _applyAuthPolicySecurity(entityToDelete);
            if (_context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        public virtual void RemoveRange(IQueryable<TEntity> entityToDeleteList)
        {
            _applyAuthPolicySecurity(entityToDeleteList.ToList());
            DbSet.RemoveRange(entityToDeleteList);
        }

        public bool Exists(object primaryKey)
        {
            return DbSet.Find(primaryKey) != null;
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.AnyAsync(predicate);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await DbSet.CountAsync(predicate);
        }

        public void AddRange(List<TEntity> entities)
        {
            _applyAuthPolicySecurity(entities);
            DbSet.AddRange(entities);
        }

        public void RemoveRange(List<TEntity> entities)
        {
            _applyAuthPolicySecurity(entities);
            DbSet.RemoveRange(entities);
        }

        public Expression<Func<TEntity, bool>> ApplyAuthPolicyFilter()
        {
            Expression<Func<TEntity, bool>> expression = entity => true;
            //if (_policyContainer != null && !SkipPolicy && _policyContainer.HasPolicy<TEntity>(""))
            //{
            //    //   return _policyContainer.BuildWhereExpression<TEntity>();
            //}
            return expression;
        }

        private void _applyAuthPolicySecurity(TEntity entity)
        {
            _applyAuthPolicySecurity(new List<TEntity>() { entity });

        }

        private void _applyAuthPolicySecurity(List<TEntity> entities)
        {
            //if (_policyContainer != null && !SkipPolicy && _policyContainer.HasPolicy<TEntity>(""))
            //{
            //    //if (!entities.AsQueryable().Where(_policyContainer.BuildSaveExpression<TEntity>()).Any())
            //    //{
            //    //    //throw new UnauthorizedCustomException(CommonException.Name.Unauthorized);
            //    //}
            //}
        }
    }
}
