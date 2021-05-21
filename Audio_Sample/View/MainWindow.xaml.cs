using Microsoft.Win32;
using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace Audio_Sample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ViewModelBase vm)
            {
                vm.ShowMessageBox += ShowMessageBox;
                vm.ShowCommonDialog += ShowCommonDialog;
                Closing += (s, e) => vm.PostPrccess();
            }
        }

        private void ShowCommonDialog(object sender, ShowCommonDialogEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => ShowCommonDialog(sender, e));
                return;
            }

            var dialog = (CommonDialog)Activator.CreateInstance(e.DialogType);
            e.PreparationDialog?.Invoke(dialog);

            var result = e.IsModal ? dialog.ShowDialog(this) : dialog.ShowDialog();
            if (result == true)
            {
                e.CallBack?.Invoke(dialog);
            }
            else
            {
                e.IsCancel = true;
            }
        }

        private void ShowMessageBox(object sender, ShowMessageBoxEventArgs e)
        {
            e.Result = MessageBox.Show(e.Message, Name, e.Button);
        }

        private void TextBlock_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !new Regex("[0-9]").IsMatch(e.Text);
        }
    }
}
