using NetIRC.Connection;
using NetIRC.Messages;
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
                client.EventHub.PrivMsg += EventHub_PrivMsg;
                Task.Run(() => client.ConnectAsync("irc.rizon.net", 6667, nickName, "NetIRC"));

                Console.ReadKey();
            }
        }

        private async static void EventHub_PrivMsg(Client client, IRCMessageEventArgs<Messages.PrivMsgMessage> e)
        {
            // Direct messages to me
            if (e.IRCMessage.To == nickName)
            {
                Console.WriteLine($"<{e.IRCMessage.From}> {e.IRCMessage.Message}");

                if (e.IRCMessage.From == myMaster)
                {
                    await client.SendAsync(new PrivMsgMessage(e.IRCMessage.From, $"Executing {e.IRCMessage.Message}..."));
                    await client.SendRaw(e.IRCMessage.Message);
                }
            }
        }

        private static void Client_OnRawDataReceived(Client client, string rawData)
        {
            Console.WriteLine(rawData);
        }
    }
}
