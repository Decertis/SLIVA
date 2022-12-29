using System;
using System.Net;
using SLIVA.Data;
namespace SLIVA.Models
{
    public class Client
    {
        public string Ip;
        public int Id;

        public Client(string ip, int id)
        {
            Ip = ip;
            Id = id;
        }

    }
}
