using System;
using Newtonsoft.Json;
using SLIVA.Data;
namespace SLIVA.Models
{
    public class UserAuthenticationData
    {
        /* Serialization/Deserialization of json file
            var authenticationData = new UserAuthenticationData()
            {
                Login = "decertis",
                Password = "55665566"
            };

            string json_string = JsonConvert.SerializeObject(authenticationData);
            Console.WriteLine(json_string);

            authenticationData = JsonConvert.DeserializeObject<UserAuthenticationData>(json_string);
            Console.WriteLine($"Login {authenticationData.Login}\nPassword {authenticationData.Password}");
            */

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
