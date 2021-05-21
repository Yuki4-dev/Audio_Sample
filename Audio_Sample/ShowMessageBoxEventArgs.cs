using System;
using System.Windows;

namespace Audio_Sample
{
    public class ShowMessageBoxEventArgs : EventArgs
    {
        public MessageBoxButton Button { get; set; } = MessageBoxButton.OK;
        public MessageBoxResult Result { get; set; }
        public string Message { get; }

        public ShowMessageBoxEventArgs(string msg)
        {
            Message = msg;
        }
    }
}
