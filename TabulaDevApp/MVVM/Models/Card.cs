using System.Collections;
using System.Collections.ObjectModel;
using TabulaDevApp.Core.Servies;

namespace TabulaDevApp.MVVM.Models
{
    public class Card : ObservableObject
    {
        private string _title;
        private ObservableCollection<LabelData> _cardLabelList;

        public bool isDrag;
        public string id;
        public string Title {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            } 
        }
        public string Description { get; set; }

        public ObservableCollection<LabelData> CardLabelList
        {
            get => _cardLabelList;
            set
            {
                _cardLabelList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Participant> Participants { get; set; }

        public Card()
        {
            Title = "Заголовок карточки";
            Description = "Описание карточки";
            isDrag = false;
            CardLabelList = new ObservableCollection<LabelData>();
        }
    }
}