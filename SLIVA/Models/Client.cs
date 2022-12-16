using System;
using System.Net;
using SLIVA.Data;
namespace SLIVA.Models
{
    public class Client
    {
        public string Ip;

        public Client(MySqlManager mySqlManager, string ip)
        {
            Ip = ip;
            if (!mySqlManager.ClientExists(Ip))
                mySqlManager.InsertClient(Ip);


        }

    }
}
