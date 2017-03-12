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
using LightDE.Settings;

namespace LightDE.AppManagement
{
    /// <summary>
    /// Interaction logic for AppChooser.xaml
    /// </summary>
    public partial class AppChooser : Window
    {
        public AppsListing apps = new AppsListing();
        public List<string> appslist
        {
            get
            {
                return Config.Current.V1.Apps_AppNames;
            }
            set
            {
                Config.Current.V1.Apps_AppNames = value; // TODO is it supposed to stay like this?
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
                })).Start();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<string> arrap = new List<string>();
            MainWindow.appslist.Clear();
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
                            MainWindow.appslist = MainWindow.appslist.Distinct<xApp>().ToList<xApp>();
                        }
                    }
                    this.appslist = arrap;
                    this.Close();
                    Config.Current.Save();
                });
                new Thread(new ThreadStart(MainWindow.instance.GetApps)).Start();
            })).Start();

        }

    }
}