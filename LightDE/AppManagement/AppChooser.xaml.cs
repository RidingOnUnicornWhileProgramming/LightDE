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
            }
        }

        public AppChooser()
        {
            InitializeComponent();
            FillBox();
        }
        public void FillBox()
        {
            new Thread(new ThreadStart(
                () =>
                {
                    foreach (xApp x in apps.GetItems())
                    {
                        try
                        {
                            Dispatcher.Invoke(
                            () =>
                            {

                                ListBoxItem it = new ListBoxItem();
                                it.Content = x.name;
                                it.IsSelected = appslist.Any(o => o.ToString() == x.name);
                                appbox.Items.Add(it);
                            });
                        }
                        catch { }
                    }
                })).Start() ;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            JArray arrap = new JArray();

            new Thread(new ThreadStart(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    List<xApp> ap = apps.GetItems();
                    foreach (ListBoxItem s in appbox.Items)
                    {
                        if (s.IsSelected)
                        {
                            arrap.Add(s.Content.ToString());
                            MainWindow.appslist.Add(ap.Find(o => o.name == s.Content.ToString()));
                        }
                    }
                    appslist = arrap;
                    this.Close();
                });
                new Thread(new ThreadStart(MainWindow.instance.GetApps)).Start();
            })).Start();

        }

    }
}
