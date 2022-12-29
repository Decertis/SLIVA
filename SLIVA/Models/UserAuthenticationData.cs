using System;
using System.Net;
namespace SLIVA.Models
{
    public class UserAuthenticationData
    {
        public string Login;
        public string Password;

        public UserAuthenticationData(HttpListenerContext _context)
        {

        }
    }
}
