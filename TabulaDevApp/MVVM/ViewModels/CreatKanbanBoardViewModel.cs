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

        public CreatKanbanBoardViewModel(NavigationStore navigation, KanbanBoardModel model)
        {
            IsNext = false;
            TitleBoard = "";
            NavigateCreatBoardCommand = new RelayCommand(obj =>
            {
                if (TitleBoard != "")
                {
                    IsNext = false; ;
                    model.TitleBoard = TitleBoard;
                    navigation.UpperViewModel = null;
                    navigation.CurrentViewModel = new KanbanBoardViewModel(navigation, model);
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
