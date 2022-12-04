using Newtonsoft.Json;
using SLIVA.Data;
namespace SLIVA.Models
{
    public class UserAuthenticationData
    {
        public string Login;
        public string Password;
    }
    public class User
    {
        public string Username;
        public string Login;
        public string Password;
        public string Emali;
        public int Id;

        public User(string username, string login, string password, string email, int id)
        {
            Username = username;
            Login = login;
            Password = password;
            Emali = email;
            Id = id;
        }
    }
}
