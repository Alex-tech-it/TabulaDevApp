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
    class CardViewModel : ObservableObject
    {
        private Card _cardModel;
        private bool _isReady;
        private StackPanel _labelList;
        private StackPanel _addNewLabel;
        private KanbanBoardModel _kanbanBoardModel;
        private ObservableCollection<LabelData> _labelCollection;
        
        public bool IsReady
        {
            get => _isReady;
            set
            {
                _isReady = value;
                OnPropertyChanged();
            }
        }
        public Card CardModel
        {
            get => _cardModel;
            set
            {
                _cardModel = value;
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
        public KanbanBoardModel kanbanBoardModel
        {
            get => _kanbanBoardModel;
            set
            {
                _kanbanBoardModel = value;
                OnPropertyChanged();
            }
        }
        public StackPanel AddNewLabel
        {
            get => _addNewLabel;
            set
            {
                _addNewLabel = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateSaveCardCommand { get; set; }
        public RelayCommand NavigateExitCardCommand { get; set; }
        public CardViewModel(NavigationStore navigationStore, KanbanBoardModel model,
            int indexColumn, int indexCard, ObservableCollection<KanbanBoardModel> thisListBoards, NavigationStore upperNavigation)
        {
            IsReady = false;
            CardModel = model.Lists[indexColumn].Cards[indexCard];
            _labelCollection = CardModel.CardLabelList;
            kanbanBoardModel = model;

            StackPanel newPanel = new StackPanel();
            newPanel.Children.Add(CreateDefButton());
            LabelList = UpdateLabelList();

            AddNewLabel = newPanel;


            NavigateSaveCardCommand = new RelayCommand(obj =>
            {
                if (CardModel.Title != "")
                {
                    IsReady = false;
                    model.Lists[indexColumn].Cards[indexCard] = CardModel;
                    navigationStore.UpperViewModel = null;
                    navigationStore.CurrentViewModel = new KanbanBoardViewModel(navigationStore, model, outListBoards: thisListBoards, upperNavigation);
                }
                else
                {
                    IsReady = true;
                }
            });
            NavigateExitCardCommand = new RelayCommand(obj =>
            {
                navigationStore.UpperViewModel = null;
            });
        }

        private StackPanel UpdateLabelList()
        {
            StackPanel newPanel = new StackPanel();
            newPanel.Orientation = Orientation.Horizontal;
            newPanel.HorizontalAlignment = HorizontalAlignment.Left;

            foreach (LabelData label in _labelCollection)
            {
                Button borderLabel = new Button();
                SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };

                borderLabel.Content = label.Description;
                borderLabel.Width = 50;
                borderLabel.Height = 20;
                borderLabel.FontSize = 13;
                borderLabel.FontWeight = FontWeights.Medium;
                borderLabel.Foreground = foreground;
                borderLabel.Margin = new Thickness(0, 0, 5, 0);
                borderLabel.Background = label.Color;
                borderLabel.Style = Application.Current.Resources["ButtonStyle"] as Style;

                newPanel.Children.Add(borderLabel);
            }

            return newPanel;
        }

        private Button CreateDefButton()
        {
            Button addLabelButton = new Button();
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };

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
            StackPanel newPanel = new StackPanel();
            WrapPanel general = new WrapPanel();
            general.MaxWidth = 405;
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };

            TextBlock textBlockTitle = new TextBlock();
            textBlockTitle.FontSize = 14;
            textBlockTitle.Foreground = foreground;
            textBlockTitle.Text = "Выберите мeтки:";
            textBlockTitle.Margin = new Thickness(0, 5, 0, 5);
            textBlockTitle.FontWeight = FontWeights.Medium;
            textBlockTitle.HorizontalAlignment = HorizontalAlignment.Left;

            if (kanbanBoardModel.LabelList.Count != 0)
            {
                for (int i = 0; i < kanbanBoardModel.LabelList.Count; i++)
                {
                    Button borderLabel = new Button();

                    borderLabel.Name = "Label_" + Convert.ToString(i);
                    borderLabel.Content = kanbanBoardModel.LabelList[i].Description;
                    borderLabel.Width = 50;
                    borderLabel.Height = 20;
                    borderLabel.FontSize = 13;
                    borderLabel.FontWeight = FontWeights.Medium;
                    borderLabel.Foreground = foreground;
                    borderLabel.Margin = new Thickness(0, 5, 5, 0);
                    borderLabel.Background = kanbanBoardModel.LabelList[i].Color;
                    borderLabel.Style = Application.Current.Resources["ButtonStyle"] as Style;
                    borderLabel.Click += AddLabel;

                    general.Children.Add(borderLabel);
                }
            }
            else
            {
                TextBlock textDescription = new TextBlock();
                textDescription.FontSize = 12;
                textDescription.Foreground = Brushes.Gray;
                textDescription.Margin = new Thickness(5, 5, 5, 3);
                textDescription.Text = "На данной доске не было созданно ни одной метки...";
                textDescription.HorizontalAlignment = HorizontalAlignment.Left;

                general.Children.Add(textDescription);
            }

            Button creatSaveLabel = new Button();
            SolidColorBrush background_save = new SolidColorBrush { Color = Color.FromRgb(85, 85, 98) };
            creatSaveLabel.Content = "Окей";
            creatSaveLabel.Width = 60;
            creatSaveLabel.Height = 25;
            creatSaveLabel.FontSize = 13;
            creatSaveLabel.FontWeight = FontWeights.Medium;
            creatSaveLabel.Foreground = foreground;
            creatSaveLabel.Margin = new Thickness(0, 15, 0, 0);
            creatSaveLabel.Background = background_save;
            creatSaveLabel.Style = Application.Current.Resources["ButtonStyle"] as Style;
            creatSaveLabel.HorizontalAlignment = HorizontalAlignment.Left;
            creatSaveLabel.Click += AddAndSaveLabel;


            newPanel.MaxHeight = 100;
            newPanel.MaxWidth = 330;
            newPanel.Children.Add(textBlockTitle);
            newPanel.Children.Add(general);
            newPanel.Children.Add(creatSaveLabel);

            AddNewLabel = newPanel;
        }

        private void AddAndSaveLabel(object sender, RoutedEventArgs e)
        {
            StackPanel newPanel = new StackPanel();
            newPanel.Children.Add(CreateDefButton());
            AddNewLabel = newPanel;
        }

        private void AddLabel(object sender, RoutedEventArgs e)
        {
            Button label = (Button)sender;
            string[] arrayWordsButton = label.Name.Split(new char[] { '_' });
            int labelIndex = Convert.ToInt32(arrayWordsButton[1]);

            bool focuseFlag = true;
            foreach (LabelData item in _labelCollection)
            {
                if (item.Color == kanbanBoardModel.LabelList[labelIndex].Color && item.Description == kanbanBoardModel.LabelList[labelIndex].Description)
                {
                    focuseFlag = false;
                }
            }

            if (focuseFlag)
            {
                _labelCollection.Add(kanbanBoardModel.LabelList[labelIndex]);
            }


            LabelList = UpdateLabelList();
        }
    }
}
