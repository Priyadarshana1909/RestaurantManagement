namespace RestaurantDAL.Interface
{
    public interface IUnitOfWork<TEntity>
    {
        bool SkipPolicy { get; set; }
        IGenericRepository<TEntity> DbRepository();
        Task<int> SaveAsync();
    }
}
