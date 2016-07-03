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
using System.IO;

namespace Serkit_RT_I
{
    /// <summary>
    /// Interaction logic for SerkitComponent.xaml
    /// </summary>
    public partial class SerkitComponent : UserControl
    {
        private FileInfo _source;
        private double _scale;
        private bool _selected;
        private bool _dragged;
        private Point MouseDownLocation;
        private double OriginalHeight, OriginalWidth, OX, OY;
        private ScaleTransform Transform;

        public string SerkitData { get; set; }
        public string GeometryData { get; set; }
        public string VisualData { get; set; }
        public string IOData { get; set; }

        public FileInfo Source { get { return _source; } set { _source = value; ParseFile(_source, true); } }
        public double Scale 
        { 
            get { return _scale; } 
            set 
            {
                _scale = value; Console.WriteLine("Setting Scale : "+_scale);
                if (Source != null) ParseFile(Source, false);
            } 
        }
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
            }
        }
        public string SerkitName { get; set; }
        public string Warehouse { get; set; }

        public delegate void SelectionHandler(SerkitComponent serkit);
        public event SelectionHandler Selected;
        public delegate void DeselectionHandler(SerkitComponent serkit);
        public event DeselectionHandler Deselected;

        public SerkitComponent()
        {
            InitializeComponent();
            UIElement parent = (UIElement)(Parent);
            _scale = 1;
            _selected = false;
            Selected += SerkitComponent_Selected;
            Deselected += SerkitComponent_Deselected;
            SelectionGrid.Visibility = System.Windows.Visibility.Collapsed;
            Console.WriteLine(parent);
            Console.WriteLine(Properties.Settings.Default["GridHeight"] + " : " + Properties.Settings.Default["GridHeight"]);
        }

        public SerkitComponent(string filename)
        {
            InitializeComponent();
            Source = new FileInfo(filename);
            Selected += SerkitComponent_Selected;
            Deselected += SerkitComponent_Deselected;
        }

        void SerkitComponent_Selected(SerkitComponent serkit)
        {
            SelectionGrid.Visibility = System.Windows.Visibility.Visible;
            _dragged = true;
        }

        void SerkitComponent_Deselected(SerkitComponent serkit)
        {
            SelectionGrid.Visibility = System.Windows.Visibility.Collapsed;
        }

        private async void ParseFile(FileInfo f, bool defaultScale)
        {
            CanvasPanel.Children.Clear();
            try
            {
                TextReader t = f.OpenText();
                SerkitData = await t.ReadToEndAsync();
                int nc = SerkitData.IndexOf("Warehouse ") + 9;
                if (nc == 6)
                    throw new SerkitFileFormatError(Source, "Warehouse not specified.");
                Warehouse = SerkitData.Substring(nc, SerkitData.IndexOf(";") - nc);
                Warehouse = Warehouse.Trim();
                int n = SerkitData.IndexOf("Serkit ")+7;
                if(n == 6)
                    throw new SerkitFileFormatError(Source, "Serkit Name not defined.");
                SerkitName = SerkitData.Substring(n, SerkitData.IndexOf(",") - n);
                if (defaultScale)
                {
                    Console.WriteLine("Reading Scale...");
                    int scales = SerkitData.IndexOf("Scale") + 6;
                    int scalee = SerkitData.IndexOf(";", scales);
                    if (scales != 5)
                    {
                        _scale = Double.Parse(SerkitData.Substring(scales, scalee - scales));
                    }
                }
                Console.WriteLine("Scale = " + Scale);
                int gs = SerkitData.IndexOf("Geometry") + 8;
                int ge = SerkitData.LastIndexOf("EndGeometry");
                if (gs != -1)
                {
                    if (ge != -1)
                    {
                        GeometryData = SerkitData.Substring(gs, ge - gs).Trim();
                        StringTokenizer st = new StringTokenizer(GeometryData);
                        double w = Int32.Parse(st.NextToken()) * Scale;
                        double h = Int32.Parse(st.NextToken()) * Scale;
                        Width = w;
                        Height = h;
                        Background = null;
                        Foreground = new SolidColorBrush(Colors.Black);
                        Console.WriteLine("Width = " + w + " Height = " + h);
                        string shape;
                        while ((shape = st.NextToken(';')) != null)
                        {
                            ParseShape(shape);
                        }
                    }
                    else
                    {
                        throw new GeometryNotDefinedException(Source, "Expecting EndGeometry");
                    }
                }
                else
                {
                    throw new GeometryNotDefinedException(Source);
                }
                int vs = SerkitData.IndexOf("Visual") + 6;
                int ve = SerkitData.LastIndexOf("EndVisual");
                if (vs != -1)
                {
                    if (ve != -1)
                    {
                        VisualData = SerkitData.Substring(vs, ve - vs).Trim();
                        StringTokenizer st = new StringTokenizer(GeometryData);
                        Background = null;
                        Foreground = new SolidColorBrush(Colors.Black);
                        string shape;
                        while ((shape = st.NextToken(';')) != null)
                        {

                            ParseVisual(shape);
                        }
                    }
                }
                int ios = SerkitData.IndexOf("IO") + 8;
                int ioe = SerkitData.LastIndexOf("EndIO");
                if (ios != -1)
                {
                    if (ioe != -1)
                    {
                        IOData = SerkitData.Substring(ios, ioe - ios).Trim();
                        StringTokenizer st = new StringTokenizer(IOData);

                    }
                    else
                    {
                        throw new IONotDefinedException(Source, "Expecting EndIO");
                    }
                }
                else
                {
                    throw new IONotDefinedException(Source);
                }
            }
            catch (Exception e)
            {
                throw new SerkitFileFormatError(Source, e.Message);
            }
        }
        public void ParseText(string text, bool defaultScale)
        {
            CanvasPanel.Children.Clear();
            try
            {
                SerkitData = text;
                int nc = SerkitData.IndexOf("Warehouse ") + 9;
                if (nc == 6)
                    throw new SerkitFileFormatError(Source, "Warehouse not specified.");
                Warehouse = SerkitData.Substring(nc, SerkitData.IndexOf(";") - nc);
                Warehouse = Warehouse.Trim();
                int n = SerkitData.IndexOf("Serkit ") + 7;
                if (n < 7)
                    throw new SerkitFileFormatError(Source, "Serkit Name not defined.");
                SerkitName = SerkitData.Substring(n, SerkitData.IndexOf(",") - n);
                if (defaultScale)
                {
                    Console.WriteLine("Reading Scale...");
                    int scales = SerkitData.IndexOf("Scale") + 6;
                    int scalee = SerkitData.IndexOf(";", scales);
                    if (scales != 5)
                    {
                        _scale = Double.Parse(SerkitData.Substring(scales, scalee - scales));
                    }
                }
                Console.WriteLine("Scale = " + Scale);
                int gs = SerkitData.IndexOf("Geometry") + 8;
                int ge = SerkitData.LastIndexOf("EndGeometry");
                if (gs != -1)
                {
                    if (ge != -1)
                    {
                        GeometryData = SerkitData.Substring(gs, ge - gs).Trim();
                        StringTokenizer st = new StringTokenizer(GeometryData);
                        double w = Int32.Parse(st.NextToken()) * Scale;
                        double h = Int32.Parse(st.NextToken()) * Scale;
                        Width = w;
                        Height = h;
                        Background = null;
                        Foreground = new SolidColorBrush(Colors.Black);
                        string shape;
                        while ((shape = st.NextToken(';')) != null)
                        {
                            ParseShape(shape);
                        }
                    }
                    else
                    {
                        throw new GeometryNotDefinedException(Source, "Expecting EndGeometry");
                    }
                }
                else
                {
                    throw new GeometryNotDefinedException(Source);
                }
                int vs = SerkitData.IndexOf("Visual") + 6;
                int ve = SerkitData.LastIndexOf("EndVisual");
                if (vs != -1)
                {
                    if (ve != -1)
                    {
                        VisualData = SerkitData.Substring(vs, ve - vs).Trim();
                        StringTokenizer st = new StringTokenizer(GeometryData);
                        Background = null;
                        Foreground = new SolidColorBrush(Colors.Black);
                        string shape;
                        while ((shape = st.NextToken(';')) != null)
                        {

                            //ParseVisual(shape);
                        }
                    }
                }
                int ios = SerkitData.IndexOf("IO") + 8;
                int ioe = SerkitData.LastIndexOf("EndIO");
                if (ios != -1)
                {
                    if (ioe != -1)
                    {
                        IOData = SerkitData.Substring(ios, ioe - ios).Trim();
                        StringTokenizer st = new StringTokenizer(IOData);

                    }
                    else
                    {
                        throw new IONotDefinedException(Source, "Expecting EndIO");
                    }
                }
                else
                {
                    throw new IONotDefinedException(Source);
                }
            }
            catch (Exception e)
            {
                string s = e.Message + " \r\nData : ";
                foreach(object o in e.Data)
                {
                    s += "\r\n" + (string)o;
                }
                throw new SerkitFileFormatError(Source, s);
            }
        }

        public void ParseShape(string shape)
        {
            if (shape.StartsWith("Line"))
            {
                StringTokenizer stl = new StringTokenizer(shape);
                stl.NextToken('\"');
                double strokewidth = Double.Parse(stl.NextToken());
                string incolor = stl.NextToken('\"');
                incolor = incolor.Substring(1);
                byte a = From2DigitHexToByte(incolor.Substring(0, 2));
                byte r = From2DigitHexToByte(incolor.Substring(2, 2));
                byte g = From2DigitHexToByte(incolor.Substring(4, 2));
                byte b = From2DigitHexToByte(incolor.Substring(6, 2));
                double x1 = Double.Parse(stl.NextToken()) * Scale;
                double y1 = Double.Parse(stl.NextToken()) * Scale;
                double x2 = Double.Parse(stl.NextToken()) * Scale;
                double y2 = Double.Parse(stl.NextToken()) * Scale;
                Line line = new Line()
                {
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                    Stroke = new SolidColorBrush(Color.FromArgb(a, r, g, b)),
                    StrokeThickness = strokewidth
                };
                CanvasPanel.Children.Add(line);
                /*Console.WriteLine("Line { Color = " + incolor +
                                  "X1 = " + x1 +
                                  "Y1 = " + y1 +
                                  "X2 = " + x2 +
                                  "Y2 = " + y2 + " }");*/
            }
            else if (shape.StartsWith("FillPolygon"))
            {
                StringTokenizer stl = new StringTokenizer(shape);
                stl.NextToken('\"');
                double strokewidth = Double.Parse(stl.NextToken());
                string color1 = stl.NextToken();
                byte a1 = From2DigitHexToByte(color1.Substring(1, 2));
                byte r1 = From2DigitHexToByte(color1.Substring(3, 2));
                byte g1 = From2DigitHexToByte(color1.Substring(5, 2));
                byte b1 = From2DigitHexToByte(color1.Substring(7, 2));
                string color2 = stl.NextToken('\"');
                byte a2 = From2DigitHexToByte(color2.Substring(1, 2));
                byte r2 = From2DigitHexToByte(color2.Substring(3, 2));
                byte g2 = From2DigitHexToByte(color2.Substring(5, 2));
                byte b2 = From2DigitHexToByte(color2.Substring(7, 2));
                Polygon poly = new Polygon()
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(a1, r1, g1, b1)),
                    Fill = new SolidColorBrush(Color.FromArgb(a2, r2, g2, b2)),
                    StrokeThickness = strokewidth,
                    Opacity = 1.0
                };
                string token;
                while ((token = stl.NextToken()).Length != 0 && token != null)
                {
                    double x = Double.Parse(token) * Scale;
                    token = stl.NextToken();
                    double y = Double.Parse(token) * Scale;
                    poly.Points.Add(new Point() { X = x, Y = y });
                }
                CanvasPanel.Children.Add(poly);
                //Console.WriteLine("FillPolygon : { Stroke = " + poly.Stroke + ", Fill = " + poly.Fill + ", Point Count = " + poly.Points.Count + " }");
            }
            else if (shape.StartsWith("Polygon"))
            {
                StringTokenizer stl = new StringTokenizer(shape);
                stl.NextToken('\"');
                double strokewidth = Double.Parse(stl.NextToken());
                string color2 = stl.NextToken('\"');
                byte a2 = From2DigitHexToByte(color2.Substring(1, 2));
                byte r2 = From2DigitHexToByte(color2.Substring(3, 2));
                byte g2 = From2DigitHexToByte(color2.Substring(5, 2));
                byte b2 = From2DigitHexToByte(color2.Substring(7, 2));
                Polygon poly = new Polygon()
                {
                    Stroke = new SolidColorBrush(Color.FromArgb(a2, r2, g2, b2)),
                    Fill = null,
                    StrokeThickness = strokewidth,
                    Opacity = 1.0
                };
                string token;
                while ((token = stl.NextToken()).Length != 0 && token != null)
                {
                    double x = Double.Parse(token) * Scale;
                    token = stl.NextToken();
                    double y = Double.Parse(token) * Scale;
                    poly.Points.Add(new Point() { X = x, Y = y });
                }
                CanvasPanel.Children.Add(poly);
                //Console.WriteLine("Polygon : { Stroke = " + poly.Stroke + ", Fill = " + poly.Fill + ", Point Count = " + poly.Points.Count + " }");
            }
            else if (shape.StartsWith("FillEllipse"))
            {
                StringTokenizer stl = new StringTokenizer(shape);
                stl.NextToken('\"');
                double strokewidth = Double.Parse(stl.NextToken());
                string color1 = stl.NextToken();
                byte a1 = From2DigitHexToByte(color1.Substring(1, 2));
                byte r1 = From2DigitHexToByte(color1.Substring(3, 2));
                byte g1 = From2DigitHexToByte(color1.Substring(5, 2));
                byte b1 = From2DigitHexToByte(color1.Substring(7, 2));
                string color2 = stl.NextToken('\"');
                byte a2 = From2DigitHexToByte(color2.Substring(1, 2));
                byte r2 = From2DigitHexToByte(color2.Substring(3, 2));
                byte g2 = From2DigitHexToByte(color2.Substring(5, 2));
                byte b2 = From2DigitHexToByte(color2.Substring(7, 2));
                Ellipse ell = new Ellipse()
                {
                    Fill = new SolidColorBrush(Color.FromArgb(a2,r2,g2,b2)),
                    Stroke = new SolidColorBrush(Color.FromArgb(a1,r1,g1,b1)),
                    StrokeThickness = strokewidth
                };
                double x = Double.Parse(stl.NextToken()) * Scale;
                double y = Double.Parse(stl.NextToken()) * Scale;
                double a = Double.Parse(stl.NextToken()) * Scale;
                double b;
                string check = stl.NextToken();
                if (check == null || check.Length == 0)
                    b = a;
                else
                    b = Double.Parse(check) * Scale;
                Canvas.SetLeft(ell, x - a / 2);
                Canvas.SetTop(ell, y - b / 2);
                ell.Width = a;
                ell.Height = b;
                CanvasPanel.Children.Add(ell);
                //Console.WriteLine("FillEllipse : { Stroke = " + ell.Stroke + ", Fill = " + ell.Fill + " }");
            }
            else if (shape.StartsWith("Ellipse"))
            {
                StringTokenizer stl = new StringTokenizer(shape);
                stl.NextToken('\"');
                double strokewidth = Double.Parse(stl.NextToken());
                string color2 = stl.NextToken('\"');
                byte a2 = From2DigitHexToByte(color2.Substring(1, 2));
                byte r2 = From2DigitHexToByte(color2.Substring(3, 2));
                byte g2 = From2DigitHexToByte(color2.Substring(5, 2));
                byte b2 = From2DigitHexToByte(color2.Substring(7, 2));
                Ellipse ell = new Ellipse()
                {
                    Fill = new SolidColorBrush(Color.FromArgb(a2, r2, g2, b2)),
                    Stroke = null,
                    StrokeThickness = strokewidth
                };
                double x = Double.Parse(stl.NextToken()) * Scale;
                double y = Double.Parse(stl.NextToken()) * Scale;
                double a = Double.Parse(stl.NextToken()) * Scale;
                double b;
                string check = stl.NextToken();
                if (check == null || check.Length == 0)
                    b = a;
                else
                    b = Double.Parse(check) * Scale;
                Canvas.SetLeft(ell, x - a / 2);
                Canvas.SetTop(ell, y - b / 2);
                ell.Width = a;
                ell.Height = b;
                CanvasPanel.Children.Add(ell);
                //Console.WriteLine("Ellipse : { Stroke = " + ell.Stroke + ", Fill = " + ell.Fill + " }");
            }
            else if (shape.StartsWith("Text"))
            {
                StringTokenizer st = new StringTokenizer(shape);
                st.NextToken('\"');
                string s = st.NextToken('\"');
                double size = Double.Parse(st.NextToken())*Scale;
                double x = Double.Parse(st.NextToken())*Scale;
                double y = Double.Parse(st.NextToken())*Scale;
                TextBlock t = new TextBlock() { Text = s, FontSize = size };
                Canvas.SetLeft(t, x);
                Canvas.SetTop(t, y);
                CanvasPanel.Children.Add(t); 
                //Console.WriteLine("Text : { " + t.Text + " }");
            }
        }

        public void ParseVisual(string visual)
        {

        }

        public byte From2DigitHexToByte(string hex)
        {
            char c1 = hex[0];
            char c0 = hex[1];
            c0 = Char.ToUpper(c0);
            c1 = Char.ToUpper(c1);
            int val0 = (int)(c0) - 48;
            int val1 = (int)(c1) - 48;
            if (!Char.IsNumber(c0))
                val0 = (int)c0 - 65 + 10;
            if (!Char.IsNumber(c1))
                val1 = (int)c1 - 65 + 10;
            return (byte)(val1 * 16 + val0);
        }

        private Point RoundToGridPoint(Point p)
        {
            int gh = (int)Properties.Settings.Default["GridHeight"];
            int gw = (int)Properties.Settings.Default["GridWidth"];
            int X = (int)p.X;
            int Y = (int)p.Y;
            int nX, nY;
            if (p.X % gw < gw / 2)
                nX = X - (X % gw);
            else
                nX = X + (gw - X % gw);

            if (p.Y % gh < gh / 2)
                nY = Y - (Y % gh);
            else
                nY = Y + (gh - Y % gh);
            return new Point(nX, nY);
        }

        private void UserControl_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MouseDownLocation = Mouse.GetPosition(SelectionRectangle);
        }

        private void UserControl_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            _dragged = true;
        }

        private void UserControl_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _dragged = false;
        }

        private void SelectionRectangle_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_selected && Mouse.LeftButton == MouseButtonState.Pressed && _dragged)
            {
                Point DragLocation = RoundToGridPoint(Mouse.GetPosition((Canvas)(Parent)));
                double nX = DragLocation.X - MouseDownLocation.X;
                double nY = DragLocation.Y - MouseDownLocation.Y;
                Canvas.SetLeft(this, nX);
                Canvas.SetTop(this, nY);
            }
        }

        private void ResizerMouseDown(object sender, MouseButtonEventArgs e)
        {
            OriginalHeight = Height;
            OriginalWidth = Width;
            OX = Canvas.GetLeft((UIElement)sender) - 2;
            OY = Canvas.GetTop((UIElement)sender) - 2;
        }
    }

    public class GeometryNotDefinedException : Exception
    {
        public GeometryNotDefinedException(){ }

        public GeometryNotDefinedException(FileInfo errorFile)
        {
            Data.Add("ErrorFile", errorFile);
            Data.Add("CustomMessage", "");
        }

        public GeometryNotDefinedException(FileInfo errorFile, string message)
        {
            Data.Add("ErrorFile", errorFile);
            Data.Add("CustomMessage", message);
        }

        public override string Message
        {
            get
            {
                FileInfo f = (FileInfo) Data["ErrorFile"];
                if(f!=null)
                    return "The file " + f.FullName + " contains errors.\r\n\tGeometry Not Defined.\r\n" + Data["CustomMessage"];
                return "The Geometry code contains errors.\r\n\tGeometry Not Defined.\r\n" + Data["CustomMessage"];
            }
        }
    }

    public class IONotDefinedException : Exception
    {
        public IONotDefinedException(){ }

        public IONotDefinedException(FileInfo errorFile)
        {
            Data.Add("ErrorFile", errorFile);
            Data.Add("CustomMessage", "");
        }

        public IONotDefinedException(FileInfo errorFile, string message)
        {
            Data.Add("ErrorFile", errorFile);
            Data.Add("CustomMessage", message);
        }

        public override string Message
        {
            get
            {
                FileInfo f = (FileInfo) Data["ErrorFile"];
                if (f != null)
                    return "The file " + f.FullName + " contains errors.\r\n\tIO Not Defined.\r\n" + Data["CustomMessage"];
                return "The IO code contains errors.\r\n\tIO Not Defined.\r\n" + Data["CustomMessage"];
            }
        }
    }

    public class SerkitFileFormatError : Exception
    {
        public SerkitFileFormatError(){ }

        public SerkitFileFormatError(FileInfo errorFile)
        {
            Data.Add("ErrorFile", errorFile);
            Data.Add("CustomMessage", "");
        }

        public SerkitFileFormatError(FileInfo errorFile, string message)
        {
            Data.Add("ErrorFile", errorFile);
            Data.Add("CustomMessage", message);
        }

        public override string Message
        {
            get
            {
                FileInfo f = (FileInfo) Data["ErrorFile"];
                if (f != null)
                    return "The file " + f.FullName + " contains errors.\r\n" + Data["CustomMessage"];
                return "The file contains errors.\r\n" + Data["CustomMessage"];
            }
        }
    }

}
