using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Network;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class KanbanBoardManageMembersViewModel : ObservableObject
    {
        private UserNetwork network;
        private UserModel _user;
        private ObservableCollection<string> userSearch;
        private KanbanBoardModel _kanbanBoardModel;
        private StackPanel _searchUsers;
        private StackPanel _currentPart;
        private TextBox _textBox;

        public RelayCommand NavigateToBoardCommand { get; set; }
        public RelayCommand NavigateSettingsCommand { get; set; }
        public RelayCommand NavigateChatBoardCommand { get; set; }

        public TextBox TextBoxSearch
        {
            get => _textBox;
            set
            {
                _textBox = value;
                OnPropertyChanged();
            }
        }
        public StackPanel SearchUsers
        {
            get => _searchUsers;
            set
            {
                _searchUsers = value;
                OnPropertyChanged();
            }
        }

        public StackPanel CurrrentPart
        {
            get => _currentPart;
            set
            {
                _currentPart = value;
                OnPropertyChanged();
            }
        }

        public KanbanBoardModel kanbanBoardModel
        {
            get => _kanbanBoardModel;
            set
            {
                _kanbanBoardModel = value;
                OnPropertyChanged();
            }
        }

        public KanbanBoardManageMembersViewModel(NavigationStore navigation, KanbanBoardModel model, NavigationStore upperNavigation, UserModel user)
        {
            kanbanBoardModel = model;
            _user = user;
            network = new UserNetwork();
            CreatUITextBoxSearch();
            UpdatePart();

            NavigateToBoardCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new KanbanBoardViewModel(navigation, kanbanBoardModel, upperNavigation, _user);
            });

            NavigateSettingsCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new KanbanBoardSettingsViewModel(navigation, kanbanBoardModel, upperNavigation, _user);
            });
            NavigateChatBoardCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new ChatViewModel(navigation, model, upperNavigation, user);
            });
        }

        private async void FindUser(string userName, string owner)
        {
            userSearch = new ObservableCollection<string>();
            userSearch = await network.FindUsers(userName, owner);
            foreach(string item in kanbanBoardModel.Participants)
            {
                if (userSearch.Contains(item))
                {
                    userSearch.Remove(item);
                }
            }
            StackPanel newPanel = new StackPanel();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            if (userSearch.Count == 0)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;
                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                textBlock.Text = "Такого пользователя не существует!";
                textBlock.Foreground = foreground;
                newPanel.Children.Add(textBlock);
            }
            else
            {
                foreach (string item in userSearch)
                {
                    string itemName = item.Replace("@", "");
                    Console.WriteLine(itemName);
                    // Init Layouts
                    Border border = new Border();
                    Grid grid = new Grid();
                    
                    // Init UI Elements
                    TextBlock username = new TextBlock();
                    Button buttonAdd = new Button();
                    SolidColorBrush buttonBackground = new SolidColorBrush { Color = Color.FromRgb(11, 218, 81) };

                    // BORDER
                    border.Style = System.Windows.Application.Current.Resources["BackgroundBorder"] as Style;
                    border.Height = 35;

                    // TEXTBLOCK
                    username.FontSize = 14;
                    username.Text = item;
                    username.Foreground = foreground;
                    username.FontWeight = FontWeights.Medium;
                    username.MaxWidth = 250;
                    username.TextTrimming = TextTrimming.WordEllipsis;
                    username.Name = "textBloc_" + itemName;
                    username.HorizontalAlignment = HorizontalAlignment.Left;
                    username.Margin = new Thickness(10, 3, 5, 3);

                    // BUTTON
                    buttonAdd.Content = "Добавить";
                    buttonAdd.Foreground = foreground;
                    buttonAdd.Name = "button_" + itemName; ;
                    buttonAdd.Width = 100;
                    buttonAdd.Height = 25;
                    buttonAdd.FontSize = 14;
                    buttonAdd.FontWeight = FontWeights.Medium;
                    buttonAdd.Margin = new Thickness(5, 0, 10, 0);
                    buttonAdd.Background = buttonBackground;
                    buttonAdd.Style = System.Windows.Application.Current.Resources["ButtonStyle"] as Style;
                    buttonAdd.HorizontalAlignment = HorizontalAlignment.Right;
                    buttonAdd.Click += InviteUserToBoard;


                    grid.Children.Add(username);
                    grid.Children.Add(buttonAdd);

                    border.Child = grid;
                    newPanel.Children.Add(border);
                }
            }
            SearchUsers = newPanel;
        }

        private async void InviteUser(string userName, string from)
        {
            await network.InviteUser(userName, from, kanbanBoardModel.TitleBoard);
        }

        private async void DeleteInviteUser(string userName, string from)
        {
            await network.DeleteInivteUser(userName, from, kanbanBoardModel.TitleBoard);
        }

        private void CreatUITextBoxSearch()
        {
            //TextChanged = "{Binding TextBox_TextChanged}"

            TextBox textBox = new TextBox();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            textBox.FontSize = 14;
            textBox.MinWidth = 405;
            textBox.MaxWidth = 405;
            textBox.HorizontalAlignment = HorizontalAlignment.Left;
            textBox.TextWrapping = TextWrapping.NoWrap;
            textBox.TextAlignment = TextAlignment.Left;
            textBox.Style = System.Windows.Application.Current.Resources["AddCardTextBoxStyle"] as Style;
            textBox.Foreground = foreground;
            textBox.CaretBrush = foreground;
            textBox.TextChanged += TextBox_TextChanged;
            textBox.Padding = new Thickness(5);
            TextBoxSearch = textBox;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            System.Windows.Application.Current.Dispatcher.Invoke(() =>
            {
                TextBox text = (TextBox)sender;
                if(text.Text != ""){
                    FindUser(text.Text.ToString(), _user.Username);
                }
                else
                {
                    SearchUsers = new StackPanel();
                }
            });

        }

        private void UpdateSearchUserPanel(bool flag = true)
        {
            StackPanel newPanel = new StackPanel();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            if (userSearch.Count == 0)
            {
                TextBlock textBlock = new TextBlock();
                textBlock.FontSize = 12;
                textBlock.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                textBlock.Text = "Такого пользователя не существует!";
                textBlock.Foreground = foreground;
                newPanel.Children.Add(textBlock);
            }
            else
            {
                foreach (string item in userSearch)
                {
                    string itemName = item.Replace("@", "");

                    // Init Layouts
                    Border border = new Border();
                    Grid grid = new Grid();

                    // Init UI Elements
                    TextBlock username = new TextBlock();
                    Button buttonAdd = new Button();
                    SolidColorBrush buttonBackground = new SolidColorBrush { Color = Color.FromRgb(11, 218, 81) };

                    // BORDER
                    border.Style = System.Windows.Application.Current.Resources["BackgroundBorder"] as Style;

                    // TEXTBLOCK
                    username.FontSize = 14;
                    username.Text = item;
                    username.Foreground = foreground;
                    username.FontWeight = FontWeights.Bold;
                    username.MaxWidth = 250;
                    username.TextTrimming = TextTrimming.WordEllipsis;
                    username.Name = "textBloc_" + itemName;
                    username.HorizontalAlignment = HorizontalAlignment.Left;
                    username.Margin = new Thickness(10, 3, 5, 3);

                    // BUTTON
                    buttonAdd.Content = "Добавить";
                    buttonAdd.Foreground = foreground;
                    buttonAdd.Name = "button_" + itemName; ;
                    buttonAdd.Width = 100;
                    buttonAdd.Height = 25;
                    buttonAdd.FontSize = 14;
                    buttonAdd.FontWeight = FontWeights.Medium;
                    buttonAdd.Margin = new Thickness(5, 0, 10, 0);
                    buttonAdd.Background = buttonBackground;
                    buttonAdd.Style = System.Windows.Application.Current.Resources["ButtonStyle"] as Style;
                    buttonAdd.HorizontalAlignment = HorizontalAlignment.Right;
                    buttonAdd.Click += InviteUserToBoard;
                    if (!flag && kanbanBoardModel.Participants.Contains(item))
                    {
                        buttonAdd.Visibility = Visibility.Hidden;
                    }


                    grid.Children.Add(username);
                    grid.Children.Add(buttonAdd);

                    border.Child = grid;
                    newPanel.Children.Add(border);
                }
            }
            SearchUsers = newPanel;
        }

        private void UpdatePart(bool flag = true)
        {
            StackPanel newPanel = new StackPanel();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            SolidColorBrush foregroundOwner = new SolidColorBrush { Color = Color.FromRgb(11, 218, 81) };

            // Init Layouts
            Border borderUser = new Border();
            Grid gridUser = new Grid();

            // Init UI Elements
            TextBlock usernameUser = new TextBlock();
            TextBlock usernameOwner = new TextBlock();
            SolidColorBrush buttonBackgroundUser = new SolidColorBrush { Color = Color.FromRgb(237, 41, 57) };

            // BORDER
            borderUser.Style = System.Windows.Application.Current.Resources["BackgroundBorder"] as Style;
            borderUser.Margin = new Thickness(0, 5, 0, 5);

            // TEXTBLOCK
            usernameUser.FontSize = 14;
            usernameUser.Text = "@" + _user.Username;
            usernameUser.Foreground = foreground;
            usernameUser.FontWeight = FontWeights.Bold;
            usernameUser.MaxWidth = 250;
            usernameUser.TextTrimming = TextTrimming.WordEllipsis;
            usernameUser.Name = "textBloc_" + _user.Username;
            usernameUser.HorizontalAlignment = HorizontalAlignment.Left;
            usernameUser.Margin = new Thickness(10, 3, 5, 3);

            // BUTTON
            usernameOwner.FontSize = 14;
            usernameOwner.Text = "Owner";
            usernameOwner.Foreground = foregroundOwner;
            usernameOwner.FontWeight = FontWeights.Bold;
            usernameOwner.MaxWidth = 250;
            usernameOwner.TextTrimming = TextTrimming.WordEllipsis;
            usernameOwner.HorizontalAlignment = HorizontalAlignment.Right;
            usernameOwner.Margin = new Thickness(5, 3, 10, 3);


            gridUser.Children.Add(usernameUser);
            gridUser.Children.Add(usernameOwner);

            borderUser.Child = gridUser;
            newPanel.Children.Add(borderUser);
            

            foreach (string item in kanbanBoardModel.Participants)
            {
                string itemName = item.Replace("@", "");

                // Init Layouts
                Border border = new Border();
                Grid grid = new Grid();

                // Init UI Elements
                TextBlock username = new TextBlock();
                Button buttonAdd = new Button();
                SolidColorBrush buttonBackground = new SolidColorBrush { Color = Color.FromRgb(237, 41, 57) };

                // BORDER
                border.Style = System.Windows.Application.Current.Resources["BackgroundBorder"] as Style;
                border.Margin = new Thickness(0, 5, 0, 5);

                // TEXTBLOCK
                username.FontSize = 14;
                username.Text = item;
                username.Foreground = foreground;
                username.FontWeight = FontWeights.Bold;
                username.MaxWidth = 250;
                username.TextTrimming = TextTrimming.WordEllipsis;
                username.Name = "textBloc_" + itemName;
                username.HorizontalAlignment = HorizontalAlignment.Left;
                username.Margin = new Thickness(10, 3, 5, 3);

                // BUTTON
                buttonAdd.Content = "Удалить";
                buttonAdd.Foreground = foreground;
                buttonAdd.Name = "button_" + itemName; ;
                buttonAdd.Width = 100;
                buttonAdd.Height = 25;
                buttonAdd.FontSize = 14;
                buttonAdd.FontWeight = FontWeights.Medium;
                buttonAdd.Margin = new Thickness(5, 3, 10, 3);
                buttonAdd.Background = buttonBackground;
                buttonAdd.Style = System.Windows.Application.Current.Resources["ButtonStyle"] as Style;
                buttonAdd.HorizontalAlignment = HorizontalAlignment.Right;
                buttonAdd.Click += DeleteUserToBoard;
                if (!flag)
                {
                    buttonAdd.Visibility = Visibility.Hidden;
                }


                grid.Children.Add(username);
                grid.Children.Add(buttonAdd);

                border.Child = grid;
                newPanel.Children.Add(border);
            }
            CurrrentPart = newPanel;
        }

        private void InviteUserToBoard(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string[] arrayWordsButton = button.Name.Split(new char[] { '_' });
            kanbanBoardModel.Participants.Add("@" + arrayWordsButton[1]);
            UpdateSearchUserPanel(flag: false);
            UpdatePart();
            InviteUser("@" + arrayWordsButton[1], "@" + _user.Username);
        }

        private void DeleteUserToBoard(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string[] arrayWordsButton = button.Name.Split(new char[] { '_' });
            kanbanBoardModel.Participants.Remove("@" + arrayWordsButton[1]);
            UpdatePart();
            DeleteInviteUser("@" + arrayWordsButton[1], "@" + _user.Username);
        }
        

    }
}
