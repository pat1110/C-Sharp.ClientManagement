using System;
using System.ComponentModel;
using System.Windows;
using Client_Management.ViewModel;

namespace Client_Management
{
    /// <summary>
    /// Interaktionslogik für CommandSelectWindow.xaml
    /// </summary>
    public partial class CommandSelectWindow : IDisposable
    {
        CommandSelect _commandSelect = new CommandSelect();
        public CommandSelectWindow()
        {
            InitializeComponent();
            DataContext = _commandSelect;
        }


        private void ExecuteCommand_Click(object sender, RoutedEventArgs e)
        {
            if (_commandSelect.SelectedPackage != null)
            {
                MainViewModel.ResultValue = _commandSelect.SelectedPackage.Name;
                this.DialogResult = true;
                this.Close();
            }
        }

        private void CommandSelectWindow_Closing(object sender, CancelEventArgs e)
        {
        }

        public void Dispose()
        {
        }
    }
}
