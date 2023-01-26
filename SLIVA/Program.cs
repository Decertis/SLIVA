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
        static string[] prefixes = { "http://localhost:8000/", "http://192.168.0.10:8000/" };
        static int free_connections = 5;
        static bool server_must_stop = false;

        public static void Main(string[] args)
        {
            PageManager.Initialize("/home/decertis/Projects/SLIVA/SLIVA/wwwroot/html");
            PageManager.ListPages();

            MySqlManager mySqlManager = new MySqlManager();
            mySqlManager.ConsoleWriteClients();
            mySqlManager.ConsoleWriteUsers();

            foreach(Message message in mySqlManager.GetLatestMessages(5))
            {
                if(message != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("Message id : " + message.Id);
                    Console.WriteLine("Message author_id : " + message.AuthorId);
                    Console.WriteLine("Message client_id : " + message.ClientId);
                    Console.WriteLine("Message content_id : " + message.Content);
                    Console.WriteLine("Message sent_at : " + message.SentAt);


                }
            }


            listener = new HttpListener();
            listener.Prefixes.Add(prefixes[0]);
            listener.Prefixes.Add(prefixes[1]); 
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
            Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + " Handling request : " + context.Request.Url.AbsolutePath);

            RequestHandler requestHandler = new RequestHandler(context);

            requestHandler.WriteResponse();

            free_connections++;

            Console.WriteLine(DateTime.UtcNow.ToString($"HH:mm:ss.fff : thread {Thread.CurrentThread.ManagedThreadId}") + " completed");
            Console.WriteLine("\n===================================\n");
        }

    }
}