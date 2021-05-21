using System;
using System.Windows.Input;

namespace Audio_Sample
{
    public class Command : ICommand
    {
        private readonly Action<object> _Method;

        public event EventHandler CanExecuteChanged;
        public virtual bool CanExecute(object parameter) => true;
        public void Execute(object parameter) => _Method.Invoke(parameter);

        public Command(Action<object> method)
        {
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }

            _Method = method;
        }

        protected void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
