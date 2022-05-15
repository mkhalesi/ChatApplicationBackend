using ChatApp.Entities.Common;
using ChatApp.Entities.Models.Access;

namespace ChatApp.DataAccess.Repository
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IQueryable<TEntity> GetQuery();
        Task AddEntity(TEntity entity);
        Task<TEntity?> GetEntityById(long entityId);
        Task AddRangeEntities(List<TEntity> entities);
        void EditEntity(TEntity entity);
        void DeleteEntity(TEntity entity);
        Task DeleteEntity(long entityId);
        void DeleteEntities(List<TEntity> entities);
        void DeletePermanent(TEntity entity);
        void DeletePermanentEntities(List<TEntity> entities);
        Task DeletePermanent(long entityId);
        Task SaveChanges();
    }
}
