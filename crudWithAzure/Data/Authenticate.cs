using Azure.Data.Tables;
using crudWithAzure.models;

namespace crudWithAzure.Data
{
    public class Authenticate:IAuthenticateUser
    {
        private const string TableName = "Login1";
        private readonly IConfiguration _configuration;
        public Authenticate(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Create table here
        private async Task<TableClient> GetTableClient()
        {
            var serviceClient = new TableServiceClient(_configuration["StorageConnectionString"]);
            var tableClient = serviceClient.GetTableClient(TableName);
            await tableClient.CreateIfNotExistsAsync();
            return tableClient;
        }

        // login user code
        public User? authenticateUser(string userName, string password)
        {
            try
            {
                var getServiceClient = new TableServiceClient(_configuration["StorageConnectionString"]);
                var tableClient = getServiceClient.GetTableClient(TableName);
                // check enter details correct or not
                var checkUser = tableClient.Query<User>(m => m.UserName == userName && m.Password == password);
                if (checkUser.FirstOrDefault() != null)
                {
                    return checkUser.FirstOrDefault();
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                return null;
            }

        }

        //public async Task<ICollection<User>> GetAllEntityAsync()
        //{
        //    ICollection<User> getAllData = new List<User>();

        //    var tableClient = await GetTableClient();

        //    var celebs = tableClient.QueryAsync<User>(filter: "");


        //    await foreach (var fileDatas in celebs)
        //    {
        //        getAllData.Add(fileDatas);
        //    }
        //    return getAllData;
        //}

        //public async Task<bool> AuthenticateUser(string username, string password)
        //{
        //    var tableClient = await GetTableClient();

        //    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        //    {
        //        Results.NotFound();
        //    }
        //    //var data =  await tableClient.GetEntityIfExistsAsync<UserCredentials>(username, password);
        //    //if (data.Value.Username == username && data.Value.Password == password) return true;

        //    //else return false;
        //    var data = await GetAllUsers();
        //    foreach (var user in data)
        //    {
        //        if ((user.UserName == username && user.Password == password))
        //        {
        //            return true;
        //        }
        //        else continue;

        //    }
        //    return false;


        //}

        //public async Task<ICollection<User>> GetAllUsers()
        //{
        //    ICollection<User> getAllData = new List<User>();

        //    var tableClient = await GetTableClient();

        //    var celebs = tableClient.QueryAsync<User>(filter: "");


        //    await foreach (var fileDatas in celebs)
        //    {
        //        getAllData.Add(fileDatas);
        //    }
        //    return getAllData;
        //}
    }
}
