using System.IO;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using SLIVA.Models;
namespace SLIVA.Controllers
{
    public class RegistrationController : Controller
    {
        string registration_status = "failed";
        public RegistrationController(HttpListenerContext context) : base(context)
        {
            UserRegistrationData registrationData;

            using (var streamReader = new StreamReader(_context.Request.InputStream))
            {
                string result = streamReader.ReadToEnd();
                registrationData = JsonConvert.DeserializeObject<UserRegistrationData>(result);

            }
            if (registrationData.Login != null)
            {
                User user = mySqlManager.RegistrateUser(registrationData, client);
                if (user != null)
                {
                    string sid = mySqlManager.GenerateSID(user);
                    mySqlManager.CreateSession(user, sid);
                    registration_status = "done";
                }
            }
        }
        public override byte[] GenerateResponseContent()
        {
            return Encoding.UTF8.GetBytes(registration_status);
        }

        public override HttpListenerResponse GenerateHttpListenerResponse()
        {

            return _context.Response;
        }

    }
}
