using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;

namespace HttpListenerExample
{
    //it is master branch
    class HttpServer
    {
        static HttpListener listener;
        static string url = "http://localhost:8000/";
        static int pageViews = 0;
        static int requestCount = 0;
        static string pageData;


        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;

            while (runServer)
            {
                HttpListenerContext _context = await listener.GetContextAsync();

                HttpListenerRequest _request = _context.Request;
                HttpListenerResponse _response = _context.Response;

                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(_request.Url.ToString());
                Console.WriteLine(_request.HttpMethod);
                Console.WriteLine(_request.UserHostName);
                Console.WriteLine(_request.UserAgent);
                Console.WriteLine();

                if ((_request.HttpMethod == "POST") && (_request.Url.AbsolutePath == "/shutdown"))
                {
                    Console.WriteLine("Shutdown requested");
                    runServer = false;
                }

                if (_request.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;
                    
                string disableSubmit = !runServer ? "disabled" : "";
                byte[] data = Encoding.UTF8.GetBytes(String.Format(pageData, pageViews, disableSubmit));
                _response.ContentType = "text/html";
                _response.ContentEncoding = Encoding.UTF8;
                _response.ContentLength64 = data.LongLength;

                await _response.OutputStream.WriteAsync(data, 0, data.Length);
                _response.Close();
            }
        }


        public static void Main(string[] args)
        {
            pageData = File.ReadAllText("wwwroot/html/index.htm");

            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }
    }
}