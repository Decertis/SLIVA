using System;
using System.Text;
using System.Net;
using System.IO;
namespace SLIVA.Controllers
{
    public class ResourcesController : Controller
    {
        public ResourcesController(HttpListenerContext context) : base(context) { }
        byte[] _file_to_attach;
        public override HttpListenerResponse GenerateHttpListenerResponse()
        {
            return _context.Response;
        }
        public void GetFile()
        {
            //http://localhost:8000/resources/css/style.css
            string requested_file_name = _context.Request.RawUrl.ToLower().Split(new string[] { "resources" }, StringSplitOptions.RemoveEmptyEntries)[1]; 
            Console.WriteLine("Requested file : " + requested_file_name);
            try
            {
                _file_to_attach = File.ReadAllBytes(@"/home/decertis/Projects/SLIVA/SLIVA/wwwroot/" + requested_file_name);
                Console.WriteLine(_file_to_attach);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(FileNotFoundException) || ex.GetType() == typeof(FieldAccessException))
                    Console.WriteLine("File : \"" + requested_file_name + "\" was not found.");
                _file_to_attach = Encoding.UTF8.GetBytes("<center><h1>404</h1></center>");
            }
        }
        public override byte[] GenerateResponseContent()
        {
            GetFile();
            return _file_to_attach;
        }
    }
}
