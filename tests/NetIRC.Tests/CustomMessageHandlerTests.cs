using NetIRC.Messages;
using NetIRC.Messages.Handlers;
using System.Threading.Tasks;
using Xunit;

namespace NetIRC.Tests
{
    public class CustomMessageHandlerTests
    {
        private readonly MessageHandlerContainer messageHandlerRegistrar;

        public CustomMessageHandlerTests()
        {
            messageHandlerRegistrar = new MessageHandlerContainer(client: null, typeof(CustomMessageHandlerTests).Assembly);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task CustomMessageHandler_ShouldBeAbleToPreventDefaultHandlerToExecute(bool handled)
        {
            MyJoinHandler.Called = false;
            MyCustomJoinHandler.Called = false;
            MyCustomJoinHandler.HandledValue = handled;
            var parsedIRCMessage = new ParsedIRCMessage(":Wiz JOIN #channel");
            messageHandlerRegistrar.RegisterCustomMessageHandler(typeof(MyCustomJoinHandler));

            await messageHandlerRegistrar.HandleAsync(parsedIRCMessage);

            Assert.True(MyCustomJoinHandler.Called);

            // If CustomMessageHandler<>.Handled is true, default message handler should not be executed
            Assert.Equal(!handled, MyJoinHandler.Called);
        }
    }

    [Command("JOIN")]
    public class MyJoinHandler : MessageHandler<JoinMessage>
    {
        public static bool Called;
        public override Task HandleAsync(JoinMessage serverMessage, Client client)
        {
            Called = true;
            return Task.CompletedTask;
        }
    }

    [Command("JOIN")]
    public class MyCustomJoinHandler : CustomMessageHandler<JoinMessage>
    {
        public static bool Called;
        public static bool HandledValue;
        public override Task HandleAsync(JoinMessage serverMessage, Client client)
        {
            Called = true;

            Handled = HandledValue;

            return Task.CompletedTask;
        }
    }
}
