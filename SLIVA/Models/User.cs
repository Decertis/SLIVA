using Newtonsoft.Json;
using SLIVA.Data;
namespace SLIVA.Models
{
    public class User
    {
        public string Login;
        public string Password;
        public int Id;
        public Client Current_Client;

        public User(string login, string password, int id, Client current_client)
        {
            Login = login;
            Password = password;
            Id = id;
            Current_Client = current_client;
        }
    }
}
