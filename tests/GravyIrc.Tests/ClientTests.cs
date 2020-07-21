using Moq;
using GravyIrc.Connection;
using GravyIrc.Messages;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace GravyIrc.Tests
{
    public class ClientTests
    {
        private static User FakeUser = new User("test", "test");
        private readonly Mock<IConnection> mockConnection;
        private readonly Client client;

        public ClientTests()
        {
            mockConnection = new Mock<IConnection>();
            client = new Client(FakeUser, mockConnection.Object);
        }

        [Fact]
        public async Task SendRawShouldCallConnectionSendAsync()
        {
            var data = "data";

            await client.SendRaw(data);

            mockConnection.Verify(c => c.SendAsync(data), Times.Once);
        }

        [Fact]
        public void DisposeShouldCallConnectionDispose()
        {
            client.Dispose();

            mockConnection.Verify(c => c.Dispose(), Times.Once);
        }

        [Fact]
        public void TriggersRawDataReceived()
        {
            var raw = "PING xyz.com";
            var rawReceived = string.Empty;

            client.OnRawDataReceived += (c, d) => rawReceived = d;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(raw, rawReceived);
        }

        [Fact]
        public void EmptyDataDoesntTriggerRawDataReceived()
        {
            var triggered = false;

            client.OnRawDataReceived += (c, d) => { triggered = true; };

            RaiseDataReceived(mockConnection, client, "\r\n");

            Assert.False(triggered);
        }

        [Fact]
        public void RespondsToPingMessage()
        {
            var data = "xyz.com";
            var raw = $"PING :{data}";

            RaiseDataReceived(mockConnection, client, raw);

            mockConnection.Verify(c => c.SendAsync($"PONG {data}"), Times.Once());
        }

        [Fact]
        public void PingWithoutHandlerShouldWork()
        {
            var raw = "PING :xyz.com";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnPingEvent()
        {
            var raw = "PING :xyz.com";
            IrcMessageEventArgs<PingMessage> args = null;

            client.EventHub.Ping += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal("xyz.com", args.IrcMessage.Target);
        }

        [Fact]
        public async Task SendsNickAndUserWhenConnected()
        {
            var nick = "guest";
            var realName = "Ronnie Reagan";
            var user = new User(nick, realName);
            var client = new Client(user, mockConnection.Object);

            await Task.Run(() => client.ConnectAsync("localhost", 6667));

            mockConnection.Verify(c => c.SendAsync($"NICK {nick}"), Times.Once());
            mockConnection.Verify(c => c.SendAsync($"USER {nick} 0 - :{realName}"), Times.Once());
        }

        [Fact]
        public async Task SendsPassNickAndUserWhenConnected()
        {
            var password = "xyz";
            var nick = "guest";
            var realName = "Ronnie Reagan";
            var user = new User(nick, realName);
            var client = new Client(user, password, mockConnection.Object);

            await Task.Run(() => client.ConnectAsync("localhost", 6667));

            mockConnection.Verify(c => c.SendAsync($"PASS {password}"), Times.Once());
            mockConnection.Verify(c => c.SendAsync($"NICK {nick}"), Times.Once());
            mockConnection.Verify(c => c.SendAsync($"USER {nick} 0 - :{realName}"), Times.Once());
        }

        [Fact]
        public void TriggersIRCMessageReceived()
        {
            var raw = ":irc.rizon.io 439 * :Please wait while we process your connection.";
            ParsedIrcMessage ircMessage = null;

            client.OnIrcMessageParsed += (c, m) => ircMessage = m;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal("irc.rizon.io", ircMessage.Prefix.From);
            Assert.Equal("439", ircMessage.Command);
            Assert.Equal("*", ircMessage.Parameters[0]);
            Assert.Equal("Please wait while we process your connection.", ircMessage.Trailing);
        }

        [Fact]
        public void PrivMsgWithoutHandlerShouldWork()
        {
            var raw = ":from PRIVMSG to :message";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnPrivMsgReceived()
        {
            var from = "Angel";
            var to = "Wiz";
            var message = "Hello are you receiving this message ?";
            var raw = $":{from} PRIVMSG {to} :{message}";
            IrcMessageEventArgs<PrivMsgMessage> args = null;

            client.EventHub.PrivMsg += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(from, args.IrcMessage.From);
            Assert.Equal(from, args.IrcMessage.Prefix.From);
            Assert.Equal(to, args.IrcMessage.To);
            Assert.Equal(message, args.IrcMessage.Message);
        }

        [Fact]
        public void TriggersOnPrivMsgReceivedWithoutTrailing()
        {
            var from = "Angel";
            var to = "Wiz";
            var message = "Hello";
            var raw = $":{from} PRIVMSG {to} {message}";
            IrcMessageEventArgs<PrivMsgMessage> args = null;

            client.EventHub.PrivMsg += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(from, args.IrcMessage.From);
            Assert.Equal(from, args.IrcMessage.Prefix.From);
            Assert.Equal(to, args.IrcMessage.To);
            Assert.Equal(message, args.IrcMessage.Message);
        }

        [Fact]
        public void PrivMsgCreatePeer()
        {
            var raw = ":from PRIVMSG to :message";

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(1, client.Peers.Count);
            Assert.Equal("from", client.Peers[0].Nick);
        }

        [Fact]
        public void PrivMsgCreateQuery()
        {
            var raw = ":from PRIVMSG to :message";

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(1, client.Queries.Count);
            Assert.Equal("from", client.Queries[0].Nick);
            Assert.Equal(client.Peers[0], client.Queries[0].User);
        }

        [Fact]
        public void PrivMsgCreateQueryChatMessage()
        {
            var from = "WiZ";
            var message = "hi there!";
            var raw = $":{from} PRIVMSG to :{message}";
            var user = client.Peers.GetUser(from);

            RaiseDataReceived(mockConnection, client, raw);

            var messages = client.Queries[0].Messages;
            Assert.Equal(1, messages.Count);
            Assert.Equal(user, messages[0].User);
            Assert.Equal(message, messages[0].Text);
        }

        [Fact]
        public void PrivMsgCreateChannelChatMessage()
        {
            var from = "WiZ";
            var message = "hi there!";
            var channelName = "#channel";
            var raw = $":{from} PRIVMSG {channelName} :{message}";
            var user = client.Peers.GetUser(from);
            var channel = client.Channels.GetChannel(channelName);

            RaiseDataReceived(mockConnection, client, raw);

            var messages = channel.Messages;
            Assert.Equal(1, messages.Count);
            Assert.Equal(user, messages[0].User);
            Assert.Equal(message, messages[0].Text);
        }

        [Fact]
        public void NoticeWithoutHandlerShouldWork()
        {
            var raw = ":from NOTICE to :message";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnNoticeReceived()
        {
            var from = "irc.server.net";
            var to = "WiZ";
            var message = "Hello world";
            var raw = $":{from} NOTICE {to} :{message}";
            IrcMessageEventArgs<NoticeMessage> args = null;

            client.EventHub.Notice += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(from, args.IrcMessage.From);
            Assert.Equal(to, args.IrcMessage.Target);
            Assert.Equal(message, args.IrcMessage.Message);
        }

        [Fact]
        public void TriggersOnNoticeReceivedWithoutTrailing()
        {
            var from = "irc.server.net";
            var to = "WiZ";
            var message = "Hello";
            var raw = $":{from} NOTICE {to} {message}";
            IrcMessageEventArgs<NoticeMessage> args = null;

            client.EventHub.Notice += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(from, args.IrcMessage.From);
            Assert.Equal(to, args.IrcMessage.Target);
            Assert.Equal(message, args.IrcMessage.Message);
        }

        [Fact]
        public void TriggersOnRplWelcomeMessageReceived()
        {
            var text = "Welcome to the Internet Relay Chat Network NetIRC";
            var raw = $":irc.server.net 001 NetIRC :{text}";
            IrcMessageEventArgs<RplWelcomeMessage> args = null;

            client.EventHub.RplWelcome += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(text, args.IrcMessage.Text);
        }

        [Fact]
        public void RplYourHostWithoutHandlerShouldWork()
        {
            var raw = ":irc.server.net 002 NetIRC :text";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnRplYourHostMessageReceived()
        {
            var text = "Your host is irc.server.net, running version plexus-4(hybrid-8.1.20)";
            var raw = $":irc.server.net 002 NetIRC :{text}";
            IrcMessageEventArgs<RplYourHostMessage> args = null;

            client.EventHub.RplYourHost += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(text, args.IrcMessage.Text);
        }

        [Fact]
        public void RplCreatedWithoutHandlerShouldWork()
        {
            var raw = ":irc.server.net 003 NetIRC :text";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnRplCreatedMessageReceived()
        {
            var text = "This server was created Nov 20 2016 at 02:34:01";
            var raw = $":irc.server.net 003 NetIRC :{text}";
            IrcMessageEventArgs<RplCreatedMessage> args = null;

            client.EventHub.RplCreated += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(text, args.IrcMessage.Text);
        }

        [Fact]
        public void RplMyInfoWithoutHandlerShouldWork()
        {
            var raw = ":irc.server.net 004 NetIRC irc.server.net xyz";
            RaiseDataReceived(mockConnection, client, raw);
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
            IrcMessageEventArgs<RplMyInfoMessage> args = null;

            client.EventHub.RplMyInfo += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(parameters[0], args.IrcMessage.Parameters[0]);
            Assert.Equal(parameters[1], args.IrcMessage.Parameters[1]);
            Assert.Equal(parameters[2], args.IrcMessage.Parameters[2]);
            Assert.Equal(parameters[3], args.IrcMessage.Parameters[3]);
            Assert.Equal(parameters[4], args.IrcMessage.Parameters[4]);
            Assert.Equal(parameters[5], args.IrcMessage.Parameters[5]);
        }

        [Fact]
        public void RplISupportWithoutHandlerShouldWork()
        {
            var raw = ":irc.server.net 005 NetIRC CALLERID";
            RaiseDataReceived(mockConnection, client, raw);
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
            IrcMessageEventArgs<RplISupportMessage> args = null;

            client.EventHub.RplISupport += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(parameters[0], args.IrcMessage.Parameters[0]);
            Assert.Equal(parameters[1], args.IrcMessage.Parameters[1]);
            Assert.Equal(parameters[2], args.IrcMessage.Parameters[2]);
            Assert.Equal(parameters[3], args.IrcMessage.Parameters[3]);
            Assert.Equal(parameters[4], args.IrcMessage.Parameters[4]);
            Assert.Equal(parameters[5], args.IrcMessage.Parameters[5]);
            Assert.Equal(text, args.IrcMessage.Parameters[6]);
            Assert.Equal(text, args.IrcMessage.Text);
        }

        [Fact]
        public void JoinWithoutHandlerShouldWork()
        {
            var raw = ":Nick JOIN #channel";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnJoinReceived()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick} JOIN {channel}";
            IrcMessageEventArgs<JoinMessage> args = null;

            client.EventHub.Join += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(nick, args.IrcMessage.Nick);
            Assert.Equal(channel, args.IrcMessage.Channel);
        }

        [Fact]
        public void PartWithoutHandlerShouldWork()
        {
            var raw = ":Nick PART #channel";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnPartReceived()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick}!~user@x.y.z PART {channel}";
            IrcMessageEventArgs<PartMessage> args = null;

            client.EventHub.Part += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(nick, args.IrcMessage.Nick);
            Assert.Equal(channel, args.IrcMessage.Channel);
        }

        [Fact]
        public void RemovesUserFromChannel()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick}!~user@x.y.z PART {channel}";

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

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(channel, client.Channels[0].Name);
            Assert.Equal(nick, client.Channels[0].Users.ElementAt(0).Nick);
        }

        [Fact]
        public void RplNamReplyWithoutHandlerShouldWork()
        {
            var raw = ":irc.server.net 353 NetIRCConsoleClient = #channel :NetIRCConsoleClient";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnRplNamReplyReceived()
        {
            var channel = "#NetIRC";
            var raw = $":irc.server.net 353 NetIRCConsoleClient = {channel} :NetIRCConsoleClient @Fredi_";
            IrcMessageEventArgs<RplNamReplyMessage> args = null;

            client.EventHub.RplNamReply += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(channel, args.IrcMessage.Channel);
            Assert.Equal(2, args.IrcMessage.Nicks.Count);
        }

        [Fact]
        public void FillChannelUsers()
        {
            var nick1 = "NetIRCConsoleClient";
            var nick2 = "Fredi_";
            var raw = $":irc.server.net 353 NetIRCConsoleClient = #NetIRC :{nick1} @{nick2}";

            RaiseDataReceived(mockConnection, client, raw);

            var channel = client.Channels[0];
            Assert.Equal(2, channel.Users.Count);
            Assert.Equal(nick1, channel.Users[0].Nick);
            Assert.Equal(nick2, channel.Users[1].Nick);
            Assert.Equal("@", channel.Users[1].Status);
            Assert.Equal($"@{nick2}", channel.Users[1].ToString());
        }

        [Fact]
        public void RemovesUserFromChannelOnPart()
        {
            var nick = "Fredi_";
            var nick2 = "WiZ";
            var channel = "#NetIRC";

            var ircChannel = client.Channels.GetChannel(channel);
            ircChannel.Users.Add(new ChannelUser(new User(nick), string.Empty));
            ircChannel.Users.Add(new ChannelUser(new User(nick2), string.Empty));

            RaiseDataReceived(mockConnection, client, $":{nick} PART {channel}");

            Assert.Equal(1, ircChannel.Users.Count);
            Assert.Equal(nick2, ircChannel.Users[0].Nick);
        }

        [Fact]
        public void QuitWithoutHandlerShouldWork()
        {
            var raw = ":Nick QUIT :message";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersQuitMessageReceived()
        {
            var nick = "WiZ";
            var message = "Out for lunch";
            var raw = $":{nick}!~host@x.y.z QUIT :{message}";
            IrcMessageEventArgs<QuitMessage> args = null;

            client.EventHub.Quit += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(nick, args.IrcMessage.Nick);
            Assert.Equal(message, args.IrcMessage.Message);
        }

        [Fact]
        public void UserRemovedFromChannelsOnQuit()
        {
            var nick = "WiZ";
            var raw = $":{nick}!~host@x.y.z QUIT :Out for lunch";

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

            client.EventHub.RegistrationCompleted += (c, a) => completed = true;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.True(completed);
        }

        [Fact]
        public void NickWithoutHandlerShouldWork()
        {
            var raw = ":OldNick NICK NewNick";
            RaiseDataReceived(mockConnection, client, raw);
        }

        [Fact]
        public void TriggersOnNickEvent()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var raw = $":{oldNick} NICK {newNick}";
            IrcMessageEventArgs<NickMessage> args = null;

            client.EventHub.Nick += (c, a) => args = a;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(oldNick, args.IrcMessage.OldNick);
            Assert.Equal(newNick, args.IrcMessage.NewNick);
        }

        [Fact]
        public void NickChangesUserNick()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var raw = $":{oldNick} NICK {newNick}";

            client.Peers.Add(new User(oldNick));

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal(newNick, client.Peers[0].Nick);
        }

        [Fact]
        public void NickTriggersOnPropertyChangedOnUser()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var raw = $":{oldNick} NICK {newNick}";
            var propertyName = string.Empty;

            var user = client.Peers.GetUser(oldNick);
            user.PropertyChanged += (s, e) => propertyName = e.PropertyName;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal("Nick", propertyName);
        }

        private void RaiseDataReceived(Mock<IConnection> mockConnection, Client client, string raw)
        {
            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));
        }
    }
}
