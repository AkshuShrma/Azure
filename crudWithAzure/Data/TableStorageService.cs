using Azure.Data.Tables;
using Azure.Storage.Blobs;
using crudWithAzure.models;
using System.Collections.Concurrent;

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

        public async Task<ICollection<FileData>> GetAllEntityAsync(int id)
        {
            if (id == 0) return null;
            ICollection<FileData> getAllData = new List<FileData>();

            var tableClient = await GetTableClient();

            var celebs = tableClient.QueryAsync<FileData>(filter: m => m.UserId == id);


            await foreach (var fileDatas  in celebs)
            {
                getAllData.Add(fileDatas);
            }
            return getAllData;
        }

        public async Task<bool> DeleteEntityAsync(string name, string id,string extension)
        {
            var tableClient = await GetTableClient();
            var removeData = tableClient.DeleteEntity(name, id);
            if (removeData.Status == 204)
            {
                string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=bloblumakin;AccountKey=Y61FT79SlbgigtBEvoylHBmBjrC33gi1GFYVV0tveZ67BX7xZxJeGrKOlNNN4LX1nG/ncKI0ammq+AStDGsDQw==;EndpointSuffix=core.windows.net";
                string ContainerName = "container-first";
                var container = BlobExtensions.GetContainer(StorageConnectionString, ContainerName);
                if (!await container.ExistsAsync())
                {
                    return false;
                }

                var blobClient = container.GetBlobClient(name + "." + extension); 
                if (await blobClient.ExistsAsync())
                {
                    await blobClient.DeleteIfExistsAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
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
         static class BlobExtensions
        {
            public static BlobContainerClient GetContainer(string connectionString, string ContainerName)
            {
                BlobServiceClient blobServiceClient = new(connectionString);
                return blobServiceClient.GetBlobContainerClient(ContainerName);
            }
        }
    }
}
