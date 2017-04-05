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

namespace LightDE.UI
{
    /// <summary>
    /// Interaction logic for AppDrawer.xaml
    /// </summary>
    public partial class AppDrawer : Window
    {
        //TODO ADD SEARCHING FOR APPS AND MAKING ICON OUT OF IT

        public AppDrawer()
        {
            InitializeComponent();
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Hide();
        }
    }
    public class AppDrawerItem
}
