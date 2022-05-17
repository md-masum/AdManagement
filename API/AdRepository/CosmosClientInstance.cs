using AdRepository.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AdRepository
{
    public static class CosmosClientInstance
    {
        public static string DatabaseName = "AdManagement";
        public static string ContainerName = "Store";
        public static async Task InitializeCosmosClientAsync(this IServiceCollection services, IConfigurationSection configurationSection)
        {
            var databaseName = configurationSection["DatabaseName"];
            var containerName = configurationSection["ContainerName"];
            var account = configurationSection["Account"];
            var key = configurationSection["Key"];
            var client = new Microsoft.Azure.Cosmos.CosmosClient(account, key);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            services.AddSingleton(client);
            services.AddScoped(typeof(ICosmosDbRepository<>), typeof(CosmosDbRepository<>));
        }
    }
}
