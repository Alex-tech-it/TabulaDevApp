using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TabulaDevApp.Core.Commands;
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    public class KanbanBoardSettingsViewModel : ObservableObject
    {
        public static Dictionary<string, SolidColorBrush> addColors = new Dictionary<string, SolidColorBrush>
        {
            {"Green", new SolidColorBrush{Color = Color.FromRgb(11, 218, 81)}},
            {"Red", new SolidColorBrush{Color = Color.FromRgb(237, 41, 57)}},
            {"White", new SolidColorBrush{Color = Color.FromRgb(244, 245, 245)}},
            {"Purpel", new SolidColorBrush{Color = Color.FromRgb(203, 25, 239)}},
            {"Blue", new SolidColorBrush{Color = Color.FromRgb(48, 128, 222)}},
            {"Orange", new SolidColorBrush{Color = Color.FromRgb(246, 177, 62)}},
            {"Yellow", new SolidColorBrush{Color = Color.FromRgb(249, 234, 55)}},
            {"Turquoise", new SolidColorBrush{Color = Color.FromRgb(106, 231, 203)}},
            {"Gray", new SolidColorBrush{Color = Color.FromRgb(171, 171, 171)}}
        };

        public static Dictionary<string, string> addColorsHEX = new Dictionary<string, string>
        {
            {"Green", "#0BDA51"},
            {"Red", "#ED2939"},
            {"White", "#F4F5F5"},
            {"Purpel", "#CB19EF"},
            {"Blue", "#3080DE"},
            {"Orange", "#F6B13E"},
            {"Yellow", "#F9EA37"},
            {"Turquoise", "#6AE7CB"},
            {"Gray", "#ABABAB"}
        };

        private StackPanel _labelList;
        private StackPanel _createNewLable;
        private KanbanBoardModel _kanbanBoardModel;
        private Border _selectedBorder;
        private SolidColorBrush _storeColor;
        private string _storeColorHex;
        private string _nameNewLabel;
        private bool _isReady;

        public KanbanBoardModel kanbanBoardModel
        {
            get => _kanbanBoardModel;
            set
            {
                _kanbanBoardModel = value;
                OnPropertyChanged();
            }
        }
        public StackPanel CreateNewLabel
        {
            get => _createNewLable;
            set
            {
                _createNewLable = value;
                OnPropertyChanged();
            }
        }
        public Border SelectedBorder
        {
            get => _selectedBorder;
            set
            {
                _selectedBorder = value;
                OnPropertyChanged("SelectedBorder");
            }
        }
        public string NameNewLabel
        {
            get => _nameNewLabel;
            set
            {
                _nameNewLabel = value;
                OnPropertyChanged("NameNewLabel");
            }
        }
        public bool IsReady
        {
            get => _isReady;
            set
            {
                _isReady = value;
                OnPropertyChanged();
            }
        }

        public StackPanel LabelList
        {
            get => _labelList;
            set
            {
                _labelList = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateSaveBoardCommand { get; set; }
        public RelayCommand NavigateExitBoardCommand { get; set; }
        public RelayCommand NavigateDeleteBoardCommand { get; set; }
        public RelayCommand NavigateMembersBoardCommand { get; set; }
        public RelayCommand NavigateChatBoardCommand { get; set; }

        public KanbanBoardSettingsViewModel(NavigationStore navigation, KanbanBoardModel model, NavigationStore upperNavigation, UserModel user)
        {
            kanbanBoardModel = model;
            LabelList = UpdateLabelList();

            StackPanel newPanel = new StackPanel();
            newPanel.Children.Add(CreateDefButton());

            CreateNewLabel = newPanel;
            SelectedBorder = CreateUICurrentBorder();
            IsReady = false;

            NavigateSaveBoardCommand = new RelayCommand(obj =>
            {
                if (kanbanBoardModel.TitleBoard != "")
                {
                    IsReady = false;
                    navigation.UpperViewModel = null;
                    navigation.CurrentViewModel = new KanbanBoardViewModel(navigation, kanbanBoardModel, upperNavigation, user);
                    upperNavigation.CurrentViewModel = new HomeViewModel(upperNavigation, user, navigation);
                }
                else
                {
                    IsReady = true;
                }
            });
            NavigateExitBoardCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new KanbanBoardViewModel(navigation, kanbanBoardModel, upperNavigation, user);
            });
            NavigateDeleteBoardCommand = new RelayCommand(obj =>
            {
                user.userBoards.Remove(kanbanBoardModel);
                navigation.UpperViewModel = null;
                upperNavigation.CurrentViewModel = new HomeViewModel(upperNavigation, user, navigation);
                navigation.CurrentViewModel = new MainPageViewModel(user);
            });

            NavigateMembersBoardCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new KanbanBoardManageMembersViewModel(navigation, model, upperNavigation, user);
            });
            NavigateChatBoardCommand = new RelayCommand(obj =>
            {
                navigation.UpperViewModel = null;
                navigation.CurrentViewModel = new ChatViewModel(navigation, model, upperNavigation, user);
            });
        }

        private StackPanel UpdateLabelList()
        {
            StackPanel newPanel = new StackPanel();
            newPanel.Orientation = Orientation.Horizontal;
            newPanel.HorizontalAlignment = HorizontalAlignment.Left;

            foreach(LabelData label in kanbanBoardModel.LabelList)
            {
                Button borderLabel = new Button();
                SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };


                borderLabel.Content = label.Description;
                borderLabel.Width = 60;
                borderLabel.Height = 25;
                borderLabel.FontSize = 13;
                borderLabel.FontWeight = FontWeights.Medium;
                borderLabel.Foreground = foreground;
                borderLabel.Margin = new Thickness(0, 0, 5, 0);
                SolidColorBrush backgroundLabel = (SolidColorBrush)new BrushConverter().ConvertFrom(label.Color);
                borderLabel.Background = backgroundLabel;
                borderLabel.Style = Application.Current.Resources["ButtonStyle"] as Style;

                newPanel.Children.Add(borderLabel);
            }

            return newPanel;
        }

        private Button CreateDefButton()
        {
            Button addLabelButton = new Button();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            SelectedBorder = CreateUICurrentBorder();

            addLabelButton.Content = "+ Добавить метку";
            addLabelButton.FontSize = 12;
            addLabelButton.FontWeight = FontWeights.Medium;
            addLabelButton.Foreground = foreground;
            addLabelButton.Cursor = Cursors.Hand;
            addLabelButton.Style = Application.Current.Resources["PreviewNavigationButtonStyle"] as Style;
            addLabelButton.Click += CreateUIAddPanel;

            return addLabelButton;
        }

        private void CreateUIAddPanel(object sender, RoutedEventArgs e)
        {
            CreateUIAddPanel();
        }
        
        private void CreateUIAddPanel()
        {
            StackPanel general = new StackPanel();

            StackPanel addPanel = new StackPanel();
            addPanel.Orientation = Orientation.Horizontal;

            StackPanel colorsPanel = new StackPanel();
            colorsPanel.VerticalAlignment = VerticalAlignment.Center;
            colorsPanel.Margin = new Thickness(0, 0, 30, 0);
            colorsPanel.Width = 85;
            colorsPanel.Height = 85;

            // ### Block 1 ###
            StackPanel firstRow = new StackPanel();
            firstRow.Orientation = Orientation.Horizontal;

            Border greenButton = new Border();
            greenButton.Width = 25;
            greenButton.Height = 25;
            greenButton.Margin = new Thickness(0, 0, 5, 5);
            greenButton.CornerRadius = new CornerRadius(5);
            greenButton.Background = addColors["Green"];
            greenButton.Name = "Green";
            greenButton.MouseDown += AddLabelColor;

            Border redButton = new Border();
            redButton.Width = 25;
            redButton.Height = 25;
            redButton.Margin = new Thickness(0, 0, 5, 5);
            redButton.CornerRadius = new CornerRadius(5);
            redButton.Background = addColors["Red"];
            redButton.Name = "Red";
            redButton.MouseDown += AddLabelColor;

            Border whiteButton = new Border();
            whiteButton.Width = 25;
            whiteButton.Height = 25;
            whiteButton.Margin = new Thickness(0, 0, 0, 5);
            whiteButton.CornerRadius = new CornerRadius(5);
            whiteButton.Background = addColors["White"];
            whiteButton.Name = "White";
            whiteButton.MouseDown += AddLabelColor;

            firstRow.Children.Add(greenButton);
            firstRow.Children.Add(redButton);
            firstRow.Children.Add(whiteButton);

            // ### Block 2 ##

            StackPanel secondRow = new StackPanel();
            secondRow.Orientation = Orientation.Horizontal;

            Border purpelButton = new Border();
            purpelButton.Width = 25;
            purpelButton.Height = 25;
            purpelButton.Margin = new Thickness(0, 0, 5, 5);
            purpelButton.CornerRadius = new CornerRadius(5);
            purpelButton.Background = addColors["Purpel"];
            purpelButton.Name = "Purpel";
            purpelButton.MouseDown += AddLabelColor;

            Border blueButton = new Border();
            blueButton.Width = 25;
            blueButton.Height = 25;
            blueButton.Margin = new Thickness(0, 0, 5, 5);
            blueButton.CornerRadius = new CornerRadius(5);
            blueButton.Background = addColors["Blue"];
            blueButton.Name = "Blue";
            blueButton.MouseDown += AddLabelColor;

            Border orangeButton = new Border();
            orangeButton.Width = 25;
            orangeButton.Height = 25;
            orangeButton.Margin = new Thickness(0, 0, 0, 5);
            orangeButton.CornerRadius = new CornerRadius(5);
            orangeButton.Background = addColors["Orange"];
            orangeButton.Name = "Orange";
            orangeButton.MouseDown += AddLabelColor;

            secondRow.Children.Add(purpelButton);
            secondRow.Children.Add(blueButton);
            secondRow.Children.Add(orangeButton);

            // ### Block 3 ###
            StackPanel thierdRow = new StackPanel();
            thierdRow.Orientation = Orientation.Horizontal;

            Border yellowButton = new Border();
            yellowButton.Width = 25;
            yellowButton.Height = 25;
            yellowButton.Margin = new Thickness(0, 0, 5, 0);
            yellowButton.CornerRadius = new CornerRadius(5);
            yellowButton.Background = addColors["Yellow"];
            yellowButton.Name = "Yellow";
            yellowButton.MouseDown += AddLabelColor;

            Border turquoiseButton = new Border();
            turquoiseButton.Width = 25;
            turquoiseButton.Height = 25;
            turquoiseButton.Margin = new Thickness(0, 0, 5, 0);
            turquoiseButton.CornerRadius = new CornerRadius(5);
            turquoiseButton.Background = addColors["Turquoise"];
            turquoiseButton.Name = "Turquoise";
            turquoiseButton.MouseDown += AddLabelColor;

            Border grayButton = new Border();
            grayButton.Width = 25;
            grayButton.Height = 25;
            grayButton.CornerRadius = new CornerRadius(5);
            grayButton.Background = addColors["Gray"];
            grayButton.Name = "Gray";
            grayButton.MouseDown += AddLabelColor;

            thierdRow.Children.Add(yellowButton);
            thierdRow.Children.Add(turquoiseButton);
            thierdRow.Children.Add(grayButton);

            colorsPanel.Children.Add(firstRow);
            colorsPanel.Children.Add(secondRow);
            colorsPanel.Children.Add(thierdRow);

            // View Label
            StackPanel labelPanel = new StackPanel();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            labelPanel.Orientation = Orientation.Horizontal;
            labelPanel.HorizontalAlignment = HorizontalAlignment.Center;
            labelPanel.VerticalAlignment = VerticalAlignment.Center;

            TextBox nameLabel = new TextBox();
            nameLabel.Text = NameNewLabel;
            nameLabel.FontSize = 14;
            nameLabel.MinWidth = 220;
            nameLabel.MaxWidth = 220;
            nameLabel.Padding = new Thickness(5);
            nameLabel.TextAlignment = TextAlignment.Left;
            nameLabel.Foreground = foreground;
            nameLabel.Style = Application.Current.Resources["AddCardTextBoxStyle"] as Style;
            nameLabel.TextChanged += textLabelChanged;

            labelPanel.Children.Add(SelectedBorder);
            labelPanel.Children.Add(nameLabel);

            // Buttons
            StackPanel buttonsPanel = new StackPanel();
            buttonsPanel.Orientation = Orientation.Horizontal;
            buttonsPanel.HorizontalAlignment = HorizontalAlignment.Right;

            Button creatSaveLabel = new Button();
            Button creatCancelLabel = new Button();
            SolidColorBrush background_save = new SolidColorBrush { Color = Color.FromRgb(61, 0, 123) };
            SolidColorBrush background_cancel = new SolidColorBrush { Color = Color.FromRgb(85, 85, 98) };

            creatSaveLabel.Content = "Добавить";
            creatSaveLabel.Width = 80;
            creatSaveLabel.Height = 25;
            creatSaveLabel.FontSize = 13;
            creatSaveLabel.FontWeight = FontWeights.Medium;
            creatSaveLabel.Foreground = foreground;
            creatSaveLabel.Margin = new Thickness(0, 0, 5, 0);
            creatSaveLabel.Background = background_save;
            creatSaveLabel.Style = Application.Current.Resources["ButtonStyle"] as Style;
            creatSaveLabel.Click += AddAndSaveLabel;

            creatCancelLabel.Content = "Отмена";
            creatCancelLabel.Width = 80;
            creatCancelLabel.Height = 25;
            creatCancelLabel.FontSize = 13;
            creatCancelLabel.FontWeight = FontWeights.Medium;
            creatCancelLabel.Foreground = foreground;
            creatCancelLabel.Margin = new Thickness(0, 0, 5, 0);
            creatCancelLabel.Background = background_cancel;
            creatCancelLabel.Style = Application.Current.Resources["ButtonStyle"] as Style;
            creatCancelLabel.Click += CancelAddLabel;

            buttonsPanel.Children.Add(creatSaveLabel);
            buttonsPanel.Children.Add(creatCancelLabel);

            addPanel.Children.Add(colorsPanel);
            addPanel.Children.Add(labelPanel);

            general.Children.Add(addPanel);
            general.Children.Add(buttonsPanel);

            CreateNewLabel = general;
        }

        private void AddLabelColor(object sender, RoutedEventArgs e)
        {
            Border colorButton = (Border)sender;
            SolidColorBrush color = (colorButton.Name != "") ? addColors[colorButton.Name] : addColors["White"];
            _storeColor = color;
            _storeColorHex = (colorButton.Name != "") ? addColorsHEX[colorButton.Name] : addColorsHEX["White"];
            SelectedBorder = CreateUICurrentBorder(color);
            CreateUIAddPanel();
        }

        private void AddAndSaveLabel(object sender, RoutedEventArgs e)
        {
            LabelData newLabel = new LabelData();
            newLabel.Color = _storeColorHex;
            newLabel.Description = NameNewLabel;

            bool focuseFlag = true;
            foreach(LabelData item in kanbanBoardModel.LabelList)
            {
                if(item.Color == newLabel.Color && item.Description == newLabel.Description)
                {
                    focuseFlag = false;
                }
            }

            if (focuseFlag)
            {
                kanbanBoardModel.LabelList.Add(newLabel);
            }

            StackPanel newPanel = new StackPanel();
            newPanel.Children.Add(CreateDefButton());

            _storeColor = null;
            _storeColorHex = null;
            NameNewLabel = "";
            CreateNewLabel = newPanel;
            LabelList = UpdateLabelList();
        }

        private void CancelAddLabel(object sender, RoutedEventArgs e)
        {
            StackPanel newPanel = new StackPanel();
            newPanel.Children.Add(CreateDefButton());

            _storeColor = null;
            _storeColorHex = null;
            NameNewLabel = "";
            CreateNewLabel = newPanel;
        }

        private Border CreateUICurrentBorder(SolidColorBrush brush = null)
        {
            Border colorLabel = new Border();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };

            colorLabel.Margin = new Thickness(0, 0, 10, 0);
            colorLabel.Width = 60;
            colorLabel.Height = 25;
            colorLabel.CornerRadius = new CornerRadius(10);
            colorLabel.Background = (brush != null) ? brush : Brushes.Transparent;
            colorLabel.BorderThickness = new Thickness(2);
            colorLabel.BorderBrush = Brushes.Black;

            return colorLabel;
        }

        private void textLabelChanged(object sender, TextChangedEventArgs args)
        {
            TextBox box = (TextBox)sender;
            NameNewLabel = box.Text;
        }
    }
}
