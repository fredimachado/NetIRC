using System;

namespace NetIRC.Desktop.ViewModel
{
    public interface ITab
    {
        string Title { get; }

        void AddMessage(string message);
    }
}
