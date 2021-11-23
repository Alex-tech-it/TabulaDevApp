using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    class UpperViewModel : ObservableObject
    {
        public RelayCommand NavigateAuthorizationCommand { get; set; }
        public RelayCommand NavigateStateCommand { get; set; }
        public UpperViewModel(NavigationStore navigationStore, UserModel user)
        {
            NavigateAuthorizationCommand = new RelayCommand(obj =>
            {
                user.Clear();
                navigationStore.CurrentViewModel = new AuthorizationViewModel(navigationStore, user);
                navigationStore.IsLogged = true;
                navigationStore.UpperViewModel = null;
            });

            NavigateStateCommand = new RelayCommand(obj =>
            {
                navigationStore.UpperViewModel = null;
            });
        }
    }
}
