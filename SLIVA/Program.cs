using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using SLIVA.Models;
using SLIVA.Data;
using Newtonsoft.Json;
namespace SLIVA
{
    class HttpServer
    {
        static HttpListener listener;
        static string url = "http://localhost:8000/";
        static int pageViews = 0;
        static int requestCount = 0;

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            while (runServer)
            {
                listener.Start();
                HttpListenerContext _context = await listener.GetContextAsync();

                HttpListenerRequest _request = _context.Request;
                HttpListenerResponse _response = _context.Response;

                Console.WriteLine("Request #: {0}", ++requestCount);
                Console.WriteLine(_request.Url.ToString());
                Console.WriteLine(_context.Request.RemoteEndPoint);
                Console.WriteLine(_request.HttpMethod);
                Console.WriteLine(_request.UserHostName);
                Console.WriteLine(_request.UserAgent);
                Console.WriteLine();



                if (_request.Url.AbsolutePath != "/favicon.ico")
                    pageViews += 1;

                var requestHandler = new RequestHandler(_context);
                await requestHandler.WriteResponse();
                listener.Stop();

            }
        }


        public static void Main(string[] args)
        {


            Console.WriteLine("Listening for connections on {0}", url);
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            listener.Close();
        }
    }
}