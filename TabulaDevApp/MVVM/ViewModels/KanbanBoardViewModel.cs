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
        private NavigationStore _navigationStore;           // Navigation to add New Card
        private StackPanel _currentStackPanel;              // Shows tabel
        private KanbanBoardModel _kanbanBoardModel;         // Model contains structure of tabel
        private Card _dragAndDropCurrentCard;
        private Border _dragAndDropEmptyCard;
        private Canvas _dragOverPreviesColumn;
        private bool _dragOverIsEmpty;
        private int _dragOverColumnStore;
        private int _dragAndDropStoreCardIndex;
        private int _dragAndDropStoreColumnIndex;

        // Gettter & Setter
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

        // Ctors
        public KanbanBoardViewModel()
        {
            
        }
        public KanbanBoardViewModel(NavigationStore navigation, KanbanBoardModel model)
        {
            _dragAndDropStoreCardIndex = -1;
            _dragAndDropStoreColumnIndex = -1;
            _dragOverColumnStore = -1;
            _dragAndDropEmptyCard = CreateUIEmptyCard();
            _dragOverIsEmpty = true;

            kanbanBoardModel = model;
            _navigationStore = navigation;

            StartUpGrid();
            UpdateStackPanel();
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
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            SolidColorBrush background = new SolidColorBrush { Color = Color.FromRgb(47, 48, 55) };
            Button newButtonCreateNewList = new Button();

            newButtonCreateNewList.Width = 225;
            newButtonCreateNewList.Height = 30;
            newButtonCreateNewList.Background = background;
            newButtonCreateNewList.Foreground = foreground;

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
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            SolidColorBrush background = new SolidColorBrush { Color = Color.FromRgb(47, 48, 55) };

            Border newColumn = new Border();
            TextBox TitleColumn = new TextBox();
            Button ButtonNewCard = new Button();
            Canvas stackPanel = new Canvas();

            // Style Column
            newColumn.BorderThickness = new Thickness(0);
            newColumn.BorderBrush = foreground;
            newColumn.CornerRadius = new CornerRadius(10);
            newColumn.Margin = new Thickness(0, 0, 10, 0);
            newColumn.VerticalAlignment = VerticalAlignment.Top;
            newColumn.AllowDrop = true;

            // Style Title 
            TitleColumn.Background = Brushes.Transparent;
            TitleColumn.Foreground = foreground;
            TitleColumn.FontSize = 14;
            TitleColumn.FontWeight = FontWeights.Medium;
            TitleColumn.HorizontalAlignment = HorizontalAlignment.Left;
            TitleColumn.Margin = new Thickness(5, 5, 0, 5);
            TitleColumn.MaxWidth = 210;
            TitleColumn.MaxLines = 2;
            TitleColumn.Name = "Column_" + Convert.ToString(indexColumn);
            TitleColumn.Style = Application.Current.Resources["ColumnTextStyle"] as Style;
            TitleColumn.TextChanged += OnColumnTextChanged;

            if (kanbanBoardModel.Lists[indexColumn].Title == "")
            {
                TitleColumn.Text = "Новая колонка";
            }
            else
            {
                TitleColumn.Text = kanbanBoardModel.Lists[indexColumn].Title;
            }

            // Style Button
            ButtonNewCard.Width = 225;
            ButtonNewCard.Height = 30;
            ButtonNewCard.Foreground = foreground;
            ButtonNewCard.VerticalAlignment = VerticalAlignment.Bottom;
            ButtonNewCard.HorizontalAlignment = HorizontalAlignment.Center;
            ButtonNewCard.Margin = new Thickness(5, 5, 0, 5);
            ButtonNewCard.Cursor = Cursors.Hand;
            ButtonNewCard.FontSize = 12;
            ButtonNewCard.Name = "Column_" + Convert.ToString(indexColumn);
            ButtonNewCard.Style = Application.Current.Resources["PreviewNavigationButtonStyle"] as Style;
            ButtonNewCard.FontWeight = FontWeights.Bold;
            ButtonNewCard.Click += CreateNewCard;
            ButtonNewCard.Content = "Добавить карточку";

            stackPanel.Width = 225;
            stackPanel.Height = 0;
            stackPanel.Name = "Column_" + Convert.ToString(indexColumn); ;
            stackPanel.Margin = new Thickness(5, 5, 5, 5);
            stackPanel.HorizontalAlignment = HorizontalAlignment.Center;

            Canvas.SetLeft(TitleColumn, 0);
            Canvas.SetTop(TitleColumn, 0);
            stackPanel.Children.Add(TitleColumn);

            stackPanel.Drop += Column_Drop;
            stackPanel.DragOver += Column_DragOver;
            stackPanel.DragLeave += Column_DragLeave;
            stackPanel.DragEnter += Column_DragEnter;

            
            double countHeight = 50;
            if (_dragAndDropStoreCardIndex == 0 && kanbanBoardModel.Lists[indexColumn].Cards.Count == 0 &&
                _dragAndDropStoreColumnIndex == indexColumn)
            {
                Border emptyCard = CreateUIEmptyCard();
                Canvas.SetLeft(emptyCard, 5);
                Canvas.SetTop(emptyCard, countHeight);
                stackPanel.Children.Add(emptyCard);
                countHeight += 115;

                _dragAndDropStoreCardIndex = -1;
                _dragAndDropStoreColumnIndex = -1;
            }

            for (int i = 0; i < kanbanBoardModel.Lists[indexColumn].Cards.Count; i++)
            {
                if (_dragAndDropStoreCardIndex != -1 && _dragAndDropStoreColumnIndex != -1)
                {
                    if (_dragAndDropStoreCardIndex >= kanbanBoardModel.Lists[indexColumn].Cards.Count &&
                        _dragAndDropStoreColumnIndex == indexColumn &&
                        i == kanbanBoardModel.Lists[indexColumn].Cards.Count - 1)
                    {
                        kanbanBoardModel.Lists[indexColumn].Cards[i].id = Convert.ToString(i) + "_" + indexColumn;
                        Border newInCard = CreateUINewCard(kanbanBoardModel.Lists[indexColumn].Cards[i]);

                        Canvas.SetLeft(newInCard, 5);
                        Canvas.SetTop(newInCard, countHeight);
                        stackPanel.Children.Add(newInCard);
                        countHeight += 115;

                        Border emptyCard = CreateUIEmptyCard();
                        Canvas.SetLeft(emptyCard, 5);
                        Canvas.SetTop(emptyCard, countHeight);
                        stackPanel.Children.Add(emptyCard);
                        countHeight += 115;
                        //_dragAndDropStoreCardIndex = -1;
                        //_dragAndDropStoreColumnIndex = -1;
                        continue;
                    } 
                    else if (_dragAndDropStoreCardIndex == i && _dragAndDropStoreColumnIndex == indexColumn)
                    {
                        Border emptyCard = CreateUIEmptyCard();
                        Canvas.SetLeft(emptyCard, 5);
                        Canvas.SetTop(emptyCard, countHeight);
                        stackPanel.Children.Add(emptyCard);
                        countHeight += 115;
                        //_dragAndDropStoreCardIndex = -1;
                        //_dragAndDropStoreColumnIndex = -1;
                    }
                    
                }

                kanbanBoardModel.Lists[indexColumn].Cards[i].id = Convert.ToString(i) + "_" + indexColumn;
                Border newCard = CreateUINewCard(kanbanBoardModel.Lists[indexColumn].Cards[i]);

                Canvas.SetLeft(newCard, 5);
                Canvas.SetTop(newCard, countHeight);
                stackPanel.Children.Add(newCard);
                countHeight += 115;
            }

            stackPanel.Height = (countHeight > 80) ? countHeight + 30 : 80;
            Canvas.SetLeft(ButtonNewCard, 0);
            Canvas.SetTop(ButtonNewCard, stackPanel.Height - 30);
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

            _navigationStore.UpperViewModel = new AddCardViewModel(_navigationStore, kanbanBoardModel, columnIndex);

            UpdateStackPanel();
        }

        // Creat UI Card
        private Border CreateUINewCard(Card card)
        {
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            SolidColorBrush background = new SolidColorBrush { Color = Color.FromRgb(47, 48, 55) };
            SolidColorBrush backgroundTitle = new SolidColorBrush { Color = Color.FromRgb(28, 28, 33) };

            // Init normal view
            Border newCard = new Border();
            Border titleCard = new Border();
            Border rectUnder = new Border();

            StackPanel newPanel = new StackPanel();
            Grid titleGrid = new Grid();

            TextBlock textBlockTitle = new TextBlock();
            TextBlock textDescription = new TextBlock();

            newPanel.Orientation = Orientation.Vertical;

            newCard.Background = background;
            newCard.CornerRadius = new CornerRadius(5);
            newCard.Margin = new Thickness(0, 0, 0, 5);
            newCard.Width = 215;
            newCard.Height = 105;
            newCard.Name = "Card_" + card.id;
            //newCard.MouseDown += OnClickCard;
            //newCard.MouseLeftButtonDown += OnClickCard;
            newCard.MouseMove += Card_MouseMove;

            rectUnder.Background = backgroundTitle;
            rectUnder.VerticalAlignment = VerticalAlignment.Bottom;
            rectUnder.Width = 215;
            rectUnder.Height = 5;

            titleCard.Background = backgroundTitle;
            titleCard.CornerRadius = new CornerRadius(5);
            titleCard.VerticalAlignment = VerticalAlignment.Top;
            titleCard.Width = 215;
            titleCard.Height = 20;

            textBlockTitle.FontSize = 14;
            textBlockTitle.TextWrapping = TextWrapping.Wrap;
            textBlockTitle.MaxHeight = 45;
            textBlockTitle.Foreground = foreground;
            textBlockTitle.Margin = new Thickness(5, 3, 5, 3);
            textBlockTitle.Text = card.Title;
            textBlockTitle.HorizontalAlignment = HorizontalAlignment.Left;
            textBlockTitle.TextTrimming = TextTrimming.WordEllipsis;

            textDescription.FontSize = 12;
            textDescription.TextWrapping = TextWrapping.Wrap;
            textDescription.MaxHeight = 40;
            textDescription.Foreground = Brushes.Gray;
            textDescription.Margin = new Thickness(5, 5, 5, 3);
            textDescription.Text = card.Description;
            textDescription.HorizontalAlignment = HorizontalAlignment.Left;
            textDescription.TextTrimming = TextTrimming.WordEllipsis;

            titleGrid.Children.Add(titleCard);
            titleGrid.Children.Add(rectUnder);

            newPanel.Children.Add(titleGrid);
            newPanel.Children.Add(textBlockTitle);
            newPanel.Children.Add(textDescription);
            
            if (card.isDrag)
            {
                newCard.Opacity = 0.5;
            } 
            else
            {
                newCard.Opacity = 1;
            }

            newCard.Child = newPanel;
            return newCard;
        }

        // Creat Empty Card
        private Border CreateUIEmptyCard()
        {
            SolidColorBrush foreground = new SolidColorBrush { Color = Color.FromRgb(244, 245, 245) };
            SolidColorBrush background = new SolidColorBrush { Color = Color.FromRgb(47, 48, 55) };

            Border newCard = new Border();

            newCard.Background = background;
            newCard.CornerRadius = new CornerRadius(5);
            newCard.BorderBrush = foreground;
            newCard.Margin = new Thickness(0, 0, 0, 5);
            newCard.Width = 215;
            newCard.Height = 105;
            return newCard;
        }
        
        // Update display
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

        // Click on card, navigate to Card View
        private void OnClickCard(object sender, MouseButtonEventArgs e)
        {
            Border card = (Border)sender;
            string[] arrayWordsButton = card.Name.Split(new char[] { '_' });
            int cardIndex = Convert.ToInt32(arrayWordsButton[1]);
            int columnIndex = Convert.ToInt32(arrayWordsButton[2]);

            Console.WriteLine(kanbanBoardModel.Lists[columnIndex].Cards[cardIndex].Title);
            _navigationStore.UpperViewModel = new CardViewModel(_navigationStore, kanbanBoardModel, columnIndex, cardIndex);
        }

        // Keeps track of the display of column name changes
        private void OnColumnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox titleColumn = (TextBox)sender;
            string[] arrayWordsButton = titleColumn.Name.Split(new char[] { '_' });
            int columnIndex = Convert.ToInt32(arrayWordsButton[1]);

            kanbanBoardModel.Lists[columnIndex].Title = titleColumn.Text;
        }

        /// ------------------------
        /// Functions Drag & Drop
        /// ------------------------

        // Event - Card mouse move
        private void Card_MouseMove(object sender, MouseEventArgs e)
        {
            Border card = (Border)sender;
            string[] arrayWordsButton = card.Name.Split(new char[] { '_' });
            int cardIndex = Convert.ToInt32(arrayWordsButton[1]);
            int columnIndex = Convert.ToInt32(arrayWordsButton[2]);

            _dragAndDropCurrentCard = kanbanBoardModel.Lists[columnIndex].Cards[cardIndex];
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                card.IsHitTestVisible = false;
                DragDrop.DoDragDrop(card, new DataObject(DataFormats.Serializable, card), DragDropEffects.Move);
                card.IsHitTestVisible = true;
            }
        }

        // Event - Card drop at column
        private void Column_Drop(object sender, DragEventArgs e)
        {
            Canvas column = (Canvas)sender;
            column.Children.Remove(_dragAndDropEmptyCard);
            string[] arrayWordsButton = column.Name.Split(new char[] { '_' });
            int columnIndex = Convert.ToInt32(arrayWordsButton[1]);

            object data = e.Data.GetData(DataFormats.Serializable);

            if (data is UIElement element)
            {
                // Add new Card
                Point dropPosition = e.GetPosition(column);
                int cardIndex = Find_CardIndex(columnIndex, dropPosition.Y);

                // Remove old Card
                string[] arrayWordsOldCard = _dragAndDropCurrentCard.id.Split(new char[] { '_' });
                int cardIndexOldCard = Convert.ToInt32(arrayWordsOldCard[0]);
                int columnIndexOldCard = Convert.ToInt32(arrayWordsOldCard[1]);

                kanbanBoardModel.Lists[columnIndexOldCard].Cards.RemoveAt(cardIndexOldCard);

                _dragAndDropCurrentCard.isDrag = false;
                if (cardIndex >= kanbanBoardModel.Lists[columnIndex].Cards.Count)
                {
                    kanbanBoardModel.Lists[columnIndex].Cards.Add(_dragAndDropCurrentCard);
                }
                else
                {
                    kanbanBoardModel.Lists[columnIndex].Cards.Insert(cardIndex, _dragAndDropCurrentCard);
                }

                _dragAndDropStoreCardIndex = -1;
                _dragAndDropStoreColumnIndex = -1;
                _dragAndDropCurrentCard = null;

                UpdateStackPanel();
            }
        }


        // Event - Card drag and move
        private void Column_DragOver(object sender, DragEventArgs e)
        {
            Canvas column = (Canvas)sender;
            object data = e.Data.GetData(DataFormats.Serializable);

            _dragAndDropCurrentCard.isDrag = true;
            Console.WriteLine(column.Name);
            if (data is UIElement element)
            { 
                Point dropPosition = e.GetPosition(column);
                Canvas.SetLeft(element, dropPosition.X);
                Canvas.SetTop(element, dropPosition.Y);
                try
                {
                    if (!column.Children.Contains(element))
                    {
                        Column_DragLeave(sender, e);
                        column.Children.Add(element);
                    }
                }
                catch (Exception)
                {

                }
            }
        }

        private void Column_DragLeave(object sender, DragEventArgs e)
        {
            Canvas column = (Canvas)sender;
            object data = e.Data.GetData(DataFormats.Serializable);

            if (data is UIElement element)
            {
                try
                {
                    string[] arrayWordsButton = (element as Border).Name.Split(new char[] { '_' });
                    int cardIndex = Convert.ToInt32(arrayWordsButton[1]);
                    int columnIndex = Convert.ToInt32(arrayWordsButton[2]);
                    column.Children.Remove(element);
                } catch (Exception) { }
            }
        }

        private int Find_CardIndex(int columnIndex, double YPosition)
        {
            int cardIndex = 0;
            double countHeight = 50;
            for(int i = 0; i < kanbanBoardModel.Lists[columnIndex].Cards.Count + 1; i++)
            {
                if(YPosition >= countHeight && YPosition <= (countHeight + 115)) {
                    cardIndex = i;
                    break;
                }
                countHeight += 115;
            }

            return cardIndex;
        }

        private void Column_DragEnter(object sender, DragEventArgs e)
        {
            Canvas column = (Canvas)sender;
            string[] arrayWordsButton = column.Name.Split(new char[] { '_' });
            int columnIndex = Convert.ToInt32(arrayWordsButton[1]);

            object data = e.Data.GetData(DataFormats.Serializable);

            if (data is UIElement element)
            {
                Point dropPosition = e.GetPosition(column);
                if (dropPosition.X >= 210.0 || dropPosition.X <= 15.0 ||
                    dropPosition.Y >= column.Height - 10 || dropPosition.Y <= 5.0)
                {
                    _dragAndDropStoreCardIndex = -1;
                    _dragAndDropStoreColumnIndex = -1;
                    _dragAndDropCurrentCard.isDrag = false;
                    UpdateStackPanel();
                }
                else
                {
                    _dragAndDropStoreCardIndex = Find_CardIndex(columnIndex, dropPosition.Y);
                    _dragAndDropStoreColumnIndex = columnIndex;
                    _dragAndDropCurrentCard.isDrag = true;
                    UpdateStackPanel();
                }
                // Preview enter
                //if (_dragOverColumnStore == -1)
                //{
                //    _dragOverColumnStore = columnIndex;
                //    _dragOverPreviesColumn = column;
                //}
                //else
                //{
                //    if (columnIndex == _dragOverColumnStore)
                //    {
                //        _dragOverIsEmpty = true;
                //        column.Children.Remove(_dragAndDropEmptyCard);
                //    }
                //    else
                //    {
                //        _dragOverColumnStore = columnIndex;
                //        _dragOverPreviesColumn.Children.Remove(_dragAndDropEmptyCard);
                //        _dragOverPreviesColumn.UpdateLayout();
                //        _dragOverPreviesColumn = column;
                //    }
                //}


                //if (_dragOverIsEmpty)
                //{
                //    ObservableCollection<UIElement> itemsList = new ObservableCollection<UIElement>();
                //    foreach (UIElement card in column.Children)
                //    {
                //        itemsList.Add(card);
                //    }

                //    column.Children.Clear();
                //    try
                //    {
                //        Canvas.SetLeft(itemsList[0], 0);
                //        Canvas.SetTop(itemsList[0], 0);
                //        column.Children.Add(itemsList[0]);
                //        double yPosition = 50;
                //        for (int i = 1; i < itemsList.Count - 1; i++)
                //        {
                //            if (dropPosition.Y > column.Height && i == itemsList.Count - 2)
                //            {
                //                Canvas.SetLeft(_dragAndDropEmptyCard, 5);
                //                Canvas.SetTop(_dragAndDropEmptyCard, yPosition);
                //                if (!column.Children.Contains(_dragAndDropEmptyCard))
                //                {
                //                    column.Children.Add(_dragAndDropEmptyCard);
                //                }
                //                yPosition += 115;
                //            }
                //            else if (dropPosition.Y >= yPosition && dropPosition.Y <= (yPosition + 115))
                //            {
                //                Canvas.SetLeft(_dragAndDropEmptyCard, 5);
                //                Canvas.SetTop(_dragAndDropEmptyCard, yPosition);
                //                if (!column.Children.Contains(_dragAndDropEmptyCard))
                //                {
                //                    column.Children.Add(_dragAndDropEmptyCard);
                //                }
                //                yPosition += 115;
                //            }
                //            Canvas.SetLeft(itemsList[i], 0);
                //            Canvas.SetTop(itemsList[i], yPosition);
                //            column.Children.Add(itemsList[i]);

                //            yPosition += 115;
                //        }
                //        Canvas.SetLeft(itemsList[itemsList.Count - 1], 0);
                //        Canvas.SetTop(itemsList[itemsList.Count - 1], yPosition);
                //        column.Children.Add(itemsList[itemsList.Count - 1]);
                //    }
                //    catch (Exception)
                //    {
                //        //UpdateStackPanel();
                //    }
                //}
                //_dragOverIsEmpty = false;
                //OnPropertyChanged("CurrentStackPanel");
            }
        }
    }
}
