using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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

namespace LightDE
{
    /// <summary>
    /// Interaction logic for appIcon.xaml
    /// </summary>
    public partial class appIcon : UserControl
    {
        public string Path;
        public appIcon()
        {
            InitializeComponent();
        }
        public appIcon(string Name, string icon, string Path)
        {
            InitializeComponent();
            ImageSource s = new BitmapImage(new Uri(icon));
            this.icon.Source = s;
            this.name.Content = Name;
            this.Path = Path;
            this.Loaded += Icon_Loaded;
        }


        private void Icon_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        public void run(object sender, MouseButtonEventArgs e)
        {
            Run();
        }
        public void Run()
        {
            try
            {
                Process.Start(Path);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not find the path!");
            }
        }
    }
}
