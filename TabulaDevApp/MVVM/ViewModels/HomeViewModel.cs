using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Servies;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        // Navigator to save and move views
        private NavigationStore _navigationMenuStore;

        // Commands for moving the View within the home screen
        public RelayCommand NavigateMainPageCommand { get; set; }
        public RelayCommand NavigateCommunityCommand { get; set; }
        public RelayCommand NavigateOwnCardsCommand { get; set; }
        public RelayCommand NavigateSettingsCommand { get; set; }
        public RelayCommand NavigateOutCommand { get; set; }

        public ObservableObject CurrentViewModel => _navigationMenuStore.CurrentViewModel;
        public bool IsLogged => _navigationMenuStore.IsLogged;

        public HomeViewModel(NavigationStore navigationStore)
        {
            _navigationMenuStore = new NavigationStore();
            _navigationMenuStore.CurrentViewModel = new VisitViewModel();
            _navigationMenuStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _navigationMenuStore.IsLoggedChanged += OnLoggedChanged;
            _navigationMenuStore.IsLogged = false;

            NavigateMainPageCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.CurrentViewModel = new MainPageViewModel();
            });

            NavigateCommunityCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.CurrentViewModel = new CommunityViewModel();
            });

            NavigateOwnCardsCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.CurrentViewModel = new OwnCardViewModel();
            });

            NavigateSettingsCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.CurrentViewModel = new SettingsViewModel();
            });

            NavigateOutCommand = new RelayCommand(obj =>
            {
                navigationStore.UpperViewModel = new UpperViewModel(navigationStore);
            });
        }

        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void OnLoggedChanged()
        {
            OnPropertyChanged(nameof(IsLogged));
        }

    }
}
