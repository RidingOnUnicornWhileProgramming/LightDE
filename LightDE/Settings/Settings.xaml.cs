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
namespace LightDE.Settings
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    /// TODO: wrap config from config manager, make controls according to type of an object, for example: bool - toggle control etc...
    public partial class Settings : MetroWindow
    {
        public Settings()
        {
            InitializeComponent();

            setControl.DataContext = Config.Current.SettingsWnd();
            setControl.SelectedIndex = 0;
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            Config.Current.Save();
        }

        // TODO else Config.Current.Reload()
    }
}
