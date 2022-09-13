using RestaurantDAL.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantDAL.EntityFrameworkUtility
{
    public class UnitOfWork<TEntity> : IDisposable, IUnitOfWork<TEntity> where TEntity : class
    {

        private readonly RestaurantManagementContext _dbContext;
        private readonly IGenericRepository<TEntity> _repository;
        private bool _disposed;

        public bool SkipPolicy { get; set; }

        public UnitOfWork(RestaurantManagementContext dbContext, IGenericRepository<TEntity> repository)
        {
            _dbContext = dbContext;
            _repository = repository;
        }

        public IGenericRepository<TEntity> DbRepository()
        {
            _repository.Context = _dbContext;
            _repository.SkipPolicy = SkipPolicy;
            return _repository;
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

        public async Task<int> SaveAsync()
        {
            var id = await _dbContext.SaveChangesAsync();
            return id;
        }
    }   
}
