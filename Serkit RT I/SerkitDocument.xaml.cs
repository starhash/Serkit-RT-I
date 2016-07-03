using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.IO;

namespace Serkit_RT_I
{
    /// <summary>
    /// Interaction logic for SerkitDocument.xaml
    /// </summary>
    public partial class SerkitDocument : UserControl
    {
        Point MouseDownLocation;
        private bool ResizeMouseDown;
        private bool MoveMouseDown;
        private HwndSource _hwndSource;
        private double minX = 100, minY = 100, minW = 400, minH = 300;
        public SolidColorBrush SelectedBrush = new SolidColorBrush(Color.FromArgb(255, 35, 136, 220));
        public SolidColorBrush UnselectedBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
        public SolidColorBrush UnselectedBorderBrush = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
        public SolidColorBrush MainGridBrush;
        private bool _selected;
        private bool _minimized;
        private bool _maximized;
        private SolidColorBrush _accent;

        public string Title 
        {
            get
            {
                return (string)WindowTitle.Content; 
            } 
            set
            {
                WindowTitle.Content = value; Tab.Title = Title; 
            } 
        }
        public WindowTab Tab;
        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (IsSelected)
                {
                    Selected(this);
                }
                else
                {
                    Deselected(this);
                }
                Tab.SetSelected(value);
            }
        }
        public bool IsMinimized 
        {
            get { return _minimized; }
            set
            {
                _minimized = value;
                if (!IsMinimized)
                    Restore();
                else
                {
                    Visibility = System.Windows.Visibility.Collapsed;
                    Minimized(this);
                }
            }
        }
        public bool IsMaximized
        {
            get { return _maximized; }
            set
            {
                _maximized = value;
                if (IsMaximized)
                {
                    Canvas parent = (Canvas)Parent;
                    minH = Height;
                    minW = Width;
                    minX = Canvas.GetLeft(this);
                    minY = Canvas.GetTop(this);
                    Canvas.SetLeft(this, 0);
                    Canvas.SetTop(this, 0);
                    MainGrid.Height = 0;
                    Height = parent.ActualHeight;
                    Width = parent.ActualWidth;
                    MaximizeToolbarButton.Content = "";
                    ContentData.Width = Width;
                    ContentData.Height = Height;
                    _minimized = false;
                    IsSelected = true;
                    Maximized(this);
                }
                else
                {
                    MaximizeToolbarButton.Content = "";
                    MainGrid.Height = 24;
                    Restore();
                }
            }
        }
        public SolidColorBrush Accent 
        {
            get { return _accent; } 
            set 
            {
                _accent = value;
                MainGrid.Background = _accent;
                BorderBrush = _accent;
            } 
        }

        public delegate void RestoredHandler(SerkitDocument sender);
        public event RestoredHandler Restored;
        public delegate void MinimizedHandler(SerkitDocument sender);
        public event MinimizedHandler Minimized;
        public delegate void MaximizedHandler(SerkitDocument sender);
        public event MaximizedHandler Maximized;
        public delegate void SelectionHandler(SerkitDocument sender);
        public event SelectionHandler Selected;
        public delegate void DeselectionHandler(SerkitDocument sender);
        public event DeselectionHandler Deselected;

        public ContentPresenter ContentData 
        { 
            get { return ContentGrid; } 
            set 
            {
                ContentGrid = value;
                Control e = (Control)ContentData.Content;
                ContentData.Width = Width;
                ContentData.Height = Height;
                e.Width = ContentGrid.Width;
                e.Height = ContentGrid.Height;
            } 
        }

        public SerkitDocument()
        {
            InitializeComponent();
            Maximized += SerkitDocument_Maximized;
            Minimized += SerkitDocument_Minimized;
            Selected += SerkitDocument_Selected;
            Deselected += SerkitDocument_Deselected;
            Restored += SerkitDocument_Restored;
            Tab = new WindowTab(this);
            Visibility = System.Windows.Visibility.Visible;
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            MainGridBrush = SelectedBrush;
            ContentData.Width = Width;
            ContentData.Height = Height - MainGrid.Height;
        }


        void SerkitDocument_Restored(SerkitDocument sender)
        {
            
        }
        void SerkitDocument_Maximized(SerkitDocument sender)
        {
            
        }
        void SerkitDocument_Selected(SerkitDocument sender)
        {
            Tab.Background = SelectedBrush;
            BorderBrush = SelectedBrush;
            BorderThickness = new Thickness(2, 3, 2, 2);
            MainGrid.Background = SelectedBrush;
            WindowTitle.Foreground = new SolidColorBrush(Color.FromArgb(255,255,255,255));
        }
        void SerkitDocument_Deselected(SerkitDocument sender)
        {
            Tab.Background = UnselectedBrush;
            if (IsMaximized)
                BorderBrush = UnselectedBorderBrush;
            else
                BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            BorderThickness = new Thickness(1, 2, 1, 1);
            MainGrid.Background = UnselectedBrush;
            WindowTitle.Foreground = new SolidColorBrush(Colors.LightGray);
        }
        void SerkitDocument_Minimized(SerkitDocument sender)
        {
            
        }

        public void Close()
        {
            Canvas parent = (Canvas)Parent;
            DockPanel pa = (DockPanel)Tab.Parent;
            parent.Children.Remove(this);
            pa.Children.Remove(Tab);
        }
        public void Minimize()
        {
            IsMinimized = true;
        }
        public void Maximize()
        {
            IsMaximized = true;
        }
        public void Restore()
        {
            Canvas parent = (Canvas)Parent;
            Canvas.SetLeft(this, minX);
            Canvas.SetTop(this, minY);
            ContentData.Width = minW;
            ContentData.Height = minH - MainGrid.Height;
            Console.WriteLine("BOYO : "+ContentData.Height + ":" + ContentData.Width);
            Height = minH;
            Width = minW;
            MaximizeToolbarButton.Content = "";
            Visibility = System.Windows.Visibility.Visible;
            _minimized = false;
            MainGrid.Height = 24;
            Restored(this);
        }

        private void MoveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            MoveMouseDown = true;
            MouseDownLocation = e.GetPosition(this);
        }
        private void MoveRectangle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsMaximized && IsSelected && MoveMouseDown)
            {
                Point pi = Mouse.GetPosition((UIElement)Parent);
                double X = pi.X - MouseDownLocation.X;
                double Y = pi.Y - MouseDownLocation.Y;
                Canvas.SetLeft(this, X);
                Canvas.SetTop(this, Y);
            }
        }
        private void MoveRectangle_MouseLeave(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && !IsMaximized)
            {
                Point pi = Mouse.GetPosition((UIElement)Parent);
                double X = pi.X - MouseDownLocation.X;
                double Y = pi.Y - MouseDownLocation.Y;
                Canvas.SetLeft(this, X);
                Canvas.SetTop(this, Y);
            }
        }
        private void MoveRectangle_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            MoveMouseDown = false;
        }
        private void MaximizeToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            IsMaximized = !IsMaximized;
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            Mouse.AddPreviewMouseMoveHandler(Parent, ResizeWindow_PreviewMouseMove);
            Mouse.AddMouseDownHandler(Parent, MoveRectangle_PreviewMouseMove);
            ContentData.Width = Width;
            ContentData.Height = Height - MainGrid.Height;
        }
        private void MinimizeToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            IsMinimized = true;
        }
        private void CloseToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void ResizeWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Pressed && ResizeMouseDown)
            {
                double W = Mouse.GetPosition((UIElement)sender).X - Canvas.GetLeft(this);
                double H = Mouse.GetPosition((UIElement)sender).Y - Canvas.GetTop(this);
                Width = (W < MinWidth) ? MinWidth : W;
                Height = (H < MinHeight) ? MinHeight : H;
                ContentData.Width = Width;
                ContentData.Height = Height - MainGrid.Height;
            }
        }
        private void ResizeWindow_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            ResizeMouseDown = true;
        }
        private void ResizeWindow_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ResizeMouseDown = false;
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            IsSelected = true;
        }
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void UserControl_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ResizeMouseDown = false;
            MoveMouseDown = false;
        }

        
        


    }
}
