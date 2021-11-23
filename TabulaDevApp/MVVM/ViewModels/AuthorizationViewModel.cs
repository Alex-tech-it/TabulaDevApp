using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Network;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class AuthorizationViewModel : ObservableObject
    {
        // Data
        private AuthorizationModel _authorizationModel;
        private UserNetwork network;

        // Check Red-fields
        private bool _isEmptyEmail;
        private bool _isEmptyPassword;
        private bool _enterFlag;


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
        public bool IsEmptyPassword
        {
            get => _isEmptyPassword;
            set
            {
                _isEmptyPassword = value;
                OnPropertyChanged();
            }
        }


        // Data getter & setter
        public AuthorizationModel authorizationModel
        {
            get => _authorizationModel;
            set
            {
                _authorizationModel = value;
                OnPropertyChanged();
            }
        }
        public bool EnterFlag
        {
            get => _enterFlag;
            set
            {
                _enterFlag = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand NavigateHomeCommand { get; set; }
        public RelayCommand NavigateRegistrationCommand { get; set; }

        public AuthorizationViewModel(NavigationStore navigationStore, UserModel user)
        {
            network = new UserNetwork();

            // Init Red-fields
            EnterFlag = false;
            IsEmptyEmail = false;
            IsEmptyPassword = false;

            authorizationModel = new AuthorizationModel();
            NavigateHomeCommand = new RelayCommand(obj =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    bool flag = true;
                    PasswordBox box = obj as PasswordBox;
                    authorizationModel.Password = box.Password;

                    if (authorizationModel.Login == "")
                    {
                        IsEmptyEmail = true;
                        flag = false;
                    }
                    else
                    {
                        IsEmptyEmail = false;
                    }

                    if (authorizationModel.Password == "")
                    {
                        IsEmptyPassword = true;
                        flag = false;
                    }
                    else
                    {
                        IsEmptyPassword = false;
                    }

                    if (flag)
                    {
                        UserModel checkUser = network.Entrance(authorizationModel);
                        if (checkUser != null)
                        {
                            EnterFlag = false;

                            user = checkUser;
                            navigationStore.IsLogged = false;
                            navigationStore.CurrentViewModel = new HomeViewModel(navigationStore, user);
                        }
                        else
                        {
                            EnterFlag = true;
                        }
                    }
                });
            });
            NavigateRegistrationCommand = new RelayCommand(obj =>
            {
                navigationStore.CurrentViewModel = new RegistrationViewModel(navigationStore, user);
                authorizationModel = new AuthorizationModel();
            });
        }

        
    }
}
