using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    class AuthorizationViewModel : ObservableObject
    {
        private AuthorizationModel _authorizationModel;
        public AuthorizationModel authorizationModel
        {
            get => _authorizationModel;
            set
            {
                _authorizationModel = value;
                OnPropertyChanged();
            }
        }
        public RelayCommand NavigateHomeCommand { get; set; }
        public RelayCommand NavigateRegistrationCommand { get; set; }

        public AuthorizationViewModel(NavigationStore navigationStore)
        {
            authorizationModel = new AuthorizationModel();
            NavigateHomeCommand = new RelayCommand(obj =>
            {
                navigationStore.IsLogged = false;
                navigationStore.CurrentViewModel = new HomeViewModel(navigationStore);
            });
            NavigateRegistrationCommand = new RelayCommand(obj =>
            {
                navigationStore.CurrentViewModel = new RegistrationViewModel(navigationStore);
                authorizationModel = new AuthorizationModel();
            });
        }
    }
}
