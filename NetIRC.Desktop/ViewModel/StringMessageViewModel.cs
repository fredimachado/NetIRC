namespace NetIRC.Desktop.ViewModel
{
    public class StringMessageViewModel : IMessage
    {
        public string Message { get; }

        public StringMessageViewModel(string message)
        {
            Message = message;
        }
    }
}
