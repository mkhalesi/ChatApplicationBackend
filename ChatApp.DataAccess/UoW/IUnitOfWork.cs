using ChatApp.DataAccess.Repository;
using ChatApp.Entities.Common;

namespace ChatApp.DataAccess.UoW
{
    public interface IUnitOfWork
    {
        IRepository<TEntity> GetRepository<TEntity>()
             where TEntity : BaseEntity;
    }
}
