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

namespace LightDE.Config
{
    /// <summary>
    /// Interaction logic for Setting.xaml
    /// </summary>
    public partial class Setting : UserControl
    {
        string name, value;
        public Setting(string name, string value)
        {
            InitializeComponent();
            settingname.Content = name;
            settingval.Text = value;
            this.name = name;
            this.value = value;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            settingval.TextChanged += (object s, TextChangedEventArgs es) => MainWindow.config.SetVar(name.Split(new char[] { '_' })[0], name.Split(new char[] { '_' })[1], settingval.Text);
            MainWindow.config.Serialize();
        }
        public void Save()
        {
            MainWindow.config.SetVar(name.Split(new char[] { '_' })[0], name.Split(new char[] { '_' })[1], settingval.Text.Trim(new char[] { '\n', '\\' }));
        }

    }
}
