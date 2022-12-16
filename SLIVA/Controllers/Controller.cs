using System.Text;
using System;
using System.Net;
using SLIVA.Models;
using SLIVA.Views;
using SLIVA.Data;
namespace SLIVA.Controllers
{
    public abstract class Controller
    {
        protected HttpListenerContext _context;
        protected MySqlManager mySqlManager;
        public Controller(HttpListenerContext context)
        {
            mySqlManager = new MySqlManager();
            this._context = context;
            Client client = new Client(mySqlManager,_context.Request.RemoteEndPoint.ToString());
        }
        public virtual HttpListenerResponse GenerateHttpListenerResponse()
        {
            return _context.Response;
        }
        public virtual byte[] GenerateResponseContent()
        {
            return Encoding.UTF8.GetBytes("<h1>Vibe check passed</h1>");
        }
        public virtual Page GetPage(string page_name)
        {
            Page result = null;
            foreach (Page page in PageManager.Pages)
            {
                if (page.Name == page_name)
                {
                    result = page;
                    break;
                }
            }

            return result;
        }
        public virtual string GetPageWithUserData(Page page, User user)
        {
            return null;
        }
    }
}
