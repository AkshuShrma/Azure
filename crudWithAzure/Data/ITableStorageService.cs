using crudWithAzure.models;

namespace crudWithAzure.Data
{
    public interface ITableStorageService<T>
    {
        public Task<ICollection<T>> GetAllEntityAsync(int id);
        Task<FileData> GetEntityAsync(string fileName, string id);
        Task<FileData> UpsertEntityAsync(FileData entity);
        Task<bool> DeleteEntityAsync(string name, string id, string extension);
    }
}
