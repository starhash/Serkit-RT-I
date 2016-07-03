using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
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
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static TreeViewItem Default;

        public static TreeViewItem GetDefaultRoot()
        {
            TreeViewItem root = new TreeViewItem() { Header = "Default" };

            TreeViewItem Current = null;
            TextReader t = null;
            TreeViewItem gate = null;
            SerkitComponent serkitcomp = null;

            Current = new TreeViewItem() { Header = "Gates" };
            Current.FontSize = 12;

            #region Gates

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Not));
                gate = new TreeViewItem() { Header = " Not" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_And));
                gate = new TreeViewItem() { Header = " And" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Or));
                gate = new TreeViewItem() { Header = " Or" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Nand));
                gate = new TreeViewItem() { Header = " Nand" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Nor));
                gate = new TreeViewItem() { Header = " Nor" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Xor));
                gate = new TreeViewItem() { Header = " Xor" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Xnor));
                gate = new TreeViewItem() { Header = " Xnor" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

            #endregion

            root.Items.Add(Current);

            Current = new TreeViewItem() { Header = "FlipFlops" };
            Current.FontSize = 12;

            #region Flip Flops

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_FlipFlops_DFlipFlop));
                gate = new TreeViewItem() { Header = " D Flip Flop" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_FlipFlops_TFlipFlop));
                gate = new TreeViewItem() { Header = " T Flip Flop" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_FlipFlops_RSFlipFlop));
                gate = new TreeViewItem() { Header = " RS Flip Flop" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

            #endregion

            root.Items.Add(Current);
            return root;
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            Default = new TreeViewItem() { Header = "Default" };

            TreeViewItem Current = null;
            TextReader t = null;
            TreeViewItem gate = null;
            SerkitComponent serkitcomp = null;

            Current = new TreeViewItem() { Header = "Gates" };
            Current.FontSize = 12;

            #region Gates

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Not));
                gate = new TreeViewItem() { Header = " Not" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_And));
                gate = new TreeViewItem() { Header = " And" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Or));
                gate = new TreeViewItem() { Header = " Or" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Nand));
                gate = new TreeViewItem() { Header = " Nand" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Nor));
                gate = new TreeViewItem() { Header = " Nor" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Xor));
                gate = new TreeViewItem() { Header = " Xor" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_Gates_Xnor));
                gate = new TreeViewItem() { Header = " Xnor" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

            #endregion
            
            Default.Items.Add(Current);

            Current = new TreeViewItem() { Header = "Flip Flops" };
            Current.FontSize = 12;

            #region Flip Flops

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_FlipFlops_DFlipFlop));
                gate = new TreeViewItem() { Header = " D Flip Flop" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_FlipFlops_TFlipFlop));
                gate = new TreeViewItem() { Header = " T Flip Flop" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

                t = new StreamReader(new MemoryStream(Serkit_RT_I.Properties.Resources.Default_FlipFlops_RSFlipFlop));
                gate = new TreeViewItem() { Header = " RS Flip Flop" };
                gate.Tag = t.ReadToEnd();
                gate.FontFamily = new FontFamily("Segoe UI Symbol");
                serkitcomp = new SerkitComponent();
                serkitcomp.ParseText((String)gate.Tag, true);
                gate.ToolTip = new ToolTip() { Content = serkitcomp };
                Current.Items.Add(gate);

            #endregion

            Default.Items.Add(Current);
        }

        public async void LoadTool(string path)
        {
            FileStream fs = new FileInfo(path).OpenRead();
            StreamReader sr = new StreamReader(fs);
            TextReader text = sr;
            string s = await text.ReadToEndAsync();
        }
    }
}
