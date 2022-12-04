using System;
using System.Net;
using SLIVA.Data;
namespace SLIVA.Models
{
    public class Client
    {
        public int Id;
        public string Ip;
        public byte[] BinaryIp;
        public bool DatabaseContainsClient;

        public Client(string client_ip)
        {
            Ip = client_ip;
            BinaryIp = GetIpAddressBinary();
        }
        public void Initialize(MySqlManager sqlManager)
        {
            DatabaseContainsClient = sqlManager.IsDatabaseCointainsClient(BinaryIp);
            Console.WriteLine("Client exist in database : " + DatabaseContainsClient);
            if (!DatabaseContainsClient)
                sqlManager.InsertClientIpIntoDatabase(BinaryIp);

        }
        private byte[] GetIpAddressBinary()
        {
            IPAddress IPDec = IPAddress.Parse(Ip);
            byte[] IPByte = IPDec.GetAddressBytes();
            return IPByte;

        }
    }
}
