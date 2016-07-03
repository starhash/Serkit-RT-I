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
    /// Interaction logic for TextShapeChooser.xaml
    /// </summary>
    public partial class TextShapeChooser : UserControl
    {
        public delegate void SelectionHandler(TextShapeChooser tsc);
        public event SelectionHandler SelectionPerformed;

        public string Type
        {
            get
            {
                ComboBoxItem c = (ComboBoxItem)ShapeChooser.SelectedItem;
                return (string)c.Content;
            }
        }
        public double Size
        {
            get
            {
                return Double.Parse(StrokeThickness.Text);
            }
        }
        public int FontSize
        {
            get
            {
                return Int32.Parse(StrokeThickness.Text);
            }
        }
        public SolidColorBrush Stroke
        {
            get
            {
                return (SolidColorBrush)StrokeColorText.BorderBrush;
            }
        }
        public SolidColorBrush Fill
        {
            get
            {
                return (SolidColorBrush)FillColorText.BorderBrush;
            }
        }
        public FontFamily FontFace
        {
            get
            {
                return new FontFamily(FontChooser.Text);
            }
        }

        public TextShapeChooser()
        {
            InitializeComponent();
            SelectionPerformed += TextShapeChooser_SelectionPerformed;
        }

        void TextShapeChooser_SelectionPerformed(TextShapeChooser tsc)
        {
            
        }

        public string GetShapeString()
        {
            string s = "";
            if (ShapeChooser.SelectedIndex == 0)
            {
                s += "Line \"";
                s += Size + " " + StrokeColorText.Text + "\"";
                s += " [Point start] [Point end]";
            }
            else if (ShapeChooser.SelectedIndex == 1)
            {
                s += "Polygon \"";
                s += Size + " " + StrokeColorText.Text + "\"";
                s += " [Point() vertices]";
            }
            else if (ShapeChooser.SelectedIndex == 3)
            {
                s += Type + " \"";
                s += Size + " " + StrokeColorText.Text + "\"";
                s += " [Point center] [Double radius]";
            }
            else if (ShapeChooser.SelectedIndex == 2)
            {
                s += "FillPolygon \"";
                s += Size + " " + StrokeColorText.Text + " " + FillColorText.Text + "\"";
                s += " [Point() vertices]";
            }
            else if (ShapeChooser.SelectedIndex == 4)
            {
                s += "FillEllipse \"";
                s += Size + " " + StrokeColorText.Text + " " + FillColorText.Text + "\"";
                s += " [Point center] [Double radius]";
            }
            else
            {
                s += "Text \"[String text]\"";
                s += " [Integer fontsize] [Point location]";
            }
            return s;
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

        private void ColorText_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox)sender;
            try
            {
                if (t.Text.Length == 0)
                    t.Text = "#FF000000";
                string incolor = t.Text;
                incolor = incolor.Substring(1);
                byte a = From2DigitHexToByte(incolor.Substring(0, 2));
                byte r = From2DigitHexToByte(incolor.Substring(2, 2));
                byte g = From2DigitHexToByte(incolor.Substring(4, 2));
                byte b = From2DigitHexToByte(incolor.Substring(6, 2));
                t.BorderBrush = new SolidColorBrush(Color.FromArgb(a, r, g, b));
                t.ToolTip = null;
                t.Background = new SolidColorBrush(Colors.White);
            }
            catch(Exception ex)
            {
                t.ToolTip = new ToolTip() { Content = "Error : Invalid Color code.", Visibility = System.Windows.Visibility.Visible };
                t.Background = new SolidColorBrush(Color.FromArgb(255, 255, 200, 200));
                t.BorderBrush = null;
            }
        }

        private void ShapeChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if (ShapeChooser.SelectedIndex == 0 || ShapeChooser.SelectedIndex == 1 || ShapeChooser.SelectedIndex == 3)
                {
                    FillText.Visibility = System.Windows.Visibility.Collapsed;
                    FillColorText.Visibility = System.Windows.Visibility.Collapsed;
                    FontChooser.Visibility = System.Windows.Visibility.Collapsed;
                    StrokeThickness.ToolTip = new ToolTip()
                    {
                        Content = new Line() { X1 = 0, Y1 = 0, X2 = 50, Y2 = 0, StrokeThickness = Int32.Parse(this.StrokeThickness.Text) }
                    };
                }
                else if (ShapeChooser.SelectedIndex == 2 || ShapeChooser.SelectedIndex == 4)
                {
                    FillText.Visibility = System.Windows.Visibility.Visible;
                    FillColorText.Visibility = System.Windows.Visibility.Visible;
                    FontChooser.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    StrokeText.Content = "Font";
                    StrokeThickness.Text = "12";
                    FillText.Visibility = System.Windows.Visibility.Collapsed;
                    FillColorText.Visibility = System.Windows.Visibility.Collapsed;
                    FontChooser.Visibility = System.Windows.Visibility.Visible;
                }
            }
        }

        private void StrokeThickness_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            SelectionPerformed(this);
        }
    }
}
