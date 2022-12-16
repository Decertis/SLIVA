using System.Text;
using System.Net;
using SLIVA.Data;
using SLIVA.Models;
using SLIVA.Views;
namespace SLIVA.Controllers
{
    public class AuthenticationController : Controller
    {
        public AuthenticationController(HttpListenerContext context) : base(context)
        {
            UserAuthenticationData authData = new UserAuthenticationData(_context);
            if (authData.Password != null && authData.Login != null)
            {
                User user = mySqlManager.GetUser(authData);
                if (user != null)
                {
                    mySqlManager.CreateSession(user);
                }
            }
        }

        public override HttpListenerResponse GenerateHttpListenerResponse()
        {

            return _context.Response;
        }

    }
}
