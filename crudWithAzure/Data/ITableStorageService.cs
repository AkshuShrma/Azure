using crudWithAzure.Hubs;
using crudWithAzure.models;
using Microsoft.AspNetCore.SignalR;

namespace crudWithAzure.Data
{
    public interface ITableStorageService<T>
    {
        public Task<ICollection<T>> GetAllEntityAsync(int id);
        Task<FileData> GetEntityAsync(string fileName, string id);
        Task<FileData> UpsertEntityAsync(FileData entity);
        Task<FileData> CreateRecord(FileData entity);
        Task<bool> DeleteEntityAsync(string name, string id, string extension, string partitionKey);
    }
}
