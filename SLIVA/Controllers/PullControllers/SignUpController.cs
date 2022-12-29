using System.Net;
using System.IO;
using Newtonsoft.Json;
using SLIVA.Models;
using System.Text;
using SLIVA.Views;

namespace SLIVA.Controllers
{
    public class SignUpController : Controller
    {
        public SignUpController(HttpListenerContext context) : base(context)
        {

        }

        public override byte[] GenerateResponseContent()
        {
            return Encoding.UTF8.GetBytes(GetPage("registration.html").Html); ;
        }
        public override HttpListenerResponse GenerateHttpListenerResponse()
        {

            return _context.Response;
        }
    }
}
