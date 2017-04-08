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

namespace LightDE.SidePanel
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            animationshow.From = System.Windows.SystemParameters.FullPrimaryScreenWidth;
            animationshow.To = System.Windows.SystemParameters.FullPrimaryScreenWidth -495;

            Left = System.Windows.SystemParameters.FullPrimaryScreenWidth - 495;
            Height = System.Windows.SystemParameters.FullPrimaryScreenHeight - 30;
            Top = 30;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Hide();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {

        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {

        }
    }
}
