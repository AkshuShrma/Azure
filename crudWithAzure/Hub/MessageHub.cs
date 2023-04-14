using crudWithAzure.models;
using Microsoft.AspNetCore.SignalR;

namespace crudWithAzure.Hub
{
    public class MessageHub : Hub<IMessageHubClient>
    {
        public async Task SendOffersToUser(FileData entity)
        {
            await Clients.All.SendOffersToUser(entity);
        }
    }
}
