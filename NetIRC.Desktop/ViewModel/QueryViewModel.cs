using NetIRC.Messages;
using System.Threading.Tasks;

namespace NetIRC.Desktop.ViewModel
{
    public class QueryViewModel : TabViewModelBase
    {
        public override string Title => query.Nick;
        
        public string Nick => query.Nick;

        private readonly Query query;
        
        public QueryViewModel(Query query)
        {
            this.query = query;
            query.Messages.CollectionChanged += Messages_CollectionChanged;
        }

        private void Messages_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count > 0)
            {
                foreach (ChatMessage message in e.NewItems)
                {
                    AddMessage($"{message.User.Nick}: {message.Text}");
                }
            }
        }

        protected override async Task Send()
        {
            if (string.IsNullOrWhiteSpace(Input))
            {
                return;
            }

            if (Input.StartsWith("/"))
            {
                await App.Client.SendRaw(Input.Substring(1));
            }
            else
            {
                AddMessage($"{App.Nick}: {Input}");
                await App.Client.SendAsync(new PrivMsgMessage(query.Nick, Input));
            }

            Input = string.Empty;
        }
    }
}
