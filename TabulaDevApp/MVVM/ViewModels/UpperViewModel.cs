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
        public UpperViewModel(NavigationStore navigationStore, ObservableCollection<KanbanBoardModel> listBoards)
        {
            NavigateAuthorizationCommand = new RelayCommand(obj =>
            {
                navigationStore.CurrentViewModel = new AuthorizationViewModel(navigationStore, listBoards);
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
