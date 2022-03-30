using AdCore.Entity;
using AdCore.Interface;
using AdRepository.Interface;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace AdRepository
{
    public class CosmosDbRepository<TEntity> : ICosmosDbRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ILogger<CosmosDbRepository<TEntity>> _logger;
        private readonly ICurrentUserService _currentUserService;
        private readonly Container _container;
        public CosmosDbRepository(CosmosClient cosmosDbClient, 
            ILogger<CosmosDbRepository<TEntity>> logger, 
            ICurrentUserService currentUserService)
        {
            _logger = logger;
            _currentUserService = currentUserService;
            var databaseName = CosmosClientInstance.DatabaseName;
            var containerName = CosmosClientInstance.ContainerName;
            _container = cosmosDbClient.GetContainer(databaseName, containerName);
        }

        public async Task<IList<TEntity>> GetAllAsync()
        {
            QueryDefinition query = new QueryDefinition(
                    "select * from c where c.type = @type ")
                .WithParameter("@type", typeof(TEntity).Name);
            var data = _container.GetItemQueryIterator<TEntity>(query);
            var results = new List<TEntity>();

            while (data.HasMoreResults)
            {
                var response = await data.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<IList<TEntity>> GetAsync(QueryDefinition query)
        {
            var data = _container.GetItemQueryIterator<TEntity>(query);
            var results = new List<TEntity>();
            while (data.HasMoreResults)
            {
                var response = await data.ReadNextAsync();
                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<TEntity> GetAsync(string id)
        {
            try
            {
                var response = await _container.ReadItemAsync<TEntity>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException e) //For handling item not found and other exceptions
            {
                _logger.LogError(e.Message, e);
                return null;
            }
        }

        public async Task<TEntity> AddAsync(TEntity item)
        {
            item.Id = Guid.NewGuid().ToString();
            item.Type = typeof(TEntity).Name;
            item.CreatedBy = _currentUserService.UserId;
            item.CreatedDate = DateTime.UtcNow;
            var response = await _container.CreateItemAsync(item, new PartitionKey(item.Id));
            return response.Resource;
        }

        public async Task<TEntity> UpdateAsync(string id, TEntity item)
        {
            item.Type = typeof(TEntity).Name;
            item.UpdateBy = _currentUserService.UserId;
            item.UpdateDate = DateTime.UtcNow;
            var response = await _container.UpsertItemAsync(item, new PartitionKey(id));
            return response.Resource;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            try
            {
                var response = await _container.DeleteItemStreamAsync(id, new PartitionKey(id));
                if (response.IsSuccessStatusCode) return true;
                return false;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message, e);
                return false;
            }
        }
    }
}
