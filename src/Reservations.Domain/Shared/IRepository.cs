using Reservations.Domain.Shared.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Reservations.Domain.Shared
{
    public interface IRepository<TEntity, in TPrimaryKey> where TEntity : IEntity<TPrimaryKey>
    {
        Task<List<TEntity>> GetAsync();

        Task<TEntity> GetAsync(TPrimaryKey id);

        void Add(TEntity entity);

        void Update(TEntity entity);

        void Remove(TPrimaryKey id);

        Task SaveChangesAsync();
    }
}
