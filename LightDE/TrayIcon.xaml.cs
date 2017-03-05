using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        private DateTime _lastRClick;
        private DateTime _lastLClick;

        public TrayIcon(NOTIFYITEMICON s)
        {
            InitializeComponent();
            thisIcon = s;
            Icon.Source = s.image;
            Icon.Height = 16;
        }
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint callback, uint wParam, uint lParam);
        private void Icon_MouseUp(object sender, MouseButtonEventArgs e)
        {
            uint WM_LBUTTONUP = 0x202;
            uint WM_LBUTTONDBLCLK = 0x203;
            uint WM_RBUTTONUP = 0x205;
            uint WM_RBUTTONDBLCLK = 0x206;


            if (e.ChangedButton == System.Windows.Input.MouseButton.Left)
            {
                if (DateTime.Now.Subtract(_lastLClick).TotalSeconds < 1)
                {
                    PostMessage(thisIcon.original.hWnd, thisIcon.original.uCallbackMessage, thisIcon.UID, WM_LBUTTONDBLCLK);
                }
                else
                {
                    PostMessage(thisIcon.hWnd, thisIcon.original.uCallbackMessage, thisIcon.UID, WM_LBUTTONUP);
                }
                _lastLClick = DateTime.Now;
            }
            else if (e.ChangedButton == System.Windows.Input.MouseButton.Right)
            {
                if (DateTime.Now.Subtract(_lastRClick).TotalSeconds < 1)
                {
                    PostMessage(thisIcon.hWnd, thisIcon.original.uCallbackMessage, thisIcon.UID, WM_RBUTTONDBLCLK);
                }
                else
                {
                    PostMessage(thisIcon.hWnd, thisIcon.original.uCallbackMessage, thisIcon.UID, WM_RBUTTONUP);
                }
                _lastRClick = DateTime.Now;
            }

            Console.WriteLine("Mouse up (" + e.ChangedButton.ToString() + ") on trayicon " + thisIcon.Title);
    }
    }
}
