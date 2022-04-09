using Client_Management.Model;
using Client_Management.ViewModel;


namespace Client_Management
{
    /// <summary>
    /// Interaktionslogik für GroupWindow.xaml
    /// </summary>
    public partial class GroupWindow
    {
        public GroupWindow()
        {
            InitializeComponent();
            DataContext = new GroupWindowViewModel();
        }
        public GroupWindow(Group group)
        {
            InitializeComponent();
            DataContext = new GroupWindowViewModel(group);
        }
    }
}
