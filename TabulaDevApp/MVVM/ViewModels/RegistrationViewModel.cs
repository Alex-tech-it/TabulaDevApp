using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Network;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;
using System.Threading;
using System.Windows;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class RegistrationViewModel : ObservableObject
    {
        private RegistrationModel _registrationModel;
        private UserNetwork network;

        // Check Red-fields
        private bool _isEmptyEmail;
        private bool _isEmptyNickName;
        private bool _isExistNickName;
        private bool _isEmptyPassword;
        private bool _isEmptyVeryPassword;

        // Red-fields getter & setter
        public bool IsEmptyEmail
        {
            get => _isEmptyEmail;
            set
            {
                _isEmptyEmail = value;
                OnPropertyChanged();
            }
        }
        public bool IsEmptyNickName
        {
            get => _isEmptyNickName;
            set
            {
                _isEmptyNickName = value;
                OnPropertyChanged();
            }
        }
        public bool IsExistNickName
        {
            get => _isExistNickName;
            set
            {
                _isExistNickName = value;
                OnPropertyChanged();
            }
        }
        public bool IsEmptyPassword
        {
            get => _isEmptyPassword;
            set
            {
                _isEmptyPassword = value;
                OnPropertyChanged();
            }
        }
        public bool IsEmptyVeryPassword
        {
            get => _isEmptyVeryPassword;
            set
            {
                _isEmptyVeryPassword = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateHomeCommand { get; set; }
        public RegistrationModel registrationModel
        {
            get => _registrationModel;
            set
            {
                _registrationModel = value;
                OnPropertyChanged();
            }
        }
        public RegistrationViewModel(NavigationStore navigationStore, UserModel user)
        {
            // Init network communication
            network = new UserNetwork();

            // Init visibiality Red-fields
            IsEmptyEmail = false;
            IsEmptyNickName = false;
            IsExistNickName = false;
            IsEmptyPassword = false;
            IsEmptyVeryPassword = false;

            registrationModel = new RegistrationModel();
            NavigateHomeCommand = new RelayCommand(obj =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bool flag = true;

                    PasswordBox box = obj as PasswordBox;
                    registrationModel.Password = box.Password.ToString();
                    Console.WriteLine(registrationModel.Password);
                    if (registrationModel.Email == "")
                    {
                        IsEmptyEmail = true;
                        flag = false;
                    }
                    else
                    {
                        IsEmptyEmail = false;
                    }

                    if (registrationModel.Username == "")
                    {
                        IsEmptyNickName = true;
                        flag = false;
                    }
                    else
                    {
                        IsEmptyNickName = false;
                    }

                    if (!IsEmptyNickName && !network.FreeNicknameCheck(registrationModel.Username))
                    {
                        IsExistNickName = true;
                        flag = false;
                    }
                    else
                    {
                        IsExistNickName = false;
                    }

                    if (registrationModel.Password == "")
                    {
                        IsEmptyPassword = true;
                        IsEmptyVeryPassword = true;
                        flag = false;
                    }
                    else
                    {
                        IsEmptyPassword = false;
                        IsEmptyVeryPassword = false;
                    }

                    if (flag)
                    {
                        IsEmptyEmail = false;
                        IsEmptyNickName = false;
                        IsEmptyPassword = false;
                        IsEmptyVeryPassword = false;
                        
                        network.CreateUser(registrationModel);
                    
                        navigationStore.IsLogged = false;
                        navigationStore.CurrentViewModel = new HomeViewModel(navigationStore, user);
                    }
                });
            });
        }
    }
}
