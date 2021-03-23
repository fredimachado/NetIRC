using Moq;
using NetIRC.Connection;
using NetIRC.Messages;
using NetIRC.Messages.Handlers;
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
        public async Task HandlesPingMessage()
        {
            var text = "xyz.com";
            var raw = $"PING :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<PingHandler>(result);
            var ircMessage = Assert.IsType<PingMessage>(handler.Message);
            Assert.Equal(text, ircMessage.Target);
        }

        [Fact]
        public async Task HandlesPrivMsgMessage()
        {
            var from = "Angel";
            var to = "Wiz";
            var message = "Hello are you receiving this message ?";
            var raw = $":{from} PRIVMSG {to} :{message}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<PrivMsgHandler>(result);
            var ircMessage = Assert.IsType<PrivMsgMessage>(handler.Message);
            Assert.Equal(from, ircMessage.From);
            Assert.Equal(from, ircMessage.Prefix.From);
            Assert.Equal(to, ircMessage.To);
            Assert.Equal(message, ircMessage.Message);
        }

        [Fact]
        public async Task HandlesPrivMsgMessageWithoutTrailing()
        {
            var from = "Angel";
            var to = "Wiz";
            var message = "Hello";
            var raw = $":{from} PRIVMSG {to} {message}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<PrivMsgHandler>(result);
            var ircMessage = Assert.IsType<PrivMsgMessage>(handler.Message);
            Assert.Equal(from, ircMessage.From);
            Assert.Equal(from, ircMessage.Prefix.From);
            Assert.Equal(to, ircMessage.To);
            Assert.Equal(message, ircMessage.Message);
        }

        [Fact]
        public async Task HandlesRplWelcomeMessage()
        {
            var text = "Welcome to the Internet Relay Chat Network NetIRC";
            var raw = $":irc.server.net 001 NetIRC :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<RplWelcomeHandler>(result);
            var ircMessage = Assert.IsType<RplWelcomeMessage>(handler.Message);
            Assert.Equal(text, ircMessage.Text);
        }

        [Fact(Skip = "Implement RplGreetingMessage for 001, 002, 003 and 004 numeric replies")]
        public async Task HandlesRplYourHostMessage()
        {
            var text = "Your host is irc.server.net, running version plexus-4(hybrid-8.1.20)";
            var raw = $":irc.server.net 002 NetIRC :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var ircMessage = Assert.IsType<RplWelcomeMessage>(result);
            Assert.Equal(text, ircMessage.Text);
        }

        [Fact(Skip = "Implement RplGreetingMessage for 001, 002, 003 and 004 numeric replies")]
        public async Task HandlesRplCreatedMessage()
        {
            var text = "This server was created Nov 20 2016 at 02:34:01";
            var raw = $":irc.server.net 003 NetIRC :{text}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var ircMessage = Assert.IsType<RplWelcomeMessage>(result);
            Assert.Equal(text, ircMessage.Text);
        }

        [Fact(Skip = "Implement RplGreetingMessage for 001, 002, 003 and 004 numeric replies")]
        public async Task HandlesRplMyInfoMessage()
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
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var ircMessage = Assert.IsType<RplWelcomeMessage>(result);
            //Assert.Equal(parameters[0], ircMessage.Parameters[0]);
            //Assert.Equal(parameters[1], ircMessage.Parameters[1]);
            //Assert.Equal(parameters[2], ircMessage.Parameters[2]);
            //Assert.Equal(parameters[3], ircMessage.Parameters[3]);
            //Assert.Equal(parameters[4], ircMessage.Parameters[4]);
            //Assert.Equal(parameters[5], ircMessage.Parameters[5]);
        }

        [Fact]
        public async Task HandlesJoinMessage()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick} JOIN {channel}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<JoinHandler>(result);
            var ircMessage = Assert.IsType<JoinMessage>(handler.Message);
            Assert.Equal(nick, ircMessage.Nick);
            Assert.Equal(channel, ircMessage.Channel);
        }

        [Fact]
        public async Task HandlesPartMessage()
        {
            var nick = "Wiz";
            var channel = "#channel";
            var raw = $":{nick}!~user@x.y.z PART {channel}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<PartHandler>(result);
            var ircMessage = Assert.IsType<PartMessage>(handler.Message);
            Assert.Equal(nick, ircMessage.Nick);
            Assert.Equal(channel, ircMessage.Channel);
        }

        [Fact]
        public async Task HandlesRplNamReplyMessage()
        {
            var channel = "#NetIRC";
            var raw = $":irc.server.net 353 NetIRCConsoleClient = {channel} :NetIRCConsoleClient";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<RplNamReplyHandler>(result);
            var ircMessage = Assert.IsType<RplNamReplyMessage>(handler.Message);
            Assert.Equal(channel, ircMessage.Channel);
            Assert.Single(ircMessage.Nicks);
        }

        [Fact]
        public async Task HandlesRplNamReplyMessageWithMultiplePeople()
        {
            var channel = "#NetIRC";
            var nicks = new[] { "NetIRCConsoleClient", "@Fredi_", "Wiz" };
            var raw = $":irc.server.net 353 NetIRCConsoleClient = {channel} :{string.Join(' ', nicks)}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<RplNamReplyHandler>(result);
            var ircMessage = Assert.IsType<RplNamReplyMessage>(handler.Message);
            Assert.Equal(channel, ircMessage.Channel);
            Assert.Equal(3, ircMessage.Nicks.Count);
            
        }

        [Fact]
        public async Task HandlesQuitMessage()
        {
            var nick = "WiZ";
            var message = "Out for lunch";
            var raw = $":{nick}!~host@x.y.z QUIT :{message}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<QuitHandler>(result);
            var ircMessage = Assert.IsType<QuitMessage>(handler.Message);
            Assert.Equal(nick, ircMessage.Nick);
            Assert.Equal(message, ircMessage.Message);
        }

        [Fact]
        public async Task HandlesNickMessage()
        {
            var oldNick = "WiZ";
            var newNick = "Kilroy";
            var raw = $":{oldNick} NICK {newNick}";
            var parsedIRCMessage = new ParsedIRCMessage(raw);

            var result = await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            var handler = Assert.IsType<NickHandler>(result);
            var ircMessage = Assert.IsType<NickMessage>(handler.Message);
            Assert.Equal(oldNick, ircMessage.OldNick);
            Assert.Equal(newNick, ircMessage.NewNick);
        }
    }
}
