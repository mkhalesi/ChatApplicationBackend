using ChatApp.DataAccess.Context;
using ChatApp.DataAccess.Repository;
using ChatApp.Entities.Common;
using Microsoft.AspNetCore.Http;

namespace ChatApp.DataAccess.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private Dictionary<Type, object> _repositories;
        private ChatAppDbContext _dbContext;
        private IHttpContextAccessor _httpContextAccessor;
        public UnitOfWork(ChatAppDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _repositories = new Dictionary<Type, object>();
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(Repository<TEntity>);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] =
                    new Repository<TEntity>(_dbContext, _httpContextAccessor);
            }

            return (IRepository<TEntity>)_repositories[type];
        }
    }
}
