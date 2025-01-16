using Client.Config;
using Client.Views.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Client.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {

        private MainWindow mainWindow;
        public MainWindowViewModel(MainWindow _mainWindow)
        {
            mainWindow = _mainWindow;
        }

        private RelayCommand exitCommand;
        public RelayCommand ExitCommand
        {
            get
            {
                return exitCommand ?? (exitCommand = new RelayCommand(obj =>
                {
                    try
                    {
                        if (MessageBox.Show("Вы точно хотите выйти из учётной записи?", "Уведомление", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            Properties.Settings.Default.TokenVS = string.Empty;
                            Properties.Settings.Default.Login = string.Empty;
                            Properties.Settings.Default.Save();

                            WindowAuth auth = new WindowAuth();
                            auth.Show();

                            mainWindow.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла критическая ошибка! \nОписание ошибки: {ex.Message}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }));
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
