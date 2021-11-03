﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Servies;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class MainViewModel : ObservableObject
    {
        // Navigator to save and move views
        private NavigationStore _navigationStore;
        private PreviewViewModel previewViewModel;
        private AuthorizationViewModel authorizationViewModel;

        // Commands for moving the View within the home screen
        public RelayCommand NavigatePreviewCommand { get; set; }
        public RelayCommand NavigateAuthorizationCommand { get; set; }

        public bool IsLogged => _navigationStore.IsLogged;
        public ObservableObject CurrentViewModel => _navigationStore.CurrentViewModel;
        public ObservableObject UpperViewModel => _navigationStore.UpperViewModel;


        // Class constructor
        public MainViewModel(NavigationStore navigationStore)
        {
            // Init Navigator Store
            _navigationStore = navigationStore;
            _navigationStore.CurrentViewModelChanged += OnCurrentViewModelChanged;
            _navigationStore.UpperViewModelChanged += OnUpperViewModelChanged;
            _navigationStore.IsLoggedChanged += OnLoggedChanged;
            _navigationStore.IsLogged = true;

            // Init ViewModels
            previewViewModel = new PreviewViewModel();
            authorizationViewModel = new AuthorizationViewModel(_navigationStore);

            // Init Commands
            NavigatePreviewCommand = new RelayCommand(obj =>
            {
                _navigationStore.CurrentViewModel = previewViewModel;
            });
            NavigateAuthorizationCommand = new RelayCommand(obj =>
            {
                _navigationStore.CurrentViewModel = authorizationViewModel;
            });
        }


        /// <summary>
        /// The function to send a notification to 
        /// listeners that the viewmodel has been changed
        /// </summary>
        private void OnCurrentViewModelChanged()
        {
            OnPropertyChanged(nameof(CurrentViewModel));
        }

        private void OnUpperViewModelChanged()
        {
            OnPropertyChanged(nameof(UpperViewModel));
        }
        private void OnLoggedChanged()
        {
            OnPropertyChanged(nameof(IsLogged));
        }
    }
}
