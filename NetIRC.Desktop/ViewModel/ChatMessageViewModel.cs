namespace NetIRC.Desktop.ViewModel
{
    public class ChatMessageViewModel : IMessage
    {
        public ChatMessage Message { get; }

        public string Nick => Message.Nick;
        public string Text => Message.Text;

        public ChatMessageViewModel(ChatMessage message)
        {
            this.Message = message;
        }
    }
}
