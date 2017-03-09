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
using MahApps.Metro.Controls;
namespace LightDE.Config
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    /// TODO: wrap config from config manager, make controls according to type of an object, for example: bool - toggle control etc...
    public partial class Settings : MetroWindow
    {
        public List<Setting> settings = new List<Setting>();
        public Settings()
        {
            InitializeComponent();
            ParseSettings();
        }
        public void ParseSettings()
        {
            List<TabItem> tabitems = new List<TabItem>();
            foreach (string p in MainWindow.config.GetComponents())
            {
                TabItem t = new TabItem();
                t.Name = p;
                t.Header = p;
                foreach (KeyValuePair<string, object> k in MainWindow.config.GetConfig(p))
                {
                    t.Content = new StackPanel();
                    var m = t.Content as StackPanel;
                    Setting s = new Setting(k.Key, k.Value.ToString());
                    settings.Add(s);
                    
                    m.Children.Add(s);
                }
                tabitems.Add(t);
            }
            setControl.DataContext = tabitems;
            setControl.SelectedIndex = 0;
            
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            foreach (Setting s in settings)
            {
                s.Save();
            }
        }
    }
}
