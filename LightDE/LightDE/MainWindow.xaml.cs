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
        AppsListing AppManager = new AppsListing();
        public PanelPos PanelPosition = PanelPos.Top;
        public int PanelHeight = 30;
        public int PanelWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        public MainWindow()
        {
            this.Show();
            InitializeComponent();
            SetPanelPos(PanelPosition);
            AppManager.GetItems();
            GetApps();
            WindowManager wm = new WindowManager(AddNewTaskItem);
            NotifyIconManager m = new NotifyIconManager();
            m.DoStuff();
            Parallel.ForEach<NOTIFYITEMICON>(m.Programs, x => Dispatcher.Invoke(() => NotifyiconHolder.Children.Add(new TrayIcon(x))));

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

        private void Window_TitleChanged(object sender, EventArgs e)
        {
            Parallel.ForEach<MenuItem>(ProcMenu.Items.Cast<MenuItem>(), x => Dispatcher.Invoke(() => x.Header = GetTitle(int.Parse(x.Tag.ToString()))));
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
    }
    public enum PanelPos
    {
        Top = 0,
        Bottom = 1,
        Left = 2,
        Right = 3
    }
}