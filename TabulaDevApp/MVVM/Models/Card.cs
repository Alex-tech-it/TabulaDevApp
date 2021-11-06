using System.Collections;
using System.Collections.ObjectModel;
using TabulaDevApp.Core.Servies;

namespace TabulaDevApp.MVVM.Models
{
    public class Card : ObservableObject
    {
        private string _title;
        public string Title {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            } 
        }
        public string Description { get; set; }
        public ArrayList Tags { get; set; }
        public ObservableCollection<Participant> Participants { get; set; }

        public Card()
        {
            Title = "Заголовок карточки";
            Description = "Описание карточки";
        }
    }
}