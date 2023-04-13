using crudWithAzure.models;

namespace crudWithAzure.Data
{
    public interface ITableStorageService<T>
    {
        public Task <ICollection<T>> GetAllEntityAsync();
        Task<FileData> GetEntityAsync(string fileName, string id);
        Task<FileData> UpsertEntityAsync(FileData entity);
        Task DeleteEntityAsync(string fileName, string id);
    }
}
