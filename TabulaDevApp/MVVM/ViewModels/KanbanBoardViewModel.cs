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
using TabulaDevApp.Core.Servies;
using TabulaDevApp.MVVM.Models;

namespace TabulaDevApp.MVVM.ViewModels
{
    class KanbanBoardViewModel : ObservableObject
    {
        private StackPanel _currentStackPanel;
        private KanbanBoardModel _kanbanBoardModel;
        public StackPanel CurrentStackPanel
        {
            get => _currentStackPanel;
            set
            {
                _currentStackPanel = value;
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

        public KanbanBoardViewModel()
        {
            kanbanBoardModel = new KanbanBoardModel();
            StartUpGrid();
        }
        public KanbanBoardViewModel(KanbanBoardModel kanbanBoardModel)
        {
        }

        // Init prefered Grid
        private void StartUpGrid()
        {
            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Orientation = Orientation.Horizontal;
            newStackPanel.Children.Add(CreateNewListButton());

            CurrentStackPanel = newStackPanel;
        }

        // Created Button that show's on leff site grid
        private Button CreateNewListButton()
        {
            Button newButtonCreateNewList = new Button();
            newButtonCreateNewList.Width = 225;
            newButtonCreateNewList.Height = 30;
            newButtonCreateNewList.Background = Brushes.Black;
            newButtonCreateNewList.Foreground = Brushes.White;
            newButtonCreateNewList.Content = "Добавить список";
            newButtonCreateNewList.HorizontalAlignment = HorizontalAlignment.Center;
            newButtonCreateNewList.VerticalAlignment = VerticalAlignment.Top;
            newButtonCreateNewList.Click += CreateNewColumn;
            newButtonCreateNewList.Style = Application.Current.Resources["ButtonStyle"] as Style;
            return newButtonCreateNewList;
        }

        // Created new List with Cards
        private void CreateNewColumn(object sender, RoutedEventArgs e)
        {
            kanbanBoardModel.Lists.Add(new ColumnData());
            UpdateStackPanel();
        }

        // Bilding UI List with Cards
        private Border CreateUINewColumn(int indexColumn)
        {
            Border newColumn = new Border();
            TextBox TitleColumn = new TextBox();
            Button ButtonNewCard = new Button();
            StackPanel stackPanel = new StackPanel();

            // Style Column
            newColumn.BorderThickness = new Thickness(1);
            newColumn.BorderBrush = Brushes.Black;
            newColumn.CornerRadius = new CornerRadius(10);
            newColumn.Margin = new Thickness(0, 0, 10, 0);
            newColumn.VerticalAlignment = VerticalAlignment.Top;
            // Style Title 
            TitleColumn.Background = Brushes.Transparent;
            TitleColumn.Foreground = Brushes.Black;
            TitleColumn.FontSize = 14;
            TitleColumn.HorizontalAlignment = HorizontalAlignment.Left;
            TitleColumn.Margin = new Thickness(5, 5, 0, 5);
            TitleColumn.MaxWidth = 210;
            TitleColumn.MaxLines = 2;
            TitleColumn.Text = "New column";

            // Style Button
            ButtonNewCard.Width = 225;
            ButtonNewCard.Height = 30;
            ButtonNewCard.Foreground = Brushes.Black;
            ButtonNewCard.VerticalAlignment = VerticalAlignment.Bottom;
            ButtonNewCard.HorizontalAlignment = HorizontalAlignment.Center;
            ButtonNewCard.Cursor = Cursors.Hand;
            ButtonNewCard.FontSize = 12;
            ButtonNewCard.Name = "Column_" + Convert.ToString(indexColumn);
            ButtonNewCard.Style = Application.Current.Resources["PreviewNavigationButtonStyle"] as Style;
            ButtonNewCard.FontWeight = FontWeights.Bold;
            ButtonNewCard.Click += CreateNewCard;
            ButtonNewCard.Content = "Добавить карточку";

            stackPanel.Margin = new Thickness(5, 5, 5, 5);
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
            stackPanel.Children.Add(TitleColumn);

            for (int i = 0; i < kanbanBoardModel.Lists[indexColumn].Cards.Count; i++)
            {
                stackPanel.Children.Add(CreateUINewCard(kanbanBoardModel.Lists[indexColumn].Cards[i]));
            }

            stackPanel.Children.Add(ButtonNewCard);
            newColumn.Child = stackPanel;

            return newColumn;
        }
        
        // Creat new card in List 
        private void CreateNewCard(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string[] arrayWordsButton = button.Name.Split(new char[] { '_' });
            int columnIndex = Convert.ToInt32(arrayWordsButton[1]);

            Card newCard = new Card();
            newCard.Title = "New Card";
            kanbanBoardModel.Lists[columnIndex].Cards.Add(newCard);

            UpdateStackPanel();
        }
        
        private Border CreateUINewCard(Card card)
        {
            Border newCard = new Border();
            TextBlock textBlockTitle = new TextBlock();

            newCard.BorderThickness = new Thickness(1);
            newCard.BorderBrush = Brushes.Black;
            newCard.CornerRadius = new CornerRadius(5);
            newCard.Margin = new Thickness(0, 0, 0, 5);
            newCard.Width = 215;
            newCard.Height = 30;

            textBlockTitle.FontSize = 13;
            textBlockTitle.Foreground = Brushes.Black;
            textBlockTitle.Margin = new Thickness(5, 3, 5, 3);
            textBlockTitle.Text = card.Title;
            textBlockTitle.HorizontalAlignment = HorizontalAlignment.Left;

            newCard.Child = textBlockTitle;

            return newCard;
        }

        private void UpdateStackPanel()
        {
            Button newButton = CreateNewListButton();
            StackPanel newStackPanel = new StackPanel();
            newStackPanel.Orientation = Orientation.Horizontal;
            for (int i = 0; i < kanbanBoardModel.Lists.Count; i++)
            {
                Border newColumn = CreateUINewColumn(i);
                newStackPanel.Children.Add(newColumn);
            }

            newStackPanel.Children.Add(newButton);
            CurrentStackPanel = newStackPanel;
        }
    }
}
