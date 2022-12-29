using System.Text;
using System.IO;
using System.Net;
namespace SLIVA.Controllers
{
    public class IndexController : Controller
    {
        public IndexController(HttpListenerContext context) : base(context) { }

        public override HttpListenerResponse GenerateHttpListenerResponse()
        {
            return _context.Response;
        }
        public override byte[] GenerateResponseContent()
        {
            return Encoding.UTF8.GetBytes(File.ReadAllText("/home/decertis/Projects/SLIVA/SLIVA/wwwroot/html/index.html"));
        }
    }
}
