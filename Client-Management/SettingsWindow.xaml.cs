using Client_Management.ViewModel;

namespace Client_Management
{
    /// <summary>
    /// Interaktionslogik für SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow
    {
        SettingsViewModel settings_ViewModel = new SettingsViewModel();

        public SettingsWindow()
        {
            InitializeComponent();

            DataContext = settings_ViewModel;

        }
    }
}
