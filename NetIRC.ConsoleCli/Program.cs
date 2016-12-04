using NetIRC.Connection;
using System;
using System.Threading.Tasks;

namespace NetIRC.ConsoleCli
{
    public class Program
    {
        private const string nickName = "NetIRCConsoleClient";
        private const string myMaster = "Fredi_"; // Who can control me

        static void Main(string[] args)
        {
            using (var client = new Client(new TcpClientConnection()))
            {
                client.OnRawDataReceived += Client_OnRawDataReceived;
                client.OnPrivMsgReceived += Client_OnPrivMsgReceived;
                Task.Run(() => client.ConnectAsync("irc.rizon.net", 6667, nickName, "NetIRC"));

                Console.ReadKey();
            }
        }

        private static async void Client_OnPrivMsgReceived(Client client, PrivMsgEventArgs args)
        {
            // Direct messages to me
            if (args.To == nickName)
            {
                Console.WriteLine($"<{args.From}> {args.Message}");

                if (args.From == myMaster)
                {
                    await client.SendRaw($"PRIVMSG {args.From} :Executing {args.Message}...");
                    await client.SendRaw(args.Message);
                }
            }
        }

        private static void Client_OnRawDataReceived(Client client, string rawData)
        {
            Console.WriteLine(rawData);
        }
    }
}
