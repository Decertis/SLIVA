using System;
using Newtonsoft.Json;
using System.IO;
using System.Net;
namespace SLIVA.Models
{
    public class UserRegistrationData
    {
        public string Login;
        public string Password;
        public string Email;
        public string Username;

        // {"Username":"aasdasd","Login":"asdasd","Email":"max","Password":"asdasd"}
        public UserRegistrationData(HttpListenerContext _context)
        {
        }
    }
}
