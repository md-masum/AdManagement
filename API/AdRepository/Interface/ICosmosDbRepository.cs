using AdCore.Entity;
using Microsoft.Azure.Cosmos;

namespace AdRepository.Interface
{
    public interface ICosmosDbRepository<TEntity> where TEntity : BaseEntity
    {
        Task<IList<TEntity>> GetAllAsync();
        Task<IList<TEntity>> GetAllAsync(QueryDefinition query);
        Task<TEntity> GetAsync(string id);
        Task<TEntity> AddAsync(TEntity item);
        Task<TEntity> UpdateAsync(string id, TEntity item);
        Task<bool> DeleteAsync(string id);
        Task<TEntity> GetAsync(QueryDefinition query);
    }
}
