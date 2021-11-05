using System.Collections;
using System.Collections.ObjectModel;

namespace TabulaDevApp.MVVM.Models
{
    public class Card
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public ArrayList Tags { get; set; }
        public ObservableCollection<Participant> Participants { get; set; }
    }
}