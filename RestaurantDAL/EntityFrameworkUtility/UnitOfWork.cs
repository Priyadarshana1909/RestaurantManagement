using RestaurantDAL.Interface;

namespace RestaurantDAL.EntityFrameworkUtility
{
    public class UnitOfWork<TEntity> : IDisposable, IUnitOfWork<TEntity> where TEntity : class
    {

        private readonly RestaurantManagementContext _dbContext;
        private readonly IGenericRepository<TEntity> _repository;
        private bool _disposed;

        public UnitOfWork(RestaurantManagementContext dbContext, IGenericRepository<TEntity> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public IGenericRepository<TEntity> DbRepository()
        {
            _repository.Context = _dbContext;
            return _repository;
        }

        public async Task<int> SaveAsync()
        {
            var id = await _dbContext.SaveChangesAsync();
            return id;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _dbContext.Dispose();
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }   
}
