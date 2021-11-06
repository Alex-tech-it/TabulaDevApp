using System.Collections.ObjectModel;
using TabulaDevApp.Core.Servies;

namespace TabulaDevApp.MVVM.Models
{
    public class KanbanBoardModel : ObservableObject
    {
        private string _titleBoard;
        private ObservableCollection<ColumnData> _lists;
        public int countColumn { get; set; }
        public string UniqueId { get; set; }
        public string TitleBoard {
            get => _titleBoard;
            set
            {
                _titleBoard = value;
                OnPropertyChanged();
            }
        }
        public string OwnCreater { get; set; }
        public ObservableCollection<ColumnData> Lists {
            get => _lists; 
            set
            {
                _lists = value;
                OnPropertyChanged();
            }
        }
        public KanbanBoardModel()
        {
            TitleBoard = "Новая доска";
            Lists = new ObservableCollection<ColumnData>();
        }
    }
}
