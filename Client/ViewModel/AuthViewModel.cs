using Client.Config;
using Client.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

        private RelayCommand authCommand;
        public RelayCommand AuthCommand
        {
            get
            {
                return authCommand ?? (authCommand = new RelayCommand(async obj =>
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
                                TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(responseContent);
                                BaseUrl.Token = token.Token;
                                MessageBox.Show("Мы авторизовались!" +
                                    $"\nidRole: {token.IdRole}" +
                                    $"\nToken: {BaseUrl.Token}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else MessageBox.Show("При авторизации произошла ошибка!" +
                                    $"\nОписание ошибки: {responseContent}", "Уведомление", MessageBoxButton.OK, MessageBoxImage.Warning);
                        }
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
