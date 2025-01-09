using Client.ViewModel;
using System.Windows;

namespace Client.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowAuth.xaml
    /// </summary>
    public partial class WindowAuth : Window
    {
        public WindowAuth()
        {
            InitializeComponent();
            DataContext = new AuthViewModel();
        }
    }
}
