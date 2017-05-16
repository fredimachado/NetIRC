using Moq;
using NetIRC.Connection;
using NetIRC.Messages;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NetIRC.Tests
{
    public class ClientTests
    {
        private static User FakeUser = new User("test", "test");

        [Fact]
        public void TriggersRawDataReceived()
        {
            var raw = "PING xyz.com";
            var rawReceived = string.Empty;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.OnRawDataReceived += (c, d) => rawReceived = d;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(raw, rawReceived);
        }

        [Fact]
        public void RespondsToPingMessage()
        {
            var data = "xyz.com";
            var raw = $"PING :{data}";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            RaiseDataReceived(mockConnection, client, raw);

            mockConnection.Verify(c => c.SendAsync($"PONG {data}"), Times.Once());
        }

        [Fact]
        public void TriggersOnPingEvent()
        {
            var raw = "PING :xyz.com";
            IRCMessageEventArgs<PingMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.Ping += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal("xyz.com", args.IRCMessage.Target);
        }

        [Fact]
        public async Task SendsNickAndUserWhenConnected()
        {
            var nick = "guest";
            var realName = "Ronnie Reagan";
            var mockConnection = new Mock<IConnection>();
            var user = new User(nick, realName);
            var client = new Client(user, mockConnection.Object);

            await Task.Run(() => client.ConnectAsync("localhost", 6667));

            mockConnection.Verify(c => c.SendAsync($"NICK {nick}"), Times.Once());
            mockConnection.Verify(c => c.SendAsync($"USER {nick} 0 - :{realName}"));
        }

        [Fact]
        public void TriggersIRCMessageReceived()
        {
            var raw = ":irc.rizon.io 439 * :Please wait while we process your connection.";
            ParsedIRCMessage ircMessage = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.OnIRCMessageParsed += (c, m) => ircMessage = m;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal("irc.rizon.io", ircMessage.Prefix.From);
            Assert.Equal("439", ircMessage.Command);
            Assert.Equal("*", ircMessage.Parameters[0]);
            Assert.Equal("Please wait while we process your connection.", ircMessage.Trailing);
        }

        [Fact]
        public void TriggersOnPrivMsgReceived()
        {
            var from = "Angel";
            var to = "Wiz";
            var message = "Hello are you receiving this message ?";
            var raw = $":{from} PRIVMSG {to} :{message}";
            IRCMessageEventArgs<PrivMsgMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.PrivMsg += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(from, args.IRCMessage.From);
            Assert.Equal(from, args.IRCMessage.Prefix.From);
            Assert.Equal(to, args.IRCMessage.To);
            Assert.Equal(message, args.IRCMessage.Message);
        }

        [Fact]
        public void TriggersOnPrivMsgReceivedWithoutTrailing()
        {
            var from = "Angel";
            var to = "Wiz";
            var message = "Hello";
            var raw = $":{from} PRIVMSG {to} {message}";
            IRCMessageEventArgs<PrivMsgMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.PrivMsg += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(from, args.IRCMessage.From);
            Assert.Equal(from, args.IRCMessage.Prefix.From);
            Assert.Equal(to, args.IRCMessage.To);
            Assert.Equal(message, args.IRCMessage.Message);
        }

        [Fact]
        public void TriggersOnNoticeReceived()
        {
            var from = "irc.server.net";
            var to = "WiZ";
            var message = "Hello world";
            var raw = $":{from} NOTICE {to} :{message}";
            IRCMessageEventArgs<NoticeMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.Notice += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(from, args.IRCMessage.From);
            Assert.Equal(to, args.IRCMessage.Target);
            Assert.Equal(message, args.IRCMessage.Message);
        }

        [Fact]
        public void TriggersOnNoticeReceivedWithoutTrailing()
        {
            var from = "irc.server.net";
            var to = "WiZ";
            var message = "Hello";
            var raw = $":{from} NOTICE {to} {message}";
            IRCMessageEventArgs<NoticeMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.Notice += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(from, args.IRCMessage.From);
            Assert.Equal(to, args.IRCMessage.Target);
            Assert.Equal(message, args.IRCMessage.Message);
        }

        [Fact]
        public void TriggersOnRplWelcomeMessageReceived()
        {
            var text = "Welcome to the Internet Relay Chat Network NetIRC";
            var raw = $":irc.server.net 001 NetIRC :{text}";
            IRCMessageEventArgs<RplWelcomeMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.RplWelcome += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(text, args.IRCMessage.Text);
        }

        [Fact]
        public void TriggersOnRplYourHostMessageReceived()
        {
            var text = "Your host is irc.server.net, running version plexus-4(hybrid-8.1.20)";
            var raw = $":irc.server.net 002 NetIRC :{text}";
            IRCMessageEventArgs<RplYourHostMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.RplYourHost += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(text, args.IRCMessage.Text);
        }

        [Fact]
        public void TriggersOnRplCreatedMessageReceived()
        {
            var text = "This server was created Nov 20 2016 at 02:34:01";
            var raw = $":irc.server.net 003 NetIRC :{text}";
            IRCMessageEventArgs<RplCreatedMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.RplCreated += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(text, args.IRCMessage.Text);
        }

        [Fact]
        public void TriggersOnRplMyInfoMessageReceived()
        {
            var parameters = new[]
            {
                "NetIRC",
                "irc.server.net",
                "plexus-4(hybrid-8.1.20)",
                "CDGNRSUWagilopqrswxyz",
                "BCIMNORSabcehiklmnopqstvz",
                "Iabehkloqv"
            };
            var raw = $":irc.server.net 004 {string.Join(" ", parameters)}";
            IRCMessageEventArgs<RplMyInfoMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.RplMyInfo += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(parameters[0], args.IRCMessage.Parameters[0]);
            Assert.Equal(parameters[1], args.IRCMessage.Parameters[1]);
            Assert.Equal(parameters[2], args.IRCMessage.Parameters[2]);
            Assert.Equal(parameters[3], args.IRCMessage.Parameters[3]);
            Assert.Equal(parameters[4], args.IRCMessage.Parameters[4]);
            Assert.Equal(parameters[5], args.IRCMessage.Parameters[5]);
        }

        [Fact]
        public void TriggersOnRplISupportMessageReceived()
        {
            var text = "are supported by this server";
            var parameters = new[]
            {
                "NetIRC",
                "CALLERID",
                "CASEMAPPING=rfc1459",
                "DEAF=D",
                "NICKLEN=30",
                "MAXTARGETS=4"
            };
            var raw = $":irc.server.net 005 {string.Join(" ", parameters)} :{text}";
            IRCMessageEventArgs<RplISupportMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.RplISupport += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(parameters[0], args.IRCMessage.Parameters[0]);
            Assert.Equal(parameters[1], args.IRCMessage.Parameters[1]);
            Assert.Equal(parameters[2], args.IRCMessage.Parameters[2]);
            Assert.Equal(parameters[3], args.IRCMessage.Parameters[3]);
            Assert.Equal(parameters[4], args.IRCMessage.Parameters[4]);
            Assert.Equal(parameters[5], args.IRCMessage.Parameters[5]);
            Assert.Equal(text, args.IRCMessage.Parameters[6]);
            Assert.Equal(text, args.IRCMessage.Text);
        }

        [Fact]
        public void TriggersOnJoinReceived()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick} JOIN {channel}";
            IRCMessageEventArgs<JoinMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.Join += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(nick, args.IRCMessage.Nick);
            Assert.Equal(channel, args.IRCMessage.Channel);
        }

        [Fact]
        public void TriggersOnPartReceived()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick}!~user@x.y.z PART {channel}";
            IRCMessageEventArgs<PartMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.Part += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(nick, args.IRCMessage.Nick);
            Assert.Equal(channel, args.IRCMessage.Channel);
        }

        [Fact]
        public void RemovesUserFromChannel()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick}!~user@x.y.z PART {channel}";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            var ircChannel = client.Channels.GetChannel(channel);
            ircChannel.Users.Add(new ChannelUser(new User(nick), string.Empty));

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(0, ircChannel.Users.Count);
        }

        [Fact]
        public void UserJoiningChannel()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick} JOIN {channel}";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(channel, client.Channels[0].Name);
            Assert.Equal(nick, client.Channels[0].Users.ElementAt(0).Nick);
        }

        [Fact]
        public void TriggersOnRplNamReplyReceived()
        {
            var channel = "#NetIRC";
            var raw = $":irc.server.net 353 NetIRCConsoleClient = {channel} :NetIRCConsoleClient @Fredi_";
            IRCMessageEventArgs<RplNamReplyMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.RplNamReply += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(channel, args.IRCMessage.Channel);
            Assert.Equal(2, args.IRCMessage.Nicks.Count);
        }

        [Fact]
        public void FillChannelUsers()
        {
            var nick1 = "NetIRCConsoleClient";
            var nick2 = "Fredi_";
            var raw = $":irc.server.net 353 NetIRCConsoleClient = #NetIRC :{nick1} @{nick2}";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(2, client.Channels[0].Users.Count);
            Assert.Equal(nick1, client.Channels[0].Users.ElementAt(0).Nick);
            Assert.Equal(nick2, client.Channels[0].Users.ElementAt(1).Nick);
        }

        [Fact]
        public void TriggersQuitMessageReceived()
        {
            var nick = "WiZ";
            var message = "Out for lunch";
            var raw = $":{nick}!~host@x.y.z QUIT :{message}";
            IRCMessageEventArgs<QuitMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.Quit += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(nick, args.IRCMessage.Nick);
            Assert.Equal(message, args.IRCMessage.Message);
        }

        [Fact]
        public void UserRemovedFromChannelsOnQuit()
        {
            var nick = "WiZ";
            var message = "Out for lunch";
            var raw = $":{nick}!~host@x.y.z QUIT :{message}";
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            var ircChannel = client.Channels.GetChannel("#channel");
            ircChannel.Users.Add(new ChannelUser(new User(nick), string.Empty));

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(0, ircChannel.Users.Count);
        }

        [Fact]
        public void TriggersOnRegistrationCompletedEvent()
        {
            var raw = ":irc.server.io 001 netIRCTest :Welcome";
            var completed = false;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.RegistrationCompleted += (c, a) => completed = true;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.True(completed);
        }

        [Fact]
        public void TriggersOnNickEvent()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var raw = $":{oldNick} NICK {newNick}";
            IRCMessageEventArgs<NickMessage> args = null;
            var mockConnection = new Mock<IConnection>();
            var client = new Client(FakeUser, mockConnection.Object);

            client.EventHub.Nick += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(oldNick, args.IRCMessage.OldNick);
            Assert.Equal(newNick, args.IRCMessage.NewNick);
        }

        private void RaiseDataReceived(Mock<IConnection> mockConnection, Client client, string raw)
        {
            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));
        }
    }
}
