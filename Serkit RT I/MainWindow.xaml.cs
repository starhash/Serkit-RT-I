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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Cursor Current = new Cursor(new MemoryStream(Properties.Resources.pointer));
        private bool _maximised;
        public bool Maximised 
        {
            get { return _maximised; }
            set
            {
                _maximised = value;
                if (Maximised)
                {
                    minH = Height;
                    minW = Width;
                    minX = Left;
                    minY = Top;
                    Height = SystemParameters.WorkArea.Height;
                    Width = SystemParameters.WorkArea.Width;
                    Left = 0;
                    Top = 0;
                    Maximize.Content = "";
                }
                else
                {
                    Height = minH;
                    Width = minW;
                    Left = minX;
                    Top = minY;
                    Maximize.Content = "";
                }
            } 
        }
        private bool IsMouseDown;
        private double minX = 100, minY = 100, minW = 500, minH = 400;
        private double distance = 0;
        private HwndSource _hwndSource;
        
        public List<SerkitDocument> Windows;
        public List<Key> Keys;

        public MainWindow()
        {
            InitializeComponent();
            Maximize.Content = "";
            Windows = new List<SerkitDocument>();
            Keys = new List<Key>();
            LoadSettings();
            
        }

        public void LoadSettings()
        {
            double pw = (double)Properties.Settings.Default["PreviousWidth"];
            double ph = (double)Properties.Settings.Default["PreviousHeight"];
            Maximised = (bool)Properties.Settings.Default["PreviousMaximizedState"];
            Width = (pw == -1) ? SystemParameters.WorkArea.Width : pw;
            Height = (ph == -1) ? SystemParameters.WorkArea.Height : ph;
            Left = (double)Properties.Settings.Default["PreviousLeft"];
            Top = (double)Properties.Settings.Default["PreviousTop"];
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
        private void MoveRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed && !Maximised)
            {
                DragMove();
            }
            else
            {
                Console.WriteLine(distance);
                if (distance > 10)
                {
                    Height = minH;
                    Width = minW;
                    Left = e.GetPosition((UIElement)Parent).X - e.GetPosition(this).X % minW;
                    Top = e.GetPosition((UIElement)Parent).Y - e.GetPosition(this).Y % minH;
                    Maximised = !Maximised;
                    DragMove();
                }
            }
        }
        private void fileMenu_Click(object sender, RoutedEventArgs e)
        {
            Control parent = (Control)ExitMenuItem.Parent;
            parent.BorderThickness = new Thickness(0);
        }
        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            Maximised = !Maximised;
        }
        private void SerkitWindow_StateChanged(object sender, EventArgs e)
        {
            if(WindowState == System.Windows.WindowState.Maximized)
            {
                WindowState = System.Windows.WindowState.Normal;
                minH = Height;
                minW = Width;
                minX = Left;
                minY = Top;
                Height = SystemParameters.WorkArea.Height;
                Width = SystemParameters.WorkArea.Width;
                Left = 0;
                Top = 0;
                Maximize.Content = "";
                Maximised = true;
            }
        }
        private void MoveRectangle_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            distance = 0;
        }
        private void MoveRectangle_PreviewDragOver(object sender, DragEventArgs e)
        {
            distance++;
        }
        private void Minimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = System.Windows.WindowState.Minimized;
        }
        private void SerkitDocument_Loaded(object sender, RoutedEventArgs e)
        {
            SerkitDocument s = (SerkitDocument)sender;
            s.Tab.MouseDown += Tab_MouseDown;
            if (!WindowTabBar.Children.Contains(s.Tab))
                WindowTabBar.Children.Add(s.Tab);
        }
        void Tab_MouseDown(object sender, MouseButtonEventArgs e)
        {
            
        }
        private void UnSelectAll()
        {
            int i = 1;
            foreach(SerkitDocument s in Windows)
            {
                s.IsSelected = false;
                Canvas.SetZIndex(s, i - Windows.Count);
            }
        }
        void doc_Selected(SerkitDocument sender)
        {
            UnSelectAll();
            Canvas.SetZIndex(sender, 1);
        }
        private void ResizeRectangle_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "TopRect":
                    Cursor = Cursors.SizeNS;
                    break;
                case "BottomRect":
                    Cursor = Cursors.SizeNS;
                    break;
                case "LeftRect":
                    Cursor = Cursors.SizeWE;
                    break;
                case "RightRect":
                    Cursor = Cursors.SizeWE;
                    break;
                case "TopLeft":
                    Cursor = Cursors.SizeNWSE;
                    break;
                case "TopRight":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "BottomLeft":
                    Cursor = Cursors.SizeNESW;
                    break;
                case "BottomRight":
                    Cursor = Cursors.SizeNWSE;
                    break;
                default:
                    Cursor = Current;
                    break;
            }
        }
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam); 
        private void ResizeWindow(ResizeDirection direction)
        {
            SendMessage(_hwndSource.Handle, 0x112, (IntPtr)(61440 + direction), IntPtr.Zero);
        }
        private enum ResizeDirection
        {
            Left = 1,
            Right = 2,
            Top = 3,
            TopLeft = 4,
            TopRight = 5,
            Bottom = 6,
            BottomLeft = 7,
            BottomRight = 8,
        }
        private void ResizeRectangle_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            IsMouseDown = true;
            Rectangle rectangle = sender as Rectangle;
            switch (rectangle.Name)
            {
                case "TopRect":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Top);
                    break;
                case "BottomRect":
                    Cursor = Cursors.SizeNS;
                    ResizeWindow(ResizeDirection.Bottom);
                    break;
                case "LeftRect":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Left);
                    break;
                case "RightRect":
                    Cursor = Cursors.SizeWE;
                    ResizeWindow(ResizeDirection.Right);
                    break;
                case "TopLeft":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.TopLeft);
                    break;
                case "TopRight":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.TopRight);
                    break;
                case "BottomLeft":
                    Cursor = Cursors.SizeNESW;
                    ResizeWindow(ResizeDirection.BottomLeft);
                    break;
                case "BottomRight":
                    Cursor = Cursors.SizeNWSE;
                    ResizeWindow(ResizeDirection.BottomRight);
                    break;
                default:
                    Cursor = Current;
                    break;
            }
        }
        private void SerkitWindow_SourceInitialized(object sender, EventArgs e)
        {
            _hwndSource = (HwndSource)PresentationSource.FromVisual(this);
        }
        private void SerkitWindow_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton != MouseButtonState.Pressed)
                Cursor = Current;
        }
        private void SerkitWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Keys.Contains(e.Key))
                Keys.Add(e.Key);
            Console.WriteLine("\nDown : ");
            foreach (Key k in Keys)
            {
                Console.Write(k.ToString() + " ");
            }
        }
        private void SerkitWindow_KeyUp(object sender, KeyEventArgs e)
        {
            if (Keys.Contains(e.Key))
                Keys.Remove(e.Key);
            Console.WriteLine("\nUp : ");
            foreach(Key k in Keys)
            {
                Console.Write(k.ToString() + " ");
            }
        }
        private void SerkitWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Properties.Settings.Default["PreviousMaximizedState"] = Maximised;
            Properties.Settings.Default["PreviousHeight"] = Height;
            Properties.Settings.Default["PreviousWidth"] = Width;
            Properties.Settings.Default["PreviousLeft"] = Left;
            Properties.Settings.Default["PreviousTop"] = Top;
        }
        public void SendKeyStroke(List<Key> keys)
        {

        }

        private void AddWindowToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            SerkitDocument doc = new SerkitDocument() { Height = 400, Width = 500 };
            doc.Loaded += SerkitDocument_Loaded;
            doc.Selected += doc_Selected;
            Canvas.SetLeft(doc, Windows.Count * 10 + 10);
            Canvas.SetTop(doc, Windows.Count * 5 + 10);
            CanvasView.Children.Add(doc);
            doc.Title = "Window  " + Windows.Count;
            Windows.Add(doc);
            UnSelectAll();
            doc.IsSelected = true;
            Canvas.SetZIndex(doc, 1);
        }
        private void AddCircuitWindowToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            SerkitDocument doc = new SerkitDocument() { Height = 400, Width = 500 };
            doc.Loaded += SerkitDocument_Loaded;
            doc.Selected += doc_Selected;
            Canvas.SetLeft(doc, Windows.Count * 10 + 10);
            Canvas.SetTop(doc, Windows.Count * 5 + 10);
            CanvasView.Children.Add(doc);
            doc.Title = "New Circuit";
            doc.ContentData.Content = new CircuitDocumentPanel() { Document = doc };
            Windows.Add(doc);
            UnSelectAll();
            doc.IsSelected = true;
            doc.IsMaximized = true;
            Canvas.SetZIndex(doc, 1);
        }

    }
}
