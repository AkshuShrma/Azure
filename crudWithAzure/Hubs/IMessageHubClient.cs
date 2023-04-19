using crudWithAzure.models;

namespace crudWithAzure.Hubs
{
    public interface IMessageHubClient
    {
        Task SignalR(ICollection<FileData> entity);
    }
}
