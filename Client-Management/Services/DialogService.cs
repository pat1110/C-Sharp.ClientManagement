using System;
using System.Threading;
using Microsoft.Win32;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using Accessibility;

namespace Client_Management.Services
{
    class DialogService : IDialogService
    {
        /// <summary>
        /// Generiert eine MessageBox mit Yes/No-Auswahlmöglichkeiten und zeigt diese an.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns>das Ergebnis von der Auswahl (Yes/No) vom Typ Enum DialogResult</returns>
        public async Task<DialogResult> ShowDialogYesNoAsync(string message, string caption)
        {
            await Task.Delay(0); 
            var messageBoxResult = MessageBox.Show(message, caption, MessageBoxButton.YesNo); 
            return messageBoxResult == MessageBoxResult.Yes ? DialogResult.Yes : DialogResult.No;
        }

        /// <summary>
        /// Generiert eine MessageBox mit OK-Button als Info, Warning, Error je nach Übergabewert des type vom Typ DialogType.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task ShowDialogOkAsync(string message, string caption, DialogType type)
        {
            await Task.Delay(0);
            switch (type)
            {
                case DialogType.Info:
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Information);
                    break;
                case DialogType.Warning:
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Warning);
                    break;
                case DialogType.Error:
                    MessageBox.Show(message, caption, MessageBoxButton.OK, MessageBoxImage.Error);
                    break;
                default:
                    MessageBox.Show(message, caption, MessageBoxButton.OK);
                    break;
            }
        }

        public async Task<string[]> FileDialog(string filter)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = filter;
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == true)
                {
                    return openFileDialog.FileNames;
                }
                return new string[0];
            }
            catch (Exception e)
            {
                DialogService dialog = new DialogService();
                await dialog.ShowDialogOkAsync(e.Message, "Error", DialogType.Error);
                return new string[0];
            }
        }
    }

    internal interface IDialogService
    {
        /// <summary>
        /// Soll Yes/No MessageBox generieren und anzeigen und das Ergebnis vom typ DialogResult zurückgeben.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <returns>(Yes/No) vom Typ Enum DialogResult</returns>
        Task<DialogResult> ShowDialogYesNoAsync(string message, string caption);

        /// <summary>
        /// Soll eine MessageBox mit Ok-Button generieren und anzeigen und je nach type vom Typ DialogType das Symbol anpassen (Information/Warning/Error).
        /// </summary>
        /// <param name="message"></param>
        /// <param name="caption"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        Task ShowDialogOkAsync(string message, string caption, DialogType type);

        /// <summary>
        /// Soll einen Dateiauswahl-Dialog mit dem angegebenen Filter öffnen und die ausgewählten Dateien zurückgeben, sofern bestätigt wurde. Bei Abbruch wird ein leeres Array zurückgegeben.
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>string[] Array der Dateiauswahl</returns>
        Task<string[]> FileDialog(string filter);
    }

    /// <summary>
    /// Dient als Rückgabewert bei den Yes/No-Auswahl des DialogFensters.
    /// </summary>
    public enum DialogResult{
        Yes,
        No
    }

    /// <summary>
    /// Dient als Typ-Auswahl für das Ok-DialogFenster
    /// </summary>
    public enum DialogType
    {
        Info,
        Warning,
        Error
    }
    
}
