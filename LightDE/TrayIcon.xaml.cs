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
using LightDE.WindowManagement;
using System.Threading;

namespace LightDE
{
    /// <summary>
    /// Interaction logic for thisIcon.xaml
    /// </summary>
    public partial class TrayIcon : UserControl
    {


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hwnd, int wMsg, int wParam, int lParam);
        uint WM_LBUTTONUP = 0x202;
        uint WM_LBUTTONDBLCLK = 0x203;
        uint WM_RBUTTONUP = 0x205;
        uint WM_RBUTTONDBLCLK = 0x206;
        NOTIFYITEMICON thisIcon;
        public TrayIcon(NOTIFYITEMICON s)
        {
            InitializeComponent();
            thisIcon = s;
            Icon.Source = s.image;
            Icon.MouseLeftButtonUp += Icon_MouseLeftButtonUp;
        }

        private void Icon_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WINAPI.ShowWindow((IntPtr)int.Parse(thisIcon.hWnd), 1);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowTextLength(HandleRef hWnd);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowText(HandleRef hWnd, StringBuilder lpString, int nMaxCount);
        private void Icon_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Console.WriteLine(thisIcon.hWnd);
            Console.WriteLine(NotifyIconManager.PostMessage((IntPtr)int.Parse(thisIcon.hWnd), thisIcon.original.uCallbackMessage, thisIcon.uID, WM_RBUTTONDBLCLK));
            Thread.Sleep(500);
            Console.WriteLine(NotifyIconManager.PostMessage((IntPtr)int.Parse(thisIcon.hWnd), thisIcon.original.uCallbackMessage, thisIcon.uID, 0x205));
        }
    }
}
