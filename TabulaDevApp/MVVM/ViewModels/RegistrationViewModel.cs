using System.Collections.ObjectModel;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class RegistrationViewModel : ObservableObject
    {
        private RegistrationModel _registrationModel;
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
        public RegistrationViewModel(NavigationStore navigationStore, ObservableCollection<KanbanBoardModel> listBoards)
        {
            registrationModel = new RegistrationModel();
            NavigateHomeCommand = new RelayCommand(obj =>
            {
                navigationStore.IsLogged = false;
                navigationStore.CurrentViewModel = new HomeViewModel(navigationStore, listBoards);
            });
        }
    }
}
