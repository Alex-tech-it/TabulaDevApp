using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TabulaDevApp.Core.Network;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class MainPageViewModel : ObservableObject
    {
        private UserNetwork network;
        private UserModel _user;
        private StackPanel _userNotify;
        private ObservableCollection<NotificationsBoard> _notifyList;

        public StackPanel UserNotify
        {
            get => _userNotify;
            set
            {
                _userNotify = value;
                OnPropertyChanged();
            }
        }


        public MainPageViewModel(UserModel user)
        {
            _user = user;
            network = new UserNetwork();

            GetNotify("@" + _user.Username);
        }

        private async void GetNotify(string userName)
        {
            ObservableCollection<NotificationsBoard> notify = await network.GetNotify(userName);
            _notifyList = new ObservableCollection<NotificationsBoard>(notify);

            Console.WriteLine(userName);

            StackPanel newPanel = new StackPanel();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            if (notify.Count == 0)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;
                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                textBlock.Margin = new Thickness(0, 10, 0, 5);
                textBlock.Text = "В данный момент у вас нет никаких уведомлений!";
                textBlock.Foreground = foreground;
                newPanel.Children.Add(textBlock);
            }
            else
            {
                int index = 0;
                foreach (NotificationsBoard item in notify)
                {
                    // Init Layouts
                    Border border = new Border();
                    Grid grid = new Grid();
                    StackPanel titleNotify = new StackPanel();
                    StackPanel buttons = new StackPanel();

                    // Init UI Elements
                    TextBlock usernameFrom = new TextBlock();
                    TextBlock titleBoard = new TextBlock();
                    Button buttonConfirm = new Button();
                    Button buttonCancel = new Button();
                    SolidColorBrush buttonBackgroundConfirm = new SolidColorBrush { Color = Color.FromRgb(11, 218, 81) };
                    SolidColorBrush buttonBackgroundCancel = new SolidColorBrush { Color = Color.FromRgb(237, 41, 57) };

                    // BORDER & Panel
                    border.Style = System.Windows.Application.Current.Resources["BackgroundBorder"] as Style;
                    border.Height = 35;

                    titleNotify.Orientation = Orientation.Horizontal;
                    titleNotify.HorizontalAlignment = HorizontalAlignment.Left;

                    buttons.Orientation = Orientation.Horizontal;
                    buttons.HorizontalAlignment = HorizontalAlignment.Right;

                    // TEXTBLOCK
                    usernameFrom.FontSize = 14;
                    usernameFrom.Text = item.From;
                    usernameFrom.Foreground = foreground;
                    usernameFrom.FontWeight = FontWeights.Medium;
                    usernameFrom.MaxWidth = 150;
                    usernameFrom.TextTrimming = TextTrimming.WordEllipsis;
                    usernameFrom.HorizontalAlignment = HorizontalAlignment.Left;
                    usernameFrom.Margin = new Thickness(10, 3, 5, 3);

                    titleBoard.FontSize = 14;
                    titleBoard.Text = "Приглашение в доску: " + item.TitleBoard;
                    titleBoard.Foreground = foreground;
                    titleBoard.TextTrimming = TextTrimming.WordEllipsis;
                    titleBoard.HorizontalAlignment = HorizontalAlignment.Left;
                    titleBoard.Margin = new Thickness(10, 3, 5, 3);


                    // BUTTON
                    buttonConfirm.Content = "Принять";
                    buttonConfirm.Foreground = foreground;
                    buttonConfirm.Name = "button_" + index;
                    buttonConfirm.Width = 100;
                    buttonConfirm.Height = 25;
                    buttonConfirm.FontSize = 14;
                    buttonConfirm.FontWeight = FontWeights.Medium;
                    buttonConfirm.Margin = new Thickness(5, 0, 5, 0);
                    buttonConfirm.Background = buttonBackgroundConfirm;
                    buttonConfirm.Style = System.Windows.Application.Current.Resources["ButtonStyle"] as Style;
                    buttonConfirm.HorizontalAlignment = HorizontalAlignment.Right;
                    buttonConfirm.Click += ConfirmUserToBoard;

                    buttonCancel.Content = "Отменить";
                    buttonCancel.Foreground = foreground;
                    buttonCancel.Name = "button_" + index;
                    buttonCancel.Width = 100;
                    buttonCancel.Height = 25;
                    buttonCancel.FontSize = 14;
                    buttonCancel.FontWeight = FontWeights.Medium;
                    buttonCancel.Margin = new Thickness(5, 0, 10, 0);
                    buttonCancel.Background = buttonBackgroundCancel;
                    buttonCancel.Style = System.Windows.Application.Current.Resources["ButtonStyle"] as Style;
                    buttonCancel.HorizontalAlignment = HorizontalAlignment.Right;
                    buttonCancel.Click += CancelUserToBoard;

                    titleNotify.Children.Add(usernameFrom);
                    titleNotify.Children.Add(titleBoard);
                    buttons.Children.Add(buttonConfirm);
                    buttons.Children.Add(buttonCancel);

                    grid.Children.Add(titleNotify);
                    grid.Children.Add(buttons);

                    border.Child = grid;
                    newPanel.Children.Add(border);
                }
            }


            UserNotify = newPanel;
        }

        private void UpdateUserNotify()
        {

        }


        private void CancelUserToBoard(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string[] arrayWordsButton = button.Name.Split(new char[] { '_' });
            int index = Convert.ToInt32(arrayWordsButton[1]);
            DeleteInviteUser(_notifyList[index].User, _notifyList[index].From, _notifyList[index].TitleBoard);
        }

        private void ConfirmUserToBoard(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string[] arrayWordsButton = button.Name.Split(new char[] { '_' });
            //DeleteInviteUser(arrayWordsButton[1], arrayWordsButton[2], arrayWordsButton[3]);
        }

        private async void DeleteInviteUser(string userName, string from, string titleBoard)
        {
            await network.DeleteInivteUser(userName, from, titleBoard);
            await network.DeletePartFromBoard(userName, from, titleBoard);
            GetNotify(userName);
        }
    }
}
