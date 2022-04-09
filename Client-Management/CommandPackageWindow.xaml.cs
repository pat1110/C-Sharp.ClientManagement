using Client_Management.ViewModel;

namespace Client_Management
{
    /// <summary>
    /// Interaktionslogik für CommandPackageWindow.xaml
    /// </summary>
    public partial class CommandPackageWindow
    {
        public CommandPackageWindow()
        {
            InitializeComponent();
            DataContext = new CommandPackageViewModel();
        }
        public CommandPackageWindow(string commandPackage)
        {
            InitializeComponent();
            DataContext = new CommandPackageViewModel(commandPackage);
        }
    }
}
