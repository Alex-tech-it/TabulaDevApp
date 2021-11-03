using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Servies;

namespace TabulaDevApp.MVVM.ViewModels
{
    class UpperViewModel : ObservableObject
    {
        public RelayCommand NavigateAuthorizationCommand { get; set; }
        public RelayCommand NavigateStateCommand { get; set; }
        public UpperViewModel(NavigationStore navigationStore)
        {
            NavigateAuthorizationCommand = new RelayCommand(obj =>
            {
                navigationStore.CurrentViewModel = new AuthorizationViewModel(navigationStore);
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
