namespace RestaurantDAL.Interface
{
    public interface IUnitOfWork<TEntity>
    {
        IGenericRepository<TEntity> DbRepository();
        Task<int> SaveAsync();
    }
}
