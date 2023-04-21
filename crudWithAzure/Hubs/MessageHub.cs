using crudWithAzure.models;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace crudWithAzure.Hubs
{
    public class MessageHub : Hub<IMessageHubClient>
    {
        private static readonly ConcurrentDictionary<string, HubModel> Users =
           new ConcurrentDictionary<string, HubModel>(StringComparer.InvariantCultureIgnoreCase);
        //here create the connection by id of user
        public async Task SignalR(ICollection<FileData> entity,IHubContext<MessageHub,IMessageHubClient> messageHub)
        {
            var getspecificUserDectionary = Users.Where(m => m.Value.id == entity.FirstOrDefault().UserId.ToString());
            IReadOnlyList<string> connections;
            var list = new List<string>();
            foreach(var connection in getspecificUserDectionary)
            {
                list.Add(connection.Value.connectionId.ToString());   
            }
            connections = list;
            await messageHub.Clients.Clients(connections).SignalR(entity);
        }
        //here check connection is connected or not 
        public async override Task OnConnectedAsync()
        {
            var userhubModels = new HubModel()
            {
                connectionId = Context.ConnectionId
            };
            Users.TryAdd(Context.ConnectionId.ToString(), userhubModels);
            await base.OnConnectedAsync();
        }
        //here create method for connection
        public void UserId(string id)
        {
            if (id == "") return;
            //now we will check int the users that owr conneciton id present or not
            Users.Remove(Context.ConnectionId, out var data);
            if(data != null)
            {
                data.id = id;
                Users.TryAdd(Context.ConnectionId.ToString(), data);
            }
        }
        //here check connection is disconnected or not 
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            Users.Remove(Context.ConnectionId,out HubModel userhub);
           await base.OnDisconnectedAsync(exception);
        }
    }
    public  class HubModel
    {
        public string id { get; set; } = default(string);
        public  string connectionId { get; set; } 
    }
}
