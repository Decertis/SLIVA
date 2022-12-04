using System;
using System.IO;
using System.Net;
using System.Linq;
using System.Reflection;
using SLIVA.Controllers;
using System.Threading.Tasks;
namespace SLIVA
{
    public class RequestHandler
    {
        HttpListenerContext _context;
        string _requested_controller_name;
        string _requested_method_name;
        string _request_url;
        HttpListenerResponse _response;

        Controller _controller;
        MethodInfo _requested_method;
        byte[] _response_content;


        public RequestHandler(HttpListenerContext context)
        {
            _context = context;
            _request_url = _context.Request.RawUrl;
            _requested_controller_name = _request_url.Split('/')[1];

            if (_request_url.Split('/').Length > 2)
                _requested_method_name = _request_url.Split('/')[2].Split('?')[0];

            _controller = GetController();

            if (_controller == null)
            {
                _controller = new IndexController(context);
                Console.WriteLine($"Default controller : {_controller.GetType().Name}");
            }

            _requested_method = GetRequestedMethod();

            if (_requested_method != null)
                _requested_method.Invoke(_controller, null);

            _response = _controller.GenerateHttpListenerResponse();
            _response_content = _controller.GenerateResponseContent();

        }


       public async Task WriteResponse()
        {
            using (Stream output = _response.OutputStream)
            {
                await output.WriteAsync(_response_content, 0, _response_content.Length);
            }
        }

        Controller GetController()
        {
            try
            {
                Type controller_type = typeof(Controller).Assembly.GetTypes()
                    .Where(type => !type.IsAbstract && string.Equals(type.Name, _requested_controller_name + "Controller", StringComparison.OrdinalIgnoreCase))
                        .SingleOrDefault();
                Console.WriteLine($"Controller : {controller_type.Name}");

                return (Controller)Activator.CreateInstance(controller_type, _context);
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(NullReferenceException))
                    Console.WriteLine(ex);

                return null;
            }
        }

        MethodInfo GetRequestedMethod()
        {
            try
            {
                MethodInfo methodInfo = _controller.GetType().GetMethods()
                     .Where(method => string.Equals(method.Name, _requested_method_name, StringComparison.CurrentCultureIgnoreCase))
                        .SingleOrDefault();
                Console.WriteLine($"Method : {methodInfo.Name}");

                return methodInfo;
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(NullReferenceException))
                    Console.WriteLine(ex);

                return null;
            }

        }


    }
}