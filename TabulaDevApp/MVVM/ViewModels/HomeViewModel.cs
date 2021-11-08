using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        // Navigator to save and move views
        private NavigationStore _navigationMenuStore;
        private StackPanel _listBoards;
        private ObservableCollection<KanbanBoardModel> _dataList;

        public StackPanel StackPanelListBoards
        {
            get => _listBoards;
            set
            {
                _listBoards = value;
                OnPropertyChanged("StackPanelListBoards");
            }
        }
        public ObservableCollection<KanbanBoardModel> DataList
        {
            get => _dataList;
            set
            {
                _dataList = value;
                OnPropertyChanged("DataList");
            }
        }

        // Commands for moving the View within the home screen
        public RelayCommand NavigateMainPageCommand { get; set; }
        public RelayCommand NavigateCommunityCommand { get; set; }
        public RelayCommand NavigateOwnCardsCommand { get; set; }
        public RelayCommand NavigateSettingsCommand { get; set; }
        public RelayCommand NavigateKanbanBoardCommand { get; set; }
        public RelayCommand NavigateOutCommand { get; set; }

        public ObservableObject CurrentViewModel => _navigationMenuStore.CurrentViewModel;
        public ObservableObject UpperViewModel => _navigationMenuStore.UpperViewModel;
        public bool IsLogged => _navigationMenuStore.IsLogged;

        public HomeViewModel(NavigationStore navigationStore, ObservableCollection<KanbanBoardModel> listBoards,
            NavigationStore restoreNavigationMenuStore = null)
        {
            DataList = listBoards;
            StackPanelListBoards = new StackPanel();
            StackPanelListBoards.HorizontalAlignment = HorizontalAlignment.Center;
            UpdateListBoards();

            if (restoreNavigationMenuStore != null)
            {
                _navigationMenuStore = restoreNavigationMenuStore;
                _navigationMenuStore.CurrentViewModel = restoreNavigationMenuStore.CurrentViewModel;
                _navigationMenuStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

                _navigationMenuStore.UpperViewModelChanged += OnUpperViewModelChanged;
                _navigationMenuStore.UpperViewModel = restoreNavigationMenuStore.UpperViewModel;

                _navigationMenuStore.IsLoggedChanged += OnLoggedChanged;
                _navigationMenuStore.IsLogged = restoreNavigationMenuStore.IsLogged;
            }
            else {
                _navigationMenuStore = new NavigationStore();
                _navigationMenuStore.CurrentViewModel = new VisitViewModel();
                _navigationMenuStore.CurrentViewModelChanged += OnCurrentViewModelChanged;

                _navigationMenuStore.UpperViewModelChanged += OnUpperViewModelChanged;
                _navigationMenuStore.UpperViewModel = null;

                _navigationMenuStore.IsLoggedChanged += OnLoggedChanged;
                _navigationMenuStore.IsLogged = false;
            }

            NavigateMainPageCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.UpperViewModel = null;
                _navigationMenuStore.CurrentViewModel = new MainPageViewModel();
            });

            NavigateCommunityCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.UpperViewModel = null;
                _navigationMenuStore.CurrentViewModel = new CommunityViewModel();
            });

            NavigateOwnCardsCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.UpperViewModel = null;
                _navigationMenuStore.CurrentViewModel = new OwnCardViewModel();
            });

            NavigateSettingsCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.UpperViewModel = null;
                _navigationMenuStore.CurrentViewModel = new SettingsViewModel();
            });

            NavigateKanbanBoardCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.UpperViewModel = null;
                _navigationMenuStore.CurrentViewModel = new CreatKanbanBoardViewModel(
                    navigationStore,
                    _navigationMenuStore,
                    DataList
                );
            });

            NavigateOutCommand = new RelayCommand(obj =>
            {
                _navigationMenuStore.UpperViewModel = null;
                navigationStore.UpperViewModel = new UpperViewModel(navigationStore, DataList);
            });
        }

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
        
        private void UpdateListBoards()
        {
            StackPanel newPanel = new StackPanel();
            for (int i = 0; i < DataList.Count; i++)
            {
                Button newButton = new Button();
                newButton.Name = "Board_" + Convert.ToString(i);
                newButton.Click += OnBoardClick;
                newButton.Margin = new Thickness(0, 5, 0, 5);
                newButton.Content = DataList[i].TitleBoard;
                newButton.Foreground = Brushes.White;
                newButton.Style = Application.Current.Resources["PreviewNavigationButtonStyle"] as Style;
                
                newPanel.Children.Add(newButton);
            }

            StackPanelListBoards = newPanel;
        }
        private void OnBoardClick(object sender, RoutedEventArgs e)
        {
            Button board = (Button)sender;
            string[] arrayWordsButton = board.Name.Split(new char[] { '_' });
            int index = Convert.ToInt32(arrayWordsButton[1]);

            _navigationMenuStore.CurrentViewModel = new KanbanBoardViewModel(_navigationMenuStore, DataList[index]);
        }
    }
}
