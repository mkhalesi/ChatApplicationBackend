using ChatApp.DataAccess.Context;
using ChatApp.DataAccess.Repository;
using ChatApp.Entities.Common;

namespace ChatApp.DataAccess.UoW
{
    public class UnitOfWork : IUnitOfWork
    {
        private Dictionary<Type, object> _repositories;
        private ChatAppDbContext _dbContext;
        public UnitOfWork(ChatAppDbContext dbContext)
        {
            _repositories = new Dictionary<Type, object>();
            _dbContext = dbContext;
        }

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            var type = typeof(Repository<TEntity>);
            if (!_repositories.ContainsKey(type))
            {
                _repositories[type] =
                    new Repository<TEntity>(_dbContext);
            }

            return (IRepository<TEntity>)_repositories[type];
        }
    }
}
