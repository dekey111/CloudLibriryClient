using Client.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Client.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для WindowAuth.xaml
    /// </summary>
    public partial class WindowAuth : Window
    {
        private AuthViewModel _viewModel;
        public WindowAuth()
        {
            InitializeComponent();
            _viewModel = new AuthViewModel(this);
            DataContext = _viewModel;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                _viewModel.Password = passwordBox.Password;
            }
        }
    }
}
