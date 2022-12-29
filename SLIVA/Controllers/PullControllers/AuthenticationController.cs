using System.Text;
using System.IO;
using System.Net;
using SLIVA.Data;
using Newtonsoft.Json;
using SLIVA.Models;
using SLIVA.Views;
namespace SLIVA.Controllers
{
    public class AuthenticationController : Controller
    {
        string authentication_status = "failed";
        public AuthenticationController(HttpListenerContext context) : base(context)
        {
            UserAuthenticationData authData;

            using (var streamReader = new StreamReader(_context.Request.InputStream))
            {
                string result = streamReader.ReadToEnd();
                authData = JsonConvert.DeserializeObject<UserAuthenticationData>(result);

            }
            if (authData.Login != null)
            {
                User user = mySqlManager.GetUser(authData,client);
                if (user != null)
                {
                    authentication_status = "succes";
                    string sid = mySqlManager.GenerateSID(user);
                    mySqlManager.CreateSession(user, sid);
                }
            }
        }
        public override byte[] GenerateResponseContent()
        {
            return Encoding.UTF8.GetBytes(authentication_status);
        }
        public override HttpListenerResponse GenerateHttpListenerResponse()
        {

            return _context.Response;
        }

    }
}
