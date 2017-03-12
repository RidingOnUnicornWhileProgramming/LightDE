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

namespace LightDE.Settings
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : UserControl
    {
        object val;

        public Setting(string name, ref string value)
        {
            InitializeComponent();
            settingname.Content = name;
            settingval.Text = value.ToString();

            val = value;
            // TODO now we can replace TextBoxes if needed - f.e. use toggles if value.Type==boolean, NumericUpDowns if Type==number
        }

        public Setting(string name, ref bool value)
        {
            InitializeComponent();
            settingname.Content = name;
            settingval.Text = value.ToString();
            // TODO check if save works
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            settingval.TextChanged += (object s, TextChangedEventArgs es) => val = Convert.ChangeType(settingval.Text, val.GetType());
        }
    }
}
