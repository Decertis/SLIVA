using System.Text;
using System.Net;
namespace SLIVA.Controllers
{
    public class AuthenticationController : Controller
    {
        public AuthenticationController(HttpListenerContext context) : base(context) { }

        public override HttpListenerResponse GenerateHttpListenerResponse()
        {
            return _context.Response;
        }
    }
}
