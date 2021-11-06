using System.Collections.ObjectModel;
using TabulaDevApp.Core.Servies;

namespace TabulaDevApp.MVVM.Models
{
    public class ColumnData : ObservableObject
    {
        private ObservableCollection<Card> _cards;
        public string Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<Card> Cards {
            get => _cards;
            set
            {
                _cards = value;
                OnPropertyChanged();
            }
        }
        public ColumnData()
        {
            Title = "";
            Cards = new ObservableCollection<Card>();
        }
    }
}