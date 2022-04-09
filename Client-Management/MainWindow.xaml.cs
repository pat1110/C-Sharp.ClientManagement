using System;
using System.Threading.Tasks;
using Client_Management.Services;
using Client_Management.ViewModel;
using System.Windows;

namespace Client_Management
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private MainViewModel _mainViewModel;
        private DialogService _dialog = new DialogService();

        public MainWindow()
        {
            InitializeComponent();

            //Überprüfen der Einstellungen zur DB
            var task = Test().GetAwaiter();
            task.OnCompleted(() =>
            {
                //Wenn Fehler, dann das Fenster für die Einstellungen öffnen
                if (!task.GetResult())
                {
                    SettingsWindow settingsWindow = new SettingsWindow();
                    settingsWindow.ShowDialog();
                }
            });
            _mainViewModel = new MainViewModel();
            DataContext = _mainViewModel;

        }

        /// <summary>
        /// Um die Datei für die Einstellungen und die Einstellungen selbst für die Verbindung zur Datenbank zu überprüfen. Schlägt es fehl wird auch gleich ein DialogFenster geöffnet mit Hinweisen zum Fehler.
        /// </summary>
        /// <returns>True oder False vom Typ bool, je nachdem, ob der Test erfolgreich war oder nicht.</returns>
        private async Task<bool> Test()
        {
            try
            {
                await FileDataService.Deserialize("settings", "settings.xml");
                await DatabaseService.TestConnectionAsync();
                return true;
            }
            catch (Exception e)
            {
                await _dialog.ShowDialogOkAsync("If you open this application for the first time than this is not a problem at all - Click OK and we will proceed with editing the settings :-)\n\nBut if you already configured this application and this error occures there might be a problem :-/ - please see the following error message for more information:\n\n" + e.Message, "Error!", DialogType.Warning);
                return false;
            }
        }

        private void OpenSelected(object sender, RoutedEventArgs e)
        {
            _mainViewModel.OpenSelected();
        }

        private void Btn_New_CommandPackage_click(object sender, RoutedEventArgs e)
        {
            CommandPackageWindow commandPackageWindow = new CommandPackageWindow();
            commandPackageWindow.Show();
        }

        private void Btn_Settings_click(object sender, RoutedEventArgs e)
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }

        private void Btn_Help_click(object sender, RoutedEventArgs e)
        {
            HelpWindow helpWindow = new HelpWindow();
            helpWindow.Show();
        }

        private void Btn_New_Computer_click(object sender, RoutedEventArgs e)
        {
            ComputerWindow computerWindow = new ComputerWindow();
            computerWindow.Show();
        }

        private void Btn_New_Group_click(object sender, RoutedEventArgs e)
        {
            GroupWindow groupWindow = new GroupWindow();
            groupWindow.Show();
        }

        /// <summary>
        /// Hilfsfunktion zum öffnen von beliebigen Fenstern.
        /// </summary>
        /// <param name="w"></param>
        public static void OpenSelected(Window w)
        {
            w.Show();
        }
    }
}
