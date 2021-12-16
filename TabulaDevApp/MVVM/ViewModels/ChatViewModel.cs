using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Network;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class ChatViewModel : ObservableObject
    {
        private UserNetwork network;
        private UserModel _user;
        private KanbanBoardModel _kanbanBoardModel;
        private string _message;
        private StackPanel _chatMessages;

        // Commands
        public RelayCommand NavigateSettingsBoardCommand { get; set; }
        public RelayCommand NavigateMembersBoardCommand { get; set; }
        public RelayCommand PushMessageCommand { get; set; }

        // Getter & Setter
        public KanbanBoardModel kanbanBoardModel
        {
            get => _kanbanBoardModel;
            set
            {
                _kanbanBoardModel = value;
                OnPropertyChanged();
            }
        }
        public string Message
        {
            get => _message;
            set
            {
                _message = value;
                OnPropertyChanged();
            }
        }
        public StackPanel ChatMessages
        {
            get => _chatMessages;
            set
            {
                _chatMessages = value;
                OnPropertyChanged();
            }
        }

        public FontWeight FOntWeights { get; private set; }

        public ChatViewModel(NavigationStore navigation, KanbanBoardModel model, NavigationStore upperNavigation, UserModel user)
        {
            kanbanBoardModel = model;
            _user = user;
            network = new UserNetwork();
            UpdateMessages();

            // Inits Commands
            NavigateSettingsBoardCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new KanbanBoardSettingsViewModel(navigation, model, upperNavigation, user);
            });
            NavigateMembersBoardCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new KanbanBoardManageMembersViewModel(navigation, model, upperNavigation, user);
            });
            PushMessageCommand = new RelayCommand(obj =>
            {
                PushMessage(Message);
                Message = "";
            });
            Thread ConnectionScan = new Thread(ConnectionServer) { IsBackground = true };
            ConnectionScan.Start();
        }
        private async void PushMessage(string message)
        {
            ChatModel newMessage = new ChatModel();
            newMessage.User = _user.Username;
            newMessage.Date = DateTime.Now.ToString();
            newMessage.Text = message;
            await network.PushMessage(newMessage, _kanbanBoardModel.ChatId);
        }

        private async void UpdateMessages()
        {
            ObservableCollection<ChatModel> messages = new ObservableCollection<ChatModel>();
            messages = await network.GetMessages(_kanbanBoardModel.ChatId);

            Application.Current.Dispatcher.Invoke(() =>
            {
                StackPanel newPanel = new StackPanel();

                // UI INIT
                newPanel.Orientation = Orientation.Vertical;
                SolidColorBrush memberColor = new SolidColorBrush { Color = Color.FromRgb(57, 58, 65) };
                SolidColorBrush userColor = new SolidColorBrush { Color = Color.FromRgb(255, 165, 0) };
                SolidColorBrush userColorOwner = new SolidColorBrush { Color = Color.FromRgb(103, 61, 255) };

                foreach (ChatModel model in messages)
                {
                    StackPanel layMessage = new StackPanel();
                    layMessage.Orientation = Orientation.Horizontal;
                    layMessage.Margin = new Thickness(0, 5, 0, 5);

                    StackPanel layText = new StackPanel();
                    layText.Orientation = Orientation.Vertical;

                    Border border = new Border();
                    border.MaxWidth = 285;
                    border.CornerRadius = new System.Windows.CornerRadius(10);
                    border.Background = memberColor;
                    border.Margin = new Thickness(5, 5, 5, 5);

                    Ellipse ellipse = new Ellipse();
                    ellipse.Width = 20;
                    ellipse.Height = 20;
                    ellipse.Fill = Brushes.LightGreen;
                    ellipse.Margin = new Thickness(5, 0, 10, 5);
                    ellipse.VerticalAlignment = VerticalAlignment.Bottom;

                    TextBlock userName = new TextBlock();
                    userName.Text = model.User;
                    userName.FontSize = 11;
                    userName.FontWeight = FontWeights.Medium;
                    userName.HorizontalAlignment = HorizontalAlignment.Left;
                    userName.Margin = new Thickness(5, 3, 5, 3);
                    userName.Foreground = userColor;

                    TextBlock userTextMessage = new TextBlock();
                    userTextMessage.Text = model.Text;
                    userTextMessage.FontSize = 12;
                    userTextMessage.HorizontalAlignment = HorizontalAlignment.Left;
                    userTextMessage.MaxWidth = 285;
                    userTextMessage.TextWrapping = TextWrapping.Wrap;
                    userTextMessage.Foreground = Brushes.White;
                    userTextMessage.Margin = new Thickness(5, 0, 5, 0);

                    TextBlock userDateMessage = new TextBlock();
                    userDateMessage.Text = model.Date;
                    userDateMessage.FontSize = 9;
                    userDateMessage.Foreground = Brushes.Gray;
                    userDateMessage.HorizontalAlignment = HorizontalAlignment.Right;
                    userDateMessage.Margin = new Thickness(5, 3, 10, 0);

                    if (_user.Username == model.User)
                    {
                        border.Background = userColorOwner;
                        userName.Foreground = Brushes.White;
                        userDateMessage.Foreground = Brushes.White;
                    }
                
                    layText.Children.Add(userName);
                    layText.Children.Add(userTextMessage);
                    layText.Children.Add(userDateMessage);

                    border.Child = layText;

                    if (_user.Username == model.User)
                    {
                        border.HorizontalAlignment = HorizontalAlignment.Right;
                        ellipse.Fill = Brushes.LightBlue;
                        layMessage.Children.Add(border);
                        layMessage.Children.Add(ellipse);
                        layMessage.HorizontalAlignment = HorizontalAlignment.Right;
                    }
                    else
                    {
                        layMessage.Children.Add(ellipse);
                        layMessage.Children.Add(border);
                    }
                

                    newPanel.Children.Add(layMessage);
                }

                ChatMessages = newPanel;
            });
        }

        private void ConnectionServer()
        {
            while (true)
            {
                UpdateMessages();
                Thread.Sleep(1000);
            }
        }
    }
}
