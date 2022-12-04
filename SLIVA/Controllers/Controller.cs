using System.Text;
using System.Net;
using SLIVA.Models;
namespace SLIVA.Controllers
{
    public abstract class Controller
    {
        protected HttpListenerContext _context;
        public Controller(HttpListenerContext context)
        {
            this._context = context;
            Client client = new Client(_context.Request.RemoteEndPoint.ToString());
        }
        public virtual HttpListenerResponse GenerateHttpListenerResponse()
        {
            return _context.Response;
        }
        public virtual byte[] GenerateResponseContent()
        {
            return Encoding.UTF8.GetBytes("<h1>Vibe check passed</h1>");
        }

    }
}
