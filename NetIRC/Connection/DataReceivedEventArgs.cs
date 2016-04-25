namespace NetIRC.Connection
{
    public class DataReceivedEventArgs
    {
        private readonly string data;

        public DataReceivedEventArgs(string data)
        {
            this.data = data;
        }

        public string Data { get { return data; } }
    }
}
