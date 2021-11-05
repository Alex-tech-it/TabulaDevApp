using System.Collections.ObjectModel;

namespace TabulaDevApp.MVVM.Models
{
    public class KanbanBoardModel
    {
        public int countColumn { get; set; }
        public string UniqueId { get; set; }
        public string TitleBoard { get; set; }
        public string OwnCreater { get; set; }
        public ObservableCollection<ColumnData> Lists { get; set; }
        public KanbanBoardModel()
        {
            TitleBoard = "Новая доска";
            Lists = new ObservableCollection<ColumnData>();
        }
    }
}
