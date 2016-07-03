using System;
using System.Collections.Generic;
using System.IO;
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
    /// Interaction logic for CircuitDocumentPanel.xaml
    /// </summary>
    public partial class CircuitDocumentPanel : UserControl
    {
        public bool ToolShow;
        public bool PropShow;
        public bool HasLoaded;
        private bool AddingTool;
        private bool EditingParameter;
        private int ParameterStartIndex = 0;
        private SerkitComponent ActiveDragComponent;
        private SerkitComponent ActiveComponent;
        private SolidColorBrush White251 = new SolidColorBrush(Color.FromArgb(255, 251, 251, 251));
        //private string SerkitString;
        //private string SerkitName;

        public SerkitComponent MainSerkit;
        public SerkitDocument Document;

        public Cursor Current;

        public TreeViewItem Wire;

        public CircuitDocumentPanel()
        {
            InitializeComponent();
            AttachEventHandlers(App.Default);
            ShowToolboxButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 55, 147, 224));
            ToolShow = true;
            Wire = new TreeViewItem()
            {
                Header = " Wire",
                FontFamily = new FontFamily("Segoe UI Symbol")
            };
            Wire.MouseDown += Wire_MouseDown;
            InitializeTools();
        }

        public void InitializeTools()
        {
            Tools.Items.Clear();
            TreeViewItem root = App.GetDefaultRoot();
            AttachEventHandlers(root);
            Tools.Items.Add(root);
            ControlGrid.Children.Remove(Wire);
            Tools.Items.Add(Wire);
        }

        public void AttachEventHandlers(TreeViewItem t)
        {
            if(t.Items.Count == 0)
            {
                t.PreviewMouseDown += Component_PreviewMouseDown;

            }
            foreach(TreeViewItem ti in t.Items)
            {
                AttachEventHandlers(ti);
            }
        }
        public string GetCode()
        {
            string s = "";
            if (IsLoaded)
            {
                s += (string)AMWarehouse.Content + "\r\n";
                s += (string)AMSerkitName.Content + "\r\n" + "\r\n";
                s += (string)AMScale.Content + "\r\n" + "\r\n";
                s += "\tGeometry " + AMWidth.Text + " " + AMHeight.Text + "\r\n";
                string g = AMGeometry.Text.Replace("\n", "\n\t\t");
                s += g + "\r\n";
                s += "\tEndGeometry\r\n\r\n";
                s += "\tVisual " + AMVisualHeader.Text + "\r\n";
                string v = AMVisual.Text.Replace("\r\n", "\r\n\t\t");
                s += v + "\r\n";
                s += "\tEndVisual\r\n\r\n";
                s += "\tIO (" + AMInputs.Text + " " + AMOutputs.Text + ") \"" + AMIONames.Text + "\"\r\n";
                string io = AMIO.Text.Replace("\n", "\n\t\t");
                s += io + "\r\n";
                s += "\tEndIO\r\n";
                s += "EndSerkit";
            }
            return s;
        }

        private void CanvasPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            if(!ToolShow)
                ToolBoxPanel.Visibility = System.Windows.Visibility.Collapsed;
        }
        private void HideToolBox_Click(object sender, RoutedEventArgs e)
        {
            ToolShow = false;
            ToolBoxPanel.Visibility = System.Windows.Visibility.Collapsed;
            CanvasScroll.Margin = new Thickness(30, 0, 0, 0);
            ShowToolboxButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
        }
        private void ToolBoxPanel_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }
        private void CanvasPanel_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Pressed && ActiveDragComponent != null)
            {
                if (!CanvasPanel.Children.Contains(ActiveDragComponent))
                    CanvasPanel.Children.Add(ActiveDragComponent);
                Point m = Mouse.GetPosition(CanvasPanel);
                Canvas.SetLeft(ActiveDragComponent, m.X - ActiveDragComponent.ActualWidth / 2);
                Canvas.SetTop(ActiveDragComponent, m.Y - ActiveDragComponent.ActualHeight / 2);
                ActiveDragComponent.Visibility = System.Windows.Visibility.Visible;
            }
        }
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            StatusBar.Width = Width;
            StatusBar.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
        }
        private void CanvasPanel_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            ActiveDragComponent = null;
            AddingTool = false; 
            foreach (Control c in CanvasPanel.Children)
            {
                if (c.GetType() == typeof(SerkitComponent))
                {
                    SerkitComponent s = (SerkitComponent)c;
                    s.IsSelected = false;
                }
            }
            if(ActiveComponent != null)
                ActiveComponent.IsSelected = true;
        }
        private void Component_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            AddingTool = true;
            TreeViewItem t = (TreeViewItem)sender;
            ActiveDragComponent = new SerkitComponent();
            ActiveDragComponent.MouseDown += ActiveDragComponent_MouseDown;
            ActiveDragComponent.ParseText((string)t.Tag, true);
            ActiveDragComponent.Visibility = System.Windows.Visibility.Visible;
            ActiveComponent = ActiveDragComponent;
            Grid.SetZIndex(ActiveDragComponent, 1);
        }
        void ActiveDragComponent_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ActiveComponent = (SerkitComponent)sender;
        }
        private void CanvasPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            if(Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if(ActiveDragComponent != null)
                {
                    CanvasPanel.Children.Remove(ActiveDragComponent);
                    //ActiveDragComponent = null;
                }
            }
        }
        private void ViewCode_Click(object sender, RoutedEventArgs e)
        {
            VisualGrid.Visibility = System.Windows.Visibility.Collapsed;
            GeometryGrid.Visibility = System.Windows.Visibility.Collapsed;
            CodeGrid.Visibility = System.Windows.Visibility.Visible;
            PropertiesGrid.Visibility = System.Windows.Visibility.Collapsed;
            ViewCode.Background = White251;
            ViewGeometry.Background = null;
            ViewVisual.Background = null;
            ViewProperties.Background = null;
            AMWarehouse.Content = "Warehouse " + WarehouseText.Text + ";";
            AMSerkitName.Content = "Serkit " + NameText.Text + ",";
            string str = "";
            foreach (Control c in CanvasPanel.Children)
            {
                if (c.GetType() == typeof(SerkitComponent))
                {
                    SerkitComponent s = (SerkitComponent)c;
                    str += "Import [" + s.Warehouse + "." + s.SerkitName + "] " + Canvas.GetLeft(s) + " " + Canvas.GetTop(s) + ";\r\n";
                }
            }
            AMVisual.Text = str;
        }
        private void ViewVisual_Click(object sender, RoutedEventArgs e)
        {
            VisualGrid.Visibility = System.Windows.Visibility.Visible;
            GeometryGrid.Visibility = System.Windows.Visibility.Collapsed;
            CodeGrid.Visibility = System.Windows.Visibility.Collapsed;
            PropertiesGrid.Visibility = System.Windows.Visibility.Collapsed;
            ViewCode.Background = null;
            ViewGeometry.Background = null;
            ViewProperties.Background = null;
            ViewVisual.Background = White251;
        }
        private void ViewGeometry_Click(object sender, RoutedEventArgs e)
        {
            VisualGrid.Visibility = System.Windows.Visibility.Collapsed;
            GeometryGrid.Visibility = System.Windows.Visibility.Visible;
            CodeGrid.Visibility = System.Windows.Visibility.Collapsed;
            PropertiesGrid.Visibility = System.Windows.Visibility.Collapsed;
            ViewCode.Background = null;
            ViewGeometry.Background = White251;
            ViewVisual.Background = null;
            ViewProperties.Background = null;
            RefreshSerkit();
        }
        private void ViewProperties_Click(object sender, RoutedEventArgs e)
        {
            VisualGrid.Visibility = System.Windows.Visibility.Collapsed;
            GeometryGrid.Visibility = System.Windows.Visibility.Collapsed;
            CodeGrid.Visibility = System.Windows.Visibility.Collapsed;
            PropertiesGrid.Visibility = System.Windows.Visibility.Visible;
            ViewCode.Background = null;
            ViewGeometry.Background = null;
            ViewVisual.Background = null;
            ViewProperties.Background = White251;
        }
        private void Wire_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.Current = new Cursor(new MemoryStream(Properties.Resources.pen));
        }
        private void CanvasPanel_MouseMove(object sender, MouseEventArgs e)
        {
            
        }
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
        private void Code_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                RefreshSerkit();
            }
        }
        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            ToolBoxPanel.Visibility = System.Windows.Visibility.Visible;
            ShowToolboxButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 74, 160, 232));
            Grid.SetZIndex(ToolBoxPanel, 1);
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ToolShow = true;
            Grid.SetZIndex(ToolBoxPanel, 1);
            ToolBoxPanel.Visibility = System.Windows.Visibility.Visible;
            ShowToolboxButton.Foreground = new SolidColorBrush(Color.FromArgb(255, 74, 160, 232));
            CanvasScroll.Margin = new Thickness(ToolBoxPanel.ActualWidth + 30, 0, 0, 0);
        }

        private void WarehouseText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (WarehouseUndefaulted != null) WarehouseUndefaulted.IsChecked = true;
            if (MainSerkit != null) MainSerkit.Warehouse = WarehouseText.Text;
        }
        private void NameText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsLoaded)
            {
                RefreshSerkit();
                NameUndefaulted.IsChecked = true;
                MainSerkit.SerkitName = NameText.Text;
                Document.Title = NameText.Text;
            }
        }
        private void ScaleText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (ScaleUndefaulted != null) ScaleUndefaulted.IsChecked = true;
            try
            {
                if (IsLoaded && ScaleText.Text.Length == 0) ScaleText.Text = "0";
                if (MainSerkit != null) MainSerkit.Scale = Double.Parse(ScaleText.Text);
                if (IsLoaded) AMScale.Content = "Scale " + MainSerkit.Scale + ";";
            }
            catch(ArgumentException ex)
            {
                ShowSmallError(ex.Message);
            }
        }
        private void WarehouseUndefaulted_Unchecked(object sender, RoutedEventArgs e)
        {
            WarehouseUndefaulted.IsChecked = false;
            MainSerkit.Warehouse = "Unnamed";
        }
        private void NameUndefaulted_Unchecked(object sender, RoutedEventArgs e)
        {
            NameUndefaulted.IsChecked = false;
            MainSerkit.SerkitName = "Unnamed";
        }
        private void ScaleUndefaulted_Unchecked(object sender, RoutedEventArgs e)
        {
            ScaleUndefaulted.IsChecked = true;
            MainSerkit.Scale = 1;
        }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox t = (TextBox)sender;
            int index = t.CaretIndex;
            if (e.Key == Key.Enter)
            {
                if (EditingParameter)
                {
                    EditingParameter = false;
                    ParameterStartIndex = 0;
                }
                else
                {
                    t.Text = t.Text.Insert(index, "\r\n");
                    t.CaretIndex = index + 1;
                }
            }
            else if(e.Key == Key.LeftCtrl)
            {
                string rem = AMGeometry.Text.Substring(ParameterStartIndex);
                if(!rem.Contains("[") && !rem.Contains("]"))
                {
                    ParameterStartIndex = 0;
                    rem = AMGeometry.Text;
                }
                if(rem.Contains("[") && rem.Contains("]"))
                {
                    int s = AMGeometry.Text.IndexOf("[", ParameterStartIndex);
                    int es = AMGeometry.Text.IndexOf("]", ParameterStartIndex) + 1;
                    AMGeometry.SelectionStart = s;
                    AMGeometry.SelectionLength = es - s;
                    ParameterStartIndex = es + 1;
                    AMGeometry.Focus();
                }
            }
        }

        public void ShowLargeError(string message)
        {
            if (StatusBar != null)
            {
                StatusBar.Height = 20;
                StatusErrorScroll.Height = Status.ActualHeight;
                Status.Text = "Error!";
                StatusBar.Background = new SolidColorBrush(Color.FromArgb(255, 232, 105, 105));
                StatusErrorText.Text = message;
                StatusErrorScroll.Margin = new Thickness(40, 25, 25, 15);
                StatusErrorText.Visibility = System.Windows.Visibility.Visible;
            }
        }
        public void ShowSmallError(string message)
        {
            StatusBar.Height = 20;
            Status.Text = message;
            StatusBar.Background = new SolidColorBrush(Color.FromArgb(255, 232, 105, 105));
            StatusErrorText.Visibility = System.Windows.Visibility.Collapsed;
            StatusErrorText.Text = "";
        }
        public void ShowLargeStatus(string message)
        {
            StatusBar.Height = 20;
            Status.Text = message;
            StatusBar.Background = new SolidColorBrush(Color.FromArgb(255, 74, 160, 232));
            StatusErrorText.Visibility = System.Windows.Visibility.Collapsed;
            StatusErrorText.Text = "";
        }
        public void ShowSmallStatus(string message)
        {
            StatusBar.Height = 20;
            Status.Text = message;
            StatusBar.Background = new SolidColorBrush(Color.FromArgb(255, 74, 160, 232));
            StatusErrorText.Visibility = System.Windows.Visibility.Collapsed;
            StatusErrorText.Text = "";
        }
        public void ShowLargeSuccess(string message)
        {
            StatusBar.Height = 20;
            Status.Text = message;
            StatusBar.Background = new SolidColorBrush(Color.FromArgb(255, 75, 170, 47));
            StatusErrorText.Visibility = System.Windows.Visibility.Collapsed;
            StatusErrorText.Text = "";
        }
        public void ShowSmallSuccess(string message)
        {
            StatusBar.Height = 20;
            Status.Text = message;
            StatusBar.Background = new SolidColorBrush(Color.FromArgb(255, 75, 170, 47));
            StatusErrorText.Visibility = System.Windows.Visibility.Collapsed;
            StatusErrorText.Text = "";
        }
        private void StatusErrorText_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetText(StatusErrorText.Text);
        }
        public void RefreshSerkit()
        {
            try
            {
                MainSerkit = new SerkitComponent();
                MainSerkit.ParseText(GetCode(), true);
                Thickness t = MainSerkit.Margin;
                t.Left = -MainSerkit.Width / 2;
                t.Top = -MainSerkit.Height / 2;
                MainSerkit.Margin = t;
                Geometry.Content = MainSerkit;
                ShowSmallSuccess("No Errors.");
            }
            catch (Exception ex)
            {
                string s = "";
                Exception ea = ex;
                while (ea != null)
                {
                    s += ea.Message + "\r\n" + ea.StackTrace;
                    ea = ea.InnerException;
                }
                ShowLargeError(s);
            }
        }

        private void ShapeChooser_SelectionPerformed(TextShapeChooser tsc)
        {
            AMGeometry.Text += tsc.GetShapeString() + ";\r\n";
            int s = AMGeometry.Text.IndexOf("[");
            int e = AMGeometry.Text.IndexOf("]") + 1;
            AMGeometry.SelectionStart = s;
            AMGeometry.SelectionLength = e - s;
            AMGeometry.Focus();
        }

        private void AMGeometry_SelectionChanged(object sender, RoutedEventArgs e)
        {
            ParameterStartIndex = AMGeometry.CaretIndex;
        }

        private void CopyCode_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(GetCode());
        }

        private void StatusBarRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(StatusErrorScroll.Visibility == System.Windows.Visibility.Visible)
            {
                Status.Text = "Dissmissed.";
                StatusBar.Height = 20;
                StatusBar.Background = new SolidColorBrush(Color.FromArgb(255, 74, 160, 232));
                StatusErrorScroll.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                StatusBar.Height = 160;
                StatusErrorScroll.Height = 130;
                StatusErrorScroll.Margin = new Thickness(4, 24, 6, 6);
                StatusErrorScroll.Visibility = System.Windows.Visibility.Visible;
            }
        }

    }
}
