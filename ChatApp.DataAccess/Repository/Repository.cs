using ChatApp.DataAccess.Context;
using ChatApp.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.DataAccess.Repository
{
    public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : BaseEntity
    {
        private readonly ChatAppDbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(ChatAppDbContext context)
        {
            _context = context;
            this._dbSet = _context.Set<TEntity>();
        }

        public IQueryable<TEntity> GetQuery()
        {
            return _dbSet.AsQueryable();
        }

        public async Task AddEntity(TEntity entity)
        {
            entity.CreatedAt = DateTime.Now;
            entity.UpdatedAt = DateTime.Now;
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeEntities(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                await AddEntity(entity);
            }
        }

        public async Task<TEntity> GetEntityById(long entityId)
        {
            return await _dbSet.SingleOrDefaultAsync(s => s.Id == entityId);
        }

        public void EditEntity(TEntity entity)
        {
            entity.UpdatedAt = DateTime.Now;
            _dbSet.Update(entity);
        }

        public void DeleteEntity(TEntity entity)
        {
            entity.IsDeleted = true;
            EditEntity(entity);
        }

        public void DeleteEntities(List<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                entity.IsDeleted = true;
                EditEntity(entity);
            }
        }
        public async Task DeleteEntity(long entityId)
        {
            TEntity entity = await GetEntityById(entityId);
            if (entity != null) DeleteEntity(entity);
        }

        public void DeletePermanent(TEntity entity)
        {
            _dbSet.Remove(entity);
        }

        public void DeletePermanentEntities(List<TEntity> entities)
        {
            _context.RemoveRange(entities);
        }

        public async Task DeletePermanent(long entityId)
        {
            TEntity entity = await GetEntityById(entityId);
            if (entity != null) DeletePermanent(entity);
        }

        public async Task SaveChanges()
        {
            await _context.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _context.DisposeAsync();
        }
    }
}
