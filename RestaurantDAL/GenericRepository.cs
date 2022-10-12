using Microsoft.EntityFrameworkCore;
using RestaurantDAL.Interface;
using System.Linq.Expressions;

namespace RestaurantDAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private DbContext _context;
        internal DbSet<TEntity> DbSet;
       
        public DbContext Context { get { return _context; } set { _context = value; DbSet = value.Set<TEntity>(); } }

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
      
        public virtual void Insert(TEntity entity)
        {
            DbSet.Add(entity);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            DbSet.Update(entityToUpdate);
        }

    }
}
