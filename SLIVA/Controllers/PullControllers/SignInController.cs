using System.Net;
using System.IO;
using Newtonsoft.Json;
using SLIVA.Models;
using System.Text;
using SLIVA.Views;

namespace SLIVA.Controllers
{
    public class SignInController : Controller
    {
        public SignInController(HttpListenerContext context) : base(context)
        {

        }

        public override byte[] GenerateResponseContent()
        {
            return Encoding.UTF8.GetBytes(GetPage("login.html").Html); ;
        }
        public override HttpListenerResponse GenerateHttpListenerResponse()
        {

            return _context.Response;
        }
    }
}
