using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace RestaurantDAL.Interface
{
    public interface IGenericRepository<TEntity>
    {
        DbContext Context { get; set; }

        List<TEntity> GetQueryWithIncludes(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, params string[] include);

      
    }
}
