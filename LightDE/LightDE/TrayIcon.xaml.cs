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

namespace LightDE
{
    /// <summary>
    /// Interaction logic for TrayIcon.xaml
    /// </summary>
    public partial class TrayIcon : UserControl
    {
        NOTIFYITEMICON thisIcon;
        public TrayIcon(NOTIFYITEMICON s)
        {
            InitializeComponent();
            thisIcon = s;
            Icon.Source = s.image;

        }

        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            SpaceReserver.ShowWindow((IntPtr)int.Parse(thisIcon.hWnd), SpaceReserver.WindowShowStyle.Show);
        }
    }
}
