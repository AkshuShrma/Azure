using crudWithAzure.models;

namespace crudWithAzure.Hub
{
    public interface IMessageHubClient
    {
        Task SendOffersToUser(FileData entity);
    }
}
