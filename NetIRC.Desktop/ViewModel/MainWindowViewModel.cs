using System.Collections.ObjectModel;

namespace NetIRC.Desktop.ViewModel
{
    public class MainWindowViewModel
    {
        public ObservableCollection<ITab> Tabs { get; set; }

        public MainWindowViewModel()
        {
            Tabs = new ObservableCollection<ITab>();
        }
    }

    public interface ITab
    {
        string Title { get; }

        void Message(string message);
    }
}
