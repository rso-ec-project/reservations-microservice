using Microsoft.EntityFrameworkCore;
using Reservations.Domain.Shared;
using Reservations.Domain.Shared.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Reservations.Infrastructure.Repositories
{
    public class Repository<TEntity, TPrimaryKey> : IRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        protected readonly ApplicationDbContext Context;

        public virtual DbSet<TEntity> DbSet => Context.Set<TEntity>();

        public Repository(ApplicationDbContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected virtual IQueryable<TEntity> Set()
        {
            return Context.Set<TEntity>();
        }

        public virtual async Task<List<TEntity>> GetAsync()
        {
            var result = await Set().AsTracking().ToListAsync();
            return result.Any() ? result : null;
        }

        public virtual async Task<TEntity> GetAsync(TPrimaryKey id)
        {
            return await Set().FirstOrDefaultAsync(x => x.Id.Equals(id));
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            var addedEntity = await Context.Set<TEntity>().AddAsync(entity);
            return addedEntity.Entity;
        }

        public virtual TEntity Update(TEntity entity)
        {
            var updatedEntity = Context.Set<TEntity>().Update(entity);
            return updatedEntity.Entity;
        }

        public virtual void Remove(TPrimaryKey id)
        {
            var entity = Context.Set<TEntity>().Find(id);

            Context.Set<TEntity>().Remove(entity);
        }

        public async Task SaveChangesAsync() => await Context.SaveChangesAsync();
    }
}
