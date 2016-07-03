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

namespace Serkit_RT_I
{
    /// <summary>
    /// Interaction logic for WindowTab.xaml
    /// </summary>
    public partial class WindowTab : UserControl
    {
        public SolidColorBrush SelectedBrush = new SolidColorBrush(Color.FromArgb(255, 35, 136, 220));
        public SolidColorBrush UnselectedBrush = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
        public SolidColorBrush UnselectedBorderBrush = new SolidColorBrush(Color.FromArgb(255, 200, 200, 200));
        public string Title { get { return (string)TitleLabel.Content; } set { TitleLabel.Content = value; ToolTip = new ToolTip() { Content = Title }; } }
        public SerkitDocument AttachedDocument;

        private bool _selected;
        private bool _minimized;
        private bool _maximized;

        public bool IsSelected
        {
            get { return _selected; }
            set
            {
                _selected = value;
                if (IsSelected)
                {
                    Background = SelectedBrush;
                    TitleLabel.Foreground = UnselectedBrush;
                }
                else
                {
                    Background = new SolidColorBrush(Color.FromArgb(255, 240, 240, 240));
                    TitleLabel.Foreground = new SolidColorBrush(Colors.LightSlateGray);
                }
                Console.WriteLine("WindowTab.Selected()");
                AttachedDocument.IsSelected = value;
            }
        }
        public bool IsMinimized
        {
            get { return _minimized; }
            set
            {
                _minimized = value;
                if (IsMinimized)
                    AttachedDocument.Restore();
                else
                {
                    AttachedDocument.IsMinimized = true;
                }
            }
        }
        public bool IsMaximized
        {
            get { return _maximized; }
            set
            {
                _maximized = value;
                AttachedDocument.IsMaximized = IsMaximized;
            }
        }

        public void SetSelected(bool selected)
        {
            _selected = selected;
            if (IsSelected)
            {
                Background = SelectedBrush;
                AttachedDocument.Accent = (SolidColorBrush)Background;
                TitleLabel.Foreground = new SolidColorBrush(Colors.White);
            }
            else
            {
                Background = UnselectedBrush;
                TitleLabel.Foreground = new SolidColorBrush(Colors.LightSlateGray);
            }
            Console.WriteLine("WindowTab.Selected()");
        }

        public WindowTab(SerkitDocument serkit)
        {
            InitializeComponent();
            AttachedDocument = serkit;
            Title = AttachedDocument.Title;
        }

        private void CloseToolbarButton_Click(object sender, RoutedEventArgs e)
        {
            AttachedDocument.Close();
        }
        private void UserControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AttachedDocument.IsSelected = true;
            AttachedDocument.IsMaximized = false;
        }
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            if (!IsSelected)
            {
                Background = new SolidColorBrush(Color.FromArgb(90, 35, 136, 220));
                AttachedDocument.Accent = (SolidColorBrush)Background;
            }
        }
        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //AttachedDocument.IsMinimized = !AttachedDocument.IsMinimized;
            AttachedDocument.IsSelected = true;
        }
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if (IsSelected)
            {
                Background = SelectedBrush;
                AttachedDocument.Accent = (SolidColorBrush)Background;
            }
            else
            {
                Background = UnselectedBrush;
                AttachedDocument.Accent = AttachedDocument.UnselectedBrush;
                if (AttachedDocument.IsMaximized)
                    AttachedDocument.BorderBrush = UnselectedBrush;
                else
                    AttachedDocument.BorderBrush = UnselectedBorderBrush;
            }
        }

        private void TitleLabel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Console.WriteLine("IsVI" + Visibility.ToString());
        }
    }
}
