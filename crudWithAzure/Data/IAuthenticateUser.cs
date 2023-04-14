using crudWithAzure.models;

namespace crudWithAzure.Data
{
    public interface IAuthenticateUser
    {
        public User? authenticateUser(string userName, string password);
        //public Task<ICollection<T>> GetAllEntityAsync();
    }
}
