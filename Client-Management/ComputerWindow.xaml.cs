using Client_Management.Model;
using Client_Management.ViewModel;

namespace Client_Management
{
    /// <summary>
    /// Interaktionslogik für ComputerWindow.xaml
    /// </summary>
    public partial class ComputerWindow
    {
        public ComputerWindow()
        {
            InitializeComponent();
            DataContext = new ComputerViewModel();
        }
        public ComputerWindow(Computer computer)
        {
            InitializeComponent();
            DataContext = new ComputerViewModel(computer);
        }
    }
}
