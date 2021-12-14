using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabulaDevApp.MVVM.Models
{
    public class UserModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Username { get; set; }
        public ObservableCollection<KanbanBoardModel> userBoards { get; set; }
        public ObservableCollection<NotificationsBoard> notifications { get; set; }

        public UserModel()
        {
            Email = "";
            Password = "";
            Username = "";
            userBoards = new ObservableCollection<KanbanBoardModel>();
            notifications = new ObservableCollection<NotificationsBoard>();
        }

        public void Clear()
        {
            Email = "";
            Password = "";
            Username = "";
            userBoards = new ObservableCollection<KanbanBoardModel>();
            notifications = new ObservableCollection<NotificationsBoard>();
        }

    }
}
