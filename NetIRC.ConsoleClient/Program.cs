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
        private const string nickName = "NetIRCConsoleClient";
        private const string myMaster = "Fredi_"; // Who can control me

        static void Main(string[] args)
        {
            using (var client = new Client(new TcpClientConnection()))
            {
                client.OnRawDataReceived += Client_OnRawDataReceived;
                client.OnIRCMessageReceived += Client_OnIRCMessageReceived;
                Task.Run(() => client.ConnectAsync("irc.rizon.net", 6667, nickName, "NetIRC"));

                Console.ReadKey();
            }
        }

        private static async void Client_OnIRCMessageReceived(Client client, IRCMessage ircMessage)
        {
            // Direct messages to me
            if (ircMessage.Command == "PRIVMSG" && ircMessage.Parameters[0] == nickName)
            {
                var from = ircMessage.Prefix.From;

                Console.WriteLine($"<{from}> {ircMessage.Trailing}");

                if (from == myMaster)
                {
                    await client.SendRaw($"PRIVMSG {from} :Executing {ircMessage.Trailing}...");
                    await client.SendRaw(ircMessage.Trailing);
                }
            }
        }

        private static void Client_OnRawDataReceived(Client client, string rawData)
        {
            Console.WriteLine(rawData);
        }
    }
}
