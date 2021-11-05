using System.Collections.ObjectModel;

namespace TabulaDevApp.MVVM.Models
{
    public class ColumnData
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public ObservableCollection<Card> Cards { get; set; }
        public ColumnData()
        {
            Title = "";
            Cards = new ObservableCollection<Card>();
        }
    }
}