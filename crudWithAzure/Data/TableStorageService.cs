using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using crudWithAzure.Hubs;
using crudWithAzure.models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using CopyStatus = Microsoft.Azure.Storage.Blob.CopyStatus;

namespace crudWithAzure.Data
{
    public class TableStorageService<T>:ITableStorageService<FileData>
    {
        private const string TableName = "Item";
        private readonly IConfiguration _configuration;
        private IHubContext<MessageHub,IMessageHubClient> messageHub;
        public TableStorageService(IHubContext<MessageHub,IMessageHubClient>  _messageHub, IConfiguration configuration)
        {
            messageHub = _messageHub;
            _configuration = configuration;
        }
        /// <summary>
        /// ///////
        /// </summary>
        /// <returns></returns>
        // Azure Table created here
        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["StorageConnectionString"]);
            var tableClient = serviceClient.GetTableClient(TableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }
        /// <summary>
        /// ////////
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // Get All  here
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
        /// <summary>
        /// ///////
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="extension"></param>
        /// <param name="partitionKey"></param>
        /// <returns></returns>
        // Delete All  here from azure table and azure blob container
        public async Task<bool> DeleteEntityAsync(string name, string id,string extension,string partitionKey)
        {
            var tableClient = await GetTableClient();
            //here delete data from the azure table
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
                //get the data from container and then delete
                var blobClient = container.GetBlobClient(partitionKey + "." + extension); 
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
        /// <summary>
        /// //////////
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        // Get single record  here
        public async Task<FileData> GetEntityAsync(string fileName, string id)
        {
            var tableClient = await GetTableClient();
            var data= await tableClient.GetEntityAsync<FileData>(fileName, id);
            return data;
        }
        /// <summary>
        /// //////////
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        // Update and add data here upsert method
        public async Task<FileData> UpsertEntityAsync(FileData entity)
        {
            var tableClient = await GetTableClient();
            //Get the Blob   
            string StorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=bloblumakin;AccountKey=" +
                "Y61FT79SlbgigtBEvoylHBmBjrC33gi1GFYVV0tveZ67BX7xZxJeGrKOlNNN4LX1nG/ncKI0ammq+AStDGsDQw==;EndpointSuffix=core.windows.net";
            string ContainerName = "container-first";
            // Blob
            BlobServiceClient blobClient = new BlobServiceClient(StorageConnectionString);
            BlobContainerClient container = blobClient.GetBlobContainerClient(ContainerName);

            BlobClient sourceBlob = container.GetBlobClient(entity.PartitionKey + "." + entity.FileExtension);

            BlobClient newBlob = container.GetBlobClient(entity.FileName + "." + entity.FileExtension);

            await newBlob.StartCopyFromUriAsync(sourceBlob.Uri);

            await sourceBlob.DeleteIfExistsAsync();
            // delete from table pervious file name
            await tableClient.DeleteEntityAsync(entity.PartitionKey, entity.RowKey);

            // update the partition key so that new file record created on table .
            entity.PartitionKey = entity.FileName;




            //here we are update the data from azure table
            await tableClient.UpsertEntityAsync(entity);
            //***********************************
            var getData = await GetAllEntityAsync(entity.UserId);
                // SignalR
                var objNotifHub = new MessageHub();
                await objNotifHub.SignalR(getData, messageHub);
            return entity;
        }

        //var tableClient = await GetTableClient();
        //await tableClient.UpsertEntityAsync(entity);
        //var getData = await GetAllEntityAsync(entity.UserId);
        //// SignalR
        //var objNotifHub = new MessageHub();
        //await objNotifHub.SignalR(getData, messageHub);
        //return entity;


        /// <summary>
        /// ////////////////////////////
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        //create the data here 
        public async Task<FileData> CreateRecord(FileData entity)
        {
            var tableClient = await GetTableClient();
            await tableClient.UpsertEntityAsync(entity);
            var getData = await GetAllEntityAsync(entity.UserId);
            // SignalR
            var objNotifHub = new MessageHub();
            await objNotifHub.SignalR(getData, messageHub);
            return entity;
        }

        // Get container
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
