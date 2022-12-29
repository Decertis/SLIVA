using System;
using System.IO;
using System.Text;
using System.Net;
using System.Threading;
using SLIVA.Models;
using SLIVA.Data;
using SLIVA.Views;
using Newtonsoft.Json;
namespace SLIVA
{
    class HttpServer
    {

        static HttpListener listener;
        //static string url = "http://localhost:8000/";
        static string url = "http://192.168.0.10:8000/";
        static int free_connections = 5;
        static bool server_must_stop = false;

        public static void Main(string[] args)
        {
            PageManager.Initialize("/home/decertis/Projects/SLIVA/SLIVA/wwwroot/html");
            PageManager.ListPages();

            new MySqlManager().ConsoleWriteClients();
            new MySqlManager().ConsoleWriteUsers();

            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            while (server_must_stop == false)
            {
                for(;free_connections > 0;free_connections--)
                {
                    listener.BeginGetContext(TakeRequest,null);
                }
            }
        }

        public static void TakeRequest(IAsyncResult ar)
        {
            var context = listener.EndGetContext(ar);
            listener.BeginGetContext(TakeRequest, null);
            Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " Handling request : " + context.Request.Url.AbsolutePath);
            
            RequestHandler requestHandler = new RequestHandler(context);

            requestHandler.WriteResponse();

            free_connections++;

            Console.WriteLine(DateTime.UtcNow.ToString($"HH:mm:ss.fff : thread {Thread.CurrentThread.ManagedThreadId}") + " completed");
            Console.WriteLine("\n===================================\n");
        }

    }
}