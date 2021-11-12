using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace TabulaDevApp.MVVM.Models
{
    public class DragAndDropUIList
    {
        public StackPanel DragAndDropPanel { get; set; }
        public  ObservableCollection<Border> DragAndDropCardList { get; set; }

        public DragAndDropUIList()
        {
            DragAndDropPanel = new StackPanel();
            DragAndDropCardList = new ObservableCollection<Border>();
        }
    }
}
