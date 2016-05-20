namespace NetIRC.Connection
{
    public class DataReceivedEventArgs
    {
        public string Data { get; }

        public DataReceivedEventArgs(string data)
        {
            Data = data;
        }
    }
}
