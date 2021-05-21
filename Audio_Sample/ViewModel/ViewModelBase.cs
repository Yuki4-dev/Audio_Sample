using System;
using System.ComponentModel;

namespace Audio_Sample
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<ShowMessageBoxEventArgs> ShowMessageBox;
        public event EventHandler<ShowCommonDialogEventArgs> ShowCommonDialog;

        private bool _IsBusy = false;
        public bool IsBusy
        {
            get => _IsBusy;
            set => OnPropertyChanged(ref _IsBusy, value, nameof(IsBusy), () => OnPropertyChanged(nameof(IsNotBusy)));
        }

        public bool IsNotBusy
        {
            get => !IsBusy;
        }

        public virtual void PostPrccess()
        {
        }

        protected void OnPropertyChanged<T>(ref T oldValue, T newValue, string name, Action postCall = null)
        {
            if ((oldValue == null && newValue != null)
                || (oldValue != null && !oldValue.Equals(newValue)))
            {
                oldValue = newValue;
                OnPropertyChanged(name, postCall);
            }
        }

        protected void OnPropertyChanged(string name, Action postCall = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            postCall?.Invoke();
        }

        protected void OnShowMessageBox(string msg)
        {
            ShowMessageBox?.Invoke(this, new ShowMessageBoxEventArgs(msg));
        }

        protected void OnShowMessageBox(ShowMessageBoxEventArgs args)
        {
            ShowMessageBox?.Invoke(this, args);
        }

        protected void OnShowCommonDialog(ShowCommonDialogEventArgs args)
        {
            ShowCommonDialog?.Invoke(this, args);
        }
    }
}
