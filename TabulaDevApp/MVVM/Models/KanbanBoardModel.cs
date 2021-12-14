using System.Collections;
using System.Collections.ObjectModel;
using TabulaDevApp.Core.Servies;

namespace TabulaDevApp.MVVM.Models
{
    public class KanbanBoardModel : ObservableObject
    {
        private string _titleBoard;
        private ObservableCollection<ColumnData> _lists;
        private ObservableCollection<LabelData> _labelList;
        private ObservableCollection<string> _participants;

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
        public ObservableCollection<LabelData> LabelList
        {
            get => _labelList;
            set
            {
                _labelList = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<string> Participants
        {
            get => _participants;
            set
            {
                _participants = value;
                OnPropertyChanged();
            }
        }

        public KanbanBoardModel()
        {
            TitleBoard = "Новая доска";
            Lists = new ObservableCollection<ColumnData>();
            LabelList = new ObservableCollection<LabelData>();
            Participants = new ObservableCollection<string>();
        }
    }
}
