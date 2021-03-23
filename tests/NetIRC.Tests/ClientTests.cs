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
            ParsedIRCMessage ircMessage = null;

            client.OnIRCMessageParsed += (c, m) => ircMessage = m;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Equal("irc.rizon.io", ircMessage.Prefix.From);
            Assert.Equal("439", ircMessage.Command);
            Assert.Equal("*", ircMessage.Parameters[0]);
            Assert.Equal("Please wait while we process your connection.", ircMessage.Trailing);
        }

        [Fact]
        public void PrivMsgCreatePeer()
        {
            var raw = ":from PRIVMSG to :message";

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Single(client.Peers);
            Assert.Equal("from", client.Peers[0].Nick);
        }

        [Fact]
        public void PrivMsgCreateQuery()
        {
            var raw = ":from PRIVMSG to :message";

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Single(client.Queries);
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
            Assert.Single(messages);
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
            Assert.Single(messages);
            Assert.Equal(user, messages[0].User);
            Assert.Equal(message, messages[0].Text);
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

            Assert.Empty(ircChannel.Users);
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

            Assert.Single(ircChannel.Users);
            Assert.Equal(nick2, ircChannel.Users[0].Nick);
        }

        [Fact]
        public void UserRemovedFromChannelsOnQuit()
        {
            var nick = "WiZ";
            var raw = $":{nick}!~host@x.y.z QUIT :Out for lunch";

            var ircChannel = client.Channels.GetChannel("#channel");
            ircChannel.Users.Add(new ChannelUser(new User(nick), string.Empty));

            RaiseDataReceived(mockConnection, client, raw);

            Assert.Empty(ircChannel.Users);
        }

        [Fact]
        public void TriggersOnRegistrationCompletedEvent()
        {
            var raw = ":irc.server.io 001 netIRCTest :Welcome";
            var completed = false;

            client.RegistrationCompleted += (c, a) => completed = true;

            RaiseDataReceived(mockConnection, client, raw);

            Assert.True(completed);
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
