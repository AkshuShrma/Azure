using Azure.Data.Tables;
using crudWithAzure.models;

namespace crudWithAzure.Data
{
    public class TableStorageService<T>:ITableStorageService<FileData>
    {
        private const string TableName = "Item";
        private readonly IConfiguration _configuration;
        public TableStorageService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["StorageConnectionString"]);
            var tableClient = serviceClient.GetTableClient(TableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }

        public async Task<ICollection<FileData>> GetAllEntityAsync()
        {
            ICollection<FileData> getAllData = new List<FileData>();

            var tableClient = await GetTableClient();

            var celebs = tableClient.QueryAsync<FileData>(filter:"");


            await foreach (var fileDatas  in celebs)
            {
                getAllData.Add(fileDatas);
            }
            return getAllData;
        }

        public async Task DeleteEntityAsync(string fileName, string id)
        {
            var tableClient = await GetTableClient();
            await tableClient.DeleteEntityAsync(fileName, id);
        }

        public async Task<FileData> GetEntityAsync(string fileName, string id)
        {
            var tableClient = await GetTableClient();
            var data= await tableClient.GetEntityAsync<FileData>(fileName, id);
            return data;
        }

        public async Task<FileData> UpsertEntityAsync(FileData entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.UpsertEntityAsync(entity);
            return entity;
        }
    }
}
