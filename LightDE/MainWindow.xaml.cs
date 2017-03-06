/*
    Panel - Main Panel Class
    Copyright (C) 2017  Piotr 'MiXer' Mikstacki

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using System.Threading.Tasks;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using DE.WindowManagement;
using System.Timers;
using AudioSwitcher.AudioApi.CoreAudio;
using WMPLib;
using LightDE.Desktop;
namespace LightDE
{
    /// <summary>
    /// Main Class for Panel
    /// </summary>
    /// //TODO END RADIAL MENU
    /// 
    [System.Serializable()]
    public partial class MainWindow : MetroWindow
    {
        private NotifyIconManager notifyiconmanager; // keep alive callbacks
        CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
        public IWMPMedia currentMedia { get; set; }

        AppsListing AppManager = new AppsListing();
        public PanelPos PanelPosition = PanelPos.Top;
        public int PanelHeight = 30;
        public int PanelWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        public System.Timers.Timer ClockTimer = new System.Timers.Timer(1000);
        public MainWindow()
        {
            this.Show();
            InitializeComponent();
            SetPanelPos(PanelPosition);
            AppManager.GetItems();
            GetApps();
            Clock.Content = DateTime.Now.ToString("dddd, dd.MM.yyyy HH:mm:ss");
                
            ClockTimer.Elapsed += (object sender, ElapsedEventArgs e) => { Dispatcher.Invoke(() => Clock.Content = DateTime.Now.ToString("dddd, dd.MM.yyyy HH:mm:ss")); };
            ClockTimer.Start();
            WindowManager wm = new WindowManager(AddNewTaskItem);
            notifyiconmanager = new NotifyIconManager(AddNewNotification);
            DesktopD D = new DesktopD();
            D.Show();
        }
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        private GUIItem AddNewTaskItem(WinHandle window)
        {
            window.TitleChanged += Window_TitleChanged;
            var g = new GUIItem();
            Application.Current.Dispatcher.Invoke(() => {
                MenuItem s = new MenuItem();
                s.Click += (object sender, RoutedEventArgs e) => { window.MaximizeMinimize(); };
                s.Header = window.Title;
                s.Tag = window.Ptr.ToString();
                Image m = new Image();
                var handle = window.WindowIcon.ToBitmap().GetHbitmap();
                try
                {
                    m.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    s.Icon = m;
                }
                finally { DeleteObject(handle); }
                g.Destroy = () => { Application.Current.Dispatcher.Invoke(() => ProcMenu.Items.Remove(ProcMenu.Items.Cast<MenuItem>().Where(x => x.Tag == s.Tag).First())); };
                ProcMenu.Items.Add(s);
            });
            return g;
        }

        private GUIItem AddNewNotification(NOTIFYITEMICON icon)
        {
            var g = new GUIItem();
            Application.Current.Dispatcher.Invoke(() => {
                var obj = new TrayIcon(icon);
                NotifyiconHolder.Children.Add(obj);
                g.Destroy = () => { Application.Current.Dispatcher.Invoke(() => NotifyiconHolder.Children.Remove(obj)); };
            });
            return g;
        }

        private void Window_TitleChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < ProcMenu.Items.Count; i++)
            {
                var p = ProcMenu.Items[i] as MenuItem;
                Dispatcher.Invoke(() => p.Header = GetTitle(int.Parse(p.Tag.ToString())));
            }
        }
        [DllImport("user32.dll")]
        private static extern int GetWindowText(int hWnd, StringBuilder title, int size);

        public string GetTitle(int hwnd)
        {

            StringBuilder title = new StringBuilder(256);
            GetWindowText(hwnd, title, 256);
            return title.ToString();
        }
        public void SetPanelPos(PanelPos panelPos)// Sets working area and checks if panels arent overlaying
        {
            SpaceReserver.MakeNewDesktopArea(0, PanelHeight, 0, 0);
            PanelPosition = PanelPos.Top;
            this.Left = 0;
            this.Top = 0;
            this.Width = PanelWidth;
            this.Height = PanelHeight;
        }

        async void GetApps()
        {
            await Task.Factory.StartNew(() =>
           {
              Parallel.ForEach<xApp>(AppManager.GetItems(), item => Dispatcher.Invoke(() =>
              {
                  try
                  {
                      MenuItem s = new MenuItem();
                      s.Click += (object sender, RoutedEventArgs e) => { Process.Start(item.Path); };
                      s.Header = item.name;
                      Image m = new Image();
                      var handle = item.icon.GetHbitmap();
                      try
                      {
                          m.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                          s.Icon = m;
                      }
                      finally { DeleteObject(handle); }
                      menu.Items.Add(s);
                  }
                  catch
                  {
                      MessageBox.Show("Unable to run item! Make sure the path is correct");
                  }
              }));
           });
        }
        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
           // Task.Factory.StartNew(() =>
           // {
           // Dispatcher.Invoke(() => menu.Items.Clear());
//Parallel.ForEach<xApp>(Apps.Where<xApp>(x=>Dispatcher.Invoke(() => x.name.Contains(SearchBox.Text))), item => Dispatcher.Invoke(() => menu.Items.Add(new appIcon(item.name, item.icon, item.Path))));
          //  });
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic));

        }

        private void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures));

        }

        private void MenuItem_Click_3(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));

        }

        private void MenuItem_Click_4(object sender, RoutedEventArgs e)
        {
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

        }

        private void Clock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {

        }

        private void Volume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Thread p = new Thread(new ThreadStart(() =>
            {
                try
                {
                    defaultPlaybackDevice.Volume = e.NewValue;
                }
                catch {
                    Console.WriteLine("Can't find output devicde!");
                }
            }));
            p.Start();
        }
        [DllImport("user32.dll")]
        public static extern void keybd_event(byte virtualKey, byte scanCode, uint flags, IntPtr extraInfo);
        public const int KEYEVENTF_EXTENTEDKEY = 1;
        public const int KEYEVENTF_KEYUP = 0;
        public const int VK_MEDIA_NEXT_TRACK = 0xB0;// code to jump to next track
        public const int VK_MEDIA_PLAY_PAUSE = 0xB3;// code to play or pause a song
        public const int VK_MEDIA_PREV_TRACK = 0xB1;// code to jump to prev track
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, IntPtr wParam, IntPtr lParam);
        private void Volume_MouseUp(object sender, MouseButtonEventArgs e)
        {
            
        }
        //Currently doesn't work, I must find another way...
        private void BackWard(object sender, RoutedEventArgs e)
        {
            SendMessage(Process.GetCurrentProcess().MainWindowHandle, 0x319, (IntPtr)11, (IntPtr)11);
            Console.WriteLine("Back");
        }

        private void PlayPause(object sender, RoutedEventArgs e)
        {
            SendMessage(Process.GetCurrentProcess().MainWindowHandle, 0x319, (IntPtr)14, (IntPtr)14);
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            SendMessage(Process.GetCurrentProcess().MainWindowHandle, 0x319, (IntPtr)12, (IntPtr)12);
        }
    }
    public enum PanelPos
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 3
    }
}