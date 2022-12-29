using Newtonsoft.Json;
using SLIVA.Data;
namespace SLIVA.Models
{
    public class User
    {
        public string Username;
        public string Login;
        public string Password;
        public string Emali;
        public int Id;
        public Client Current_Client;

        public User(string username, string login, string password, string email, int id, Client current_client)
        {
            Username = username;
            Login = login;
            Password = password;
            Emali = email;
            Id = id;
            Current_Client = current_client;
        }
    }
}
