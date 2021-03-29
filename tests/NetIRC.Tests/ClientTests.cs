using Moq;
using NetIRC.Connection;
using NetIRC.Ctcp;
using NetIRC.Messages;
using System;
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

            client.RawDataReceived += (c, d) => rawReceived = d;

            RaiseDataReceived(raw);

            Assert.Equal(raw, rawReceived);
        }

        [Fact]
        public void EmptyDataDoesntTriggerRawDataReceived()
        {
            var triggered = false;

            client.RawDataReceived += (c, d) => { triggered = true; };

            RaiseDataReceived("\r\n");

            Assert.False(triggered);
        }

        [Fact]
        public void RespondsToPingMessage()
        {
            var data = "xyz.com";
            var raw = $"PING :{data}";

            RaiseDataReceived(raw);

            mockConnection.Verify(c => c.SendAsync($"PONG {data}"), Times.Once());
        }

        [Fact]
        public async Task SendsNickAndUserWhenConnected()
        {
            var nick = "guest";
            var realName = "Ronnie Reagan";
            var user = new User(nick, realName);
            var client = new Client(user, mockConnection.Object);

            await Task.Run(() => client.ConnectAsync());

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

            await Task.Run(() => client.ConnectAsync());

            mockConnection.Verify(c => c.SendAsync($"PASS {password}"), Times.Once());
            mockConnection.Verify(c => c.SendAsync($"NICK {nick}"), Times.Once());
            mockConnection.Verify(c => c.SendAsync($"USER {nick} 0 - :{realName}"), Times.Once());
        }

        [Fact]
        public void TriggersIRCMessageReceived()
        {
            var raw = ":irc.rizon.io 439 * :Please wait while we process your connection.";
            ParsedIRCMessage ircMessage = null;

            client.IRCMessageParsed += (c, m) => ircMessage = m;

            RaiseDataReceived(raw);

            Assert.Equal("irc.rizon.io", ircMessage.Prefix.From);
            Assert.Equal("439", ircMessage.Command);
            Assert.Equal("*", ircMessage.Parameters[0]);
            Assert.Equal("Please wait while we process your connection.", ircMessage.Trailing);
        }

        [Fact]
        public void PrivMsgCreatePeer()
        {
            var raw = ":from PRIVMSG to :message";

            RaiseDataReceived(raw);

            Assert.Single(client.Peers);
            Assert.Equal("from", client.Peers[0].Nick);
        }

        [Fact]
        public void PrivMsgCreateQuery()
        {
            var raw = ":from PRIVMSG to :message";

            RaiseDataReceived(raw);

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

            RaiseDataReceived(raw);

            var messages = client.Queries[0].Messages;
            Assert.Single(messages);
            Assert.Equal(user, messages[0].User);
            Assert.Equal(message, messages[0].Text);
            Assert.Equal(DateTime.Now, messages[0].Date, TimeSpan.FromSeconds(1));
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

            RaiseDataReceived(raw);

            var messages = channel.Messages;
            Assert.Single(messages);
            Assert.Equal(user, messages[0].User);
            Assert.Equal(channelName, messages[0].Channel.Name);
            Assert.Equal(message, messages[0].Text);
            Assert.Equal(DateTime.Now, messages[0].Date, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void TriggersCtcpReceivedReceived()
        {
            var from = "Angel";
            var to = "WiZ";
            var ctcpCommand = "ACTION";
            var ctcpMessage = "likes NetIRC";
            var message = $"{CtcpCommands.CtcpDelimiter}{ctcpCommand} {ctcpMessage}{CtcpCommands.CtcpDelimiter}";
            var raw = $":{from} PRIVMSG {to} :{message}";
            CtcpEventArgs ctcpEventArgs = null;

            client.CtcpReceived += (c, e) => ctcpEventArgs = e;

            RaiseDataReceived(raw);

            Assert.Equal(from, ctcpEventArgs.From);
            Assert.Equal(from, ctcpEventArgs.Prefix.From);
            Assert.Equal(to, ctcpEventArgs.To);
            Assert.Equal(message, ctcpEventArgs.Message);
            Assert.Equal(ctcpCommand, ctcpEventArgs.CtcpCommand);
            Assert.Equal(ctcpMessage, ctcpEventArgs.CtcpMessage);
        }

        [Fact]
        public void RemovesUserFromChannel()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick}!~user@x.y.z PART {channel}";

            var ircChannel = client.Channels.GetChannel(channel);
            ircChannel.Users.Add(new ChannelUser(new User(nick), string.Empty));

            RaiseDataReceived(raw);

            Assert.Empty(ircChannel.Users);
        }

        [Fact]
        public void UserJoiningChannel()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick} JOIN {channel}";

            RaiseDataReceived(raw);

            Assert.Equal(channel, client.Channels[0].Name);
            Assert.Equal(nick, client.Channels[0].Users.ElementAt(0).Nick);
        }

        [Fact]
        public void FillChannelUsers()
        {
            var nick1 = "NetIRCConsoleClient";
            var nick2 = "Fredi_";
            var raw = $":irc.server.net 353 NetIRCConsoleClient = #NetIRC :{nick1} @{nick2}";

            RaiseDataReceived(raw);

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

            RaiseDataReceived($":{nick} PART {channel}");

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

            RaiseDataReceived(raw);

            Assert.Empty(ircChannel.Users);
        }

        [Fact]
        public void TriggersOnRegistrationCompletedEvent()
        {
            var raw = ":irc.server.io 001 netIRCTest :Welcome";
            var completed = false;

            client.RegistrationCompleted += (c, a) => completed = true;

            RaiseDataReceived(raw);

            Assert.True(completed);
        }

        [Fact]
        public void TriggeringOnRegistrationCompletedEventWithNoHandler_ShouldWork()
        {
            var raw = ":irc.server.io 001 netIRCTest :Welcome";

            RaiseDataReceived(raw);
        }

        [Fact]
        public void NickChangesUserNick()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var raw = $":{oldNick} NICK {newNick}";

            client.Peers.Add(new User(oldNick));

            RaiseDataReceived(raw);

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

            RaiseDataReceived(raw);

            Assert.Equal("Nick", propertyName);
        }

        [Fact]
        public void CustomMessageHandlerRegistration_ShouldWork()
        {
            NoticeHandler.Called = false;
            client.RegisterCustomMessageHandler<NoticeHandler>();

            RaiseDataReceived(":irc.server.net NOTICE AUTH :*** Looking up your hostname...");

            Assert.True(NoticeHandler.Called);
        }

        [Fact]
        public void CustomMessageHandlerRegistrationWithAttribute_ShouldWork()
        {
            EndOfMotdHandler.Called = false;
            client.RegisterCustomMessageHandler<EndOfMotdHandler>();

            RaiseDataReceived(":irc.server.net 376 netIRCTest :End of /MOTD command.");

            Assert.True(EndOfMotdHandler.Called);
        }

        [Fact]
        public void CustomMessageHandlerAssemblyRegistration_ShouldWork()
        {
            NoticeHandler.Called = false;
            EndOfMotdHandler.Called = false;
            client.RegisterCustomMessageHandlers(typeof(ClientTests).Assembly);

            RaiseDataReceived(":irc.server.net NOTICE AUTH :*** Looking up your hostname...");
            RaiseDataReceived(":irc.server.net 376 netIRCTest :End of /MOTD command.");

            Assert.True(NoticeHandler.Called);
            Assert.True(EndOfMotdHandler.Called);
        }

        private void RaiseDataReceived(string raw)
        {
            mockConnection.Raise(c => c.DataReceived += null, client, new DataReceivedEventArgs(raw));
        }
    }

    public class NoticeHandler : CustomMessageHandler<NoticeMessage>
    {
        public static bool Called;
        public override Task HandleAsync(NoticeMessage serverMessage, Client client)
        {
            Called = true;
            return Task.CompletedTask;
        }
    }

    [Command("376")]
    public class EndOfMotdHandler : CustomMessageHandler<NoticeMessage>
    {
        public static bool Called;
        public override Task HandleAsync(NoticeMessage serverMessage, Client client)
        {
            Called = true;
            return Task.CompletedTask;
        }
    }
}
