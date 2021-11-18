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
    class CreatKanbanBoardViewModel : ObservableObject
    {
        private string _titleBoard;
        private bool _isNext;
        public string TitleBoard {
            get => _titleBoard;
            set
            {
                _titleBoard = value;
                OnPropertyChanged();
            }
        }
        public bool IsNext
        {
            get => _isNext;
            set
            {
                _isNext = value;
                OnPropertyChanged();
            }
        }
        
        public RelayCommand NavigateCreatBoardCommand { get; set; }
        public RelayCommand NavigateExitBoardCommand { get; set; }

        public CreatKanbanBoardViewModel(NavigationStore upperNavigation, 
            NavigationStore navigation,
            ObservableCollection<KanbanBoardModel> listBoards)
        {
            IsNext = false;
            TitleBoard = "Новая доска";
            NavigateCreatBoardCommand = new RelayCommand(obj =>
            {
                if (TitleBoard != "")
                {
                    KanbanBoardModel newModel = new KanbanBoardModel();
                    IsNext = false; ;
                    newModel.TitleBoard = TitleBoard;
                    listBoards.Add(newModel);
                    navigation.UpperViewModel = null;
                    navigation.CurrentViewModel = new KanbanBoardViewModel(navigation, newModel, listBoards, upperNavigation);
                    upperNavigation.CurrentViewModel = new HomeViewModel(upperNavigation, listBoards, navigation);

                } else
                {
                    IsNext = true;
                }
                
            });

            NavigateExitBoardCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new MainPageViewModel();
            });
        }
    }
}
