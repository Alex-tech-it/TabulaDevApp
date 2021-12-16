using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabulaDevApp.MVVM.Models
{
    public class ChatModel
    {
        public string User { get; set; }
        public string Date { get; set; }
        public string Text { get; set; }

        public ChatModel()
        {
            User = "";
            Date = "";
            Text = "";
        }
    }
}
