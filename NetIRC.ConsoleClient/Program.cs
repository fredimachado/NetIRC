using NetIRC.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetIRC.ConsoleClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var client = new Client(new TcpClientConnection()))
            {
                client.OnRawDataReceived += Client_OnRawDataReceived;
                Task.Run(() => client.ConnectAsync("irc.rizon.net", 6667, "NetIRCConsoleClient", "NetIRC"));

                Console.ReadKey();
            }
        }

        private static void Client_OnRawDataReceived(Client client, string rawData)
        {
            Console.WriteLine(rawData);
        }
    }
}
