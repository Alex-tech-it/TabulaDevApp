using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabulaDevApp.Core.Servies
{
    public class NavigationStore
    {
        private ObservableObject _observableObject;
        private ObservableObject _observableUpperObject;

        // Flag to track whether the user has been authorised or registered
        private bool _isLogged;
        public ObservableObject CurrentViewModel {
            get => _observableObject; 
            set
            {
                _observableObject = value;
                OnCurrentViewModelChanged();
            }
        }
        public ObservableObject UpperViewModel
        {
            get => _observableUpperObject;
            set
            {
                _observableUpperObject = value;
                OnUpperViewModelChanged();
            }
        }
        public bool IsLogged
        {
            get => _isLogged;
            set
            {
                _isLogged = value;
                OnLoggedChanged();
            }
        }

        public event Action CurrentViewModelChanged;
        public event Action UpperViewModelChanged;
        public event Action IsLoggedChanged;

        private void OnCurrentViewModelChanged()
        {
            CurrentViewModelChanged?.Invoke();
        }
        private void OnUpperViewModelChanged()
        {
            UpperViewModelChanged?.Invoke();
        }
        private void OnLoggedChanged()
        {
            IsLoggedChanged?.Invoke();
        }
    }
}
