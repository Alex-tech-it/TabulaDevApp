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
    class CardViewModel : ObservableObject
    {
        private Card _cardModel;
        private bool _isReady;
        public bool IsReady
        {
            get => _isReady;
            set
            {
                _isReady = value;
                OnPropertyChanged();
            }
        }

        public Card CardModel
        {
            get => _cardModel;
            set
            {
                _cardModel = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateSaveCardCommand { get; set; }
        public RelayCommand NavigateExitCardCommand { get; set; }
        public CardViewModel(NavigationStore navigationStore, KanbanBoardModel model,
            int indexColumn, int indexCard)
        {
            IsReady = false;
            CardModel = model.Lists[indexColumn].Cards[indexCard];
            NavigateSaveCardCommand = new RelayCommand(obj =>
            {
                if (CardModel.Title != "")
                {
                    IsReady = false;
                    model.Lists[indexColumn].Cards[indexCard] = CardModel;
                    navigationStore.UpperViewModel = null;
                    navigationStore.CurrentViewModel = new KanbanBoardViewModel(navigationStore, model);
                }
                else
                {
                    IsReady = true;
                }
            });
            NavigateExitCardCommand = new RelayCommand(obj =>
            {
                navigationStore.UpperViewModel = null;
            });
        }
    }
}
