using Client.Config;
using Client.Model;
using Client.Views.Windows;
using Microsoft.Win32;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Server.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Client.ViewModel
{
    public class AuthViewModel : INotifyPropertyChanged
    {
        private string login, password;
        public string Login
        {
            get => login;
            set
            {
                if (login != value)
                {
                    login = value;
                    OnPropertyChanged(nameof(Login));
                }
            }
        }
        public string Password
        {
            get => password;
            set
            {
                if (password != value)
                {
                    password = value;
                    OnPropertyChanged(nameof(Password));
                }
            }
        }

        private bool saveUser = false;
        public bool SaveUser
        {
            get => saveUser;
            set
            {
                if (saveUser != value)
                {
                    saveUser = value;
                    OnPropertyChanged(nameof(SaveUser));
                }
            }
        }

        private Visibility visiblePass = Visibility.Visible, hiddenPass = Visibility.Collapsed;
        public Visibility VisiblePass
        {
            get => visiblePass;
            set
            {
                if (visiblePass != value)
                {
                    visiblePass = value;
                    OnPropertyChanged(nameof(VisiblePass));
                }
            }
        }
        public Visibility HiddenPass
        {
            get => hiddenPass;
            set
            {
                if (hiddenPass != value)
                {
                    hiddenPass = value;
                    OnPropertyChanged(nameof(HiddenPass));
                }
            }
        }

        private WindowAuth windowAuth;
        public AuthViewModel(WindowAuth _windowAuth)
        {
            windowAuth = _windowAuth;

            try
            {
                TokenResponse tokenRes = new TokenResponse()
                {
                    Login = Properties.Settings.Default.Login,
                    Token = Properties.Settings.Default.TokenVS
                };
                GetToken(tokenRes);
            }
            catch
            {
                return;
            }
        }

        private async void GetToken(TokenResponse tokenRes)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + tokenRes.Token);
                    var response = await client.GetAsync($"{BaseUrl.url}/api/auth/");
                    if (response.IsSuccessStatusCode)
                    {
                        Token.token = tokenRes.Token;
                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();

                        windowAuth.Close();
                    }
                }
            }
            catch
            {
                return;
            }
        }


        private RelayCommand authCommand;
        public RelayCommand AuthCommand
        {
            get
            {
                return authCommand ?? (authCommand = new RelayCommand(async obj =>
                {
                    try
                    {
                        if (String.IsNullOrEmpty(Login) && String.IsNullOrEmpty(Password))
                            MessageBox.Show("Данные не могут быть пустыми!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                        else if (String.IsNullOrEmpty(Login))
                            MessageBox.Show("Поле 'Логин' не может быть пустым!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                        else if (String.IsNullOrEmpty(Password))
                            MessageBox.Show("Поле 'Пароль' не может быть пустым!", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                        else
                        {

                            using (HttpClient client = new HttpClient())
                            {

                                TokenRequest tokenRequest = new TokenRequest()
                                {
                                    Login = Login,
                                    Password = Password
                                };
                                var json = JsonConvert.SerializeObject(tokenRequest);
                                var content = new StringContent(json, Encoding.UTF8, "application/json");

                                var response = await client.PostAsync(BaseUrl.url + "/api/auth/GetToken", content);
                                var responseContent = await response.Content.ReadAsStringAsync();

                                if (response.IsSuccessStatusCode)
                                {
                                    var TokenRes = JsonConvert.DeserializeObject<TokenResponse>(responseContent);


                                    if(saveUser) 
                                        SaveToken(TokenRes);


                                    MainWindow mainWindow = new MainWindow();
                                    mainWindow.Show();

                                    windowAuth.Close();
                                }
                                else MessageBox.Show("При авторизации произошла ошибка!" +
                                        $"\nОписание ошибки: {responseContent}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Произошла критическая ошибка! \nОписание ошибки: {ex.Message}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Error);
                    }


                }));
            }
        }

        private static void SaveToken(TokenResponse TokenRes)
        {
            Properties.Settings.Default.TokenVS = string.Empty;
            Properties.Settings.Default.Login = string.Empty;
            Properties.Settings.Default.Save();


            Properties.Settings.Default.TokenVS = TokenRes.Token;
            Properties.Settings.Default.Login = TokenRes.Login;
            Properties.Settings.Default.Save();

            Token.token = TokenRes.Token;
            Token.login = TokenRes.Login;
        }

        private RelayCommand changeVisibilityPassword;
        public RelayCommand ChangeVisibilityPassword
        {
            get
            {
                return changeVisibilityPassword ?? (changeVisibilityPassword = new RelayCommand(obj =>
                {

                    try
                    {
                        if(VisiblePass == Visibility.Visible) // Если у нас скрытый пароль
                        {
                            VisiblePass = Visibility.Collapsed;
                            HiddenPass = Visibility.Visible;
                        }
                        else
                        {
                            HiddenPass = Visibility.Collapsed;
                            VisiblePass = Visibility.Visible;
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
