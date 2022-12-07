using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading;
using SLIVA.Models;
using SLIVA.Data;
using Newtonsoft.Json;
namespace SLIVA
{
    class HttpServer
    {

        static HttpListener listener;
        static string url = "http://localhost:8000/";
        RequestHandler requestHandler;
        static int free_connections = 5;

        public static void TakeRequest(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(TakeRequest, null);
            Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " Handling request");

            RequestHandler requestHandler = new RequestHandler(context);
            requestHandler.WriteResponse();

            free_connections++;

            Console.WriteLine(DateTime.UtcNow.ToString($"HH:mm:ss.fff : thread {Thread.CurrentThread.ManagedThreadId}") + " completed");
        }

        public static void Main(string[] args)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            while (true)
            {
                for(;free_connections > 0;free_connections--)
                {
                 listener.BeginGetContext(TakeRequest,null);
                    Console.WriteLine(free_connections);
                }
            }
        }
    }
}