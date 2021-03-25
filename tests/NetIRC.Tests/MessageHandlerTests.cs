using Moq;
using NetIRC.Connection;
using NetIRC.Messages;
using System.Threading.Tasks;
using Xunit;

namespace NetIRC.Tests
{
    public class MessageHandlerTests
    {
        private static User FakeUser = new User("test", "test");
        private readonly Mock<IConnection> mockConnection;
        private readonly Client client;
        private readonly MessageHandlerContainer messageHandlerRegistrar;

        public MessageHandlerTests()
        {
            mockConnection = new Mock<IConnection>();
            client = new Client(FakeUser, mockConnection.Object);
            messageHandlerRegistrar = new MessageHandlerContainer(client);
        }

        [Fact]
        public async Task PingMessageHandler_ShouldSendPongMessageToServer()
        {
            var text = "xyz.com";
            var raw = $"PING :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            mockConnection.Verify(c => c.SendAsync(new PongMessage(text).ToString()), Times.Once);
        }

        [Fact]
        public async Task PrivMsgMessageHandler_ShouldAddQueryMessage()
        {
            var from = "Angel";
            var to = "Wiz";
            var text = "Hello are you receiving this message ?";
            var raw = $":{from} PRIVMSG {to} :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var user = client.Peers.GetUser(from);
            var query = client.Queries.GetQuery(user);

            Assert.Empty(query.Messages);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Single(query.Messages);
            Assert.Equal(text, query.Messages[0].Text);
            Assert.Equal(user, query.Messages[0].User);
        }

        [Fact]
        public async Task PrivMsgMessageHandler_ShouldAddChannelMessage()
        {
            var from = "Angel";
            var channelName = "#channel";
            var text = "Hello are you receiving this message ?";
            var raw = $":{from} PRIVMSG {channelName} :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var user = client.Peers.GetUser(from);
            var channel = client.Channels.GetChannel(channelName);

            Assert.Empty(channel.Messages);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Single(channel.Messages);
            Assert.Equal(text, channel.Messages[0].Text);
            Assert.Equal(user, channel.Messages[0].User);
        }

        [Fact]
        public async Task RplWelcomeMessageHandler_ShouldCallOnRegistrationCompleted()
        {
            var raw = ":irc.server.net 001 NetIRC :Welcome";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var called = false;
            client.RegistrationCompleted += (s, e) => called = true;

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.True(called);
        }

        [Fact]
        public async Task JoinMessageHandler_ShouldAddUserToChannel()
        {
            var nick = "Wiz";
            var channelName = "#channel";
            var raw = $":{nick} JOIN {channelName}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var channel = client.Channels.GetChannel(channelName);

            Assert.Null(channel.GetUser(nick));

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var user = channel.GetUser(nick);
            Assert.NotNull(user);
            Assert.Equal(nick, user.Nick);
        }

        [Fact]
        public async Task PartMessageHandler_ShouldRemoveUserFromChannel()
        {
            var nick = "Wiz";
            var channelName = "#channel";
            var raw = $":{nick}!~user@x.y.z PART {channelName}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var user = new User(nick);
            var channel = client.Channels.GetChannel(channelName);

            channel.AddUser(user);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Null(channel.GetUser(nick));
        }

        [Fact]
        public async Task RplNamReplyMessageHandler_ShouldAddUserToChannel()
        {
            var channelName = "#NetIRC";
            var nick = "NetIRCConsoleClient";
            var raw = $":irc.server.net 353 NetIRCConsoleClient = {channelName} :{nick}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var channel = client.Channels.GetChannel(channelName);

            Assert.Empty(channel.Users);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Single(channel.Users);
            Assert.NotNull(channel.GetUser(nick));
        }

        [Fact]
        public async Task RplNamReplyMessageHandlerWithMultiplePeople_ShouldAddUsersToChannel()
        {
            var channelName = "#NetIRC";
            var nicks = new[] { "NetIRCConsoleClient", "Fredi_", "Wiz" };
            var raw = $":irc.server.net 353 NetIRCConsoleClient = {channelName} :{string.Join(' ', nicks)}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var channel = client.Channels.GetChannel(channelName);

            Assert.Empty(channel.Users);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Equal(3, channel.Users.Count);
            foreach (var nick in nicks)
            {
                Assert.NotNull(channel.GetUser(nick));
            }
        }

        [Fact]
        public async Task QuitMessageHandler_ShouldRemoveFromChannels()
        {
            var nick = "WiZ";
            var raw = $":{nick}!~host@x.y.z QUIT :Out for lunch";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var user = new User(nick);
            var channel1 = client.Channels.GetChannel("#channel1");
            var channel2 = client.Channels.GetChannel("#channel2");

            channel1.AddUser(user);
            channel2.AddUser(user);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Null(channel1.GetUser(nick));
            Assert.Null(channel2.GetUser(nick));
        }

        [Fact]
        public async Task NickMessageHandler_ShouldChangeNick()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var raw = $":{oldNick} NICK {newNick}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var user = client.Peers.GetUser(oldNick);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Same(user, client.Peers.GetUser(newNick));
        }

        [Fact]
        public async Task TopicMessageHandler_ShouldSetTopic()
        {
            var channel = "#NetIRC";
            var topic = "NetIRC is nice!";
            var raw = $":irc.server.net TOPIC {channel} :{topic}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Equal(topic, client.Channels.GetChannel(channel).Topic);
        }

        [Fact]
        public async Task KickMessageHandler_ShouldRemoveFromChannel()
        {
            var channelName = "#netirctest";
            var nick = "NetIRCConsoleClient";
            var raw = $":Fredi!~Fredi@XYZ.IP KICK {channelName} {nick} :I love you!";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var channel = client.Channels.GetChannel(channelName);
            channel.AddUser(new User(nick));

            Assert.NotNull(channel.GetUser(nick));

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Null(channel.GetUser(nick));
        }

        [Fact]
        public async Task KickMessageHandler_ShouldRemoveUserFromChannel()
        {
            var channelName = "#netirctest";
            var raw = $":Fredi!~Fredi@XYZ.IP KICK {channelName} {client.User.Nick} :Bye";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            client.Channels.GetChannel(channelName);

            Assert.Single(client.Channels);

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.Empty(client.Channels);
        }
    }
}
