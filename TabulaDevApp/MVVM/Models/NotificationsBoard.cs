using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabulaDevApp.MVVM.Models
{
    public class NotificationsBoard
    {
        public string User { get; set; }
        public string From { get; set; }
        public string TitleBoard { get; set; }
        public string UniqueId { get; set; }

        public NotificationsBoard()
        {
            User = "";
            From = "";
            TitleBoard = "";
            UniqueId = "";
        }
    }
}
