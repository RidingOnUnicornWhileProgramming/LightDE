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
using System.Windows.Shapes;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace LightDE.AppManagement
{
    /// <summary>
    /// Interaction logic for AppChooser.xaml
    /// </summary>
    public partial class AppChooser : Window
    {
        public AppsListing apps = new AppsListing();
       public Newtonsoft.Json.Linq.JArray appslist
        {
            get
            {
                if (MainWindow.config.GetVar("Apps", "appnames") == null)
                {
                    return new JArray();
                }
                else
                {
                    return MainWindow.config.GetVar("Apps", "appnames") as JArray;
                }
            }
            set
            {
                MainWindow.config.SetVar("Apps", "appnames", value);
                appslist = value;
            }
        }

        public AppChooser()
        {
            InitializeComponent();
            FillBox();
        }
        public void FillBox()
        {
            foreach (xApp x in apps.GetItems())
            {
                try
                {
                    ListBoxItem it = new ListBoxItem();
                    it.Content = x.name;
                    it.IsSelected = appslist.Any(o => o.ToString() == x.name);
                    appbox.Items.Add(it);
                }
                catch { }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<xApp> ap = apps.GetItems();
            foreach (ListBoxItem s in appbox.Items)
            {
                if (s.IsSelected)
                {
                    MainWindow.appslist.Add(ap.Find(o => o.name == s.Content.ToString()));
                }
            }
            new Thread(new ThreadStart(MainWindow.instance.GetApps)).Start();

        }

    }
}
