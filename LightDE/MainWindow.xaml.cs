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
using LightDE.Config;
using MaterialDesignThemes;
using LightDE.AppManagement;
using Newtonsoft.Json.Linq;

namespace LightDE
{
    /// <summary>
    /// Main Class for Panel
    /// </summary>
    /// //TODO END RADIAL MENU
    /// 
    [System.Serializable()]
    public partial class MainWindow : Window
    {
        public static ConfigManager config;
        static AppChooser ap;
        public static List<xApp> appslist;
        private NotifyIconManager notifyiconmanager; // keep alive callbacks
        CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
        public IWMPMedia currentMedia { get; set; }
        public static AppsListing AppManager;
        public PanelPos PanelPosition = PanelPos.Top;
        public int PanelHeight = 30;
        public int PanelWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        public System.Timers.Timer ClockTimer = new System.Timers.Timer(1000);
        public static MainWindow instance;
        public MainWindow()
        {
            
            config = new ConfigManager();
            config.GetFile();
            DesktopD D = new DesktopD();
            D.Show();
            this.Show();
            InitializeComponent();
            Clock.Header = DateTime.Now.ToString("HH:mm:ss");
            appslist = new List<xApp>();
            ap = new AppChooser();
            ap.Show();
            SetPanelPos(PanelPosition);
            AppManager = new AppsListing();
            ClockTimer.Elapsed += (object sender, ElapsedEventArgs e) => { Dispatcher.Invoke(() => Clock.Header = DateTime.Now.ToString("HH:mm:ss")); };
            ClockTimer.Start();
            WindowManager wm = new WindowManager(AddNewTaskItem);
            notifyiconmanager = new NotifyIconManager(AddNewNotification);
            D.InitializeDesktop();
            Volume.Value = defaultPlaybackDevice.Volume;

            new Thread(new ThreadStart(GetApps)).Start();
            instance = this;
        }
        ~MainWindow()
        {
            config.Serialize();
        }
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        private GUIItem AddNewTaskItem(WinHandle window)
        {
            window.TitleChanged += Window_TitleChanged;
            var g = new GUIItem();
            Application.Current.Dispatcher.Invoke(() => {
                try
                {
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
                }
                catch { }
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
            SpaceReserver.MakeNewDesktopArea(0, PanelHeight+10, 0, 0);
            PanelPosition = PanelPos.Top;
            this.Left = 0;
            this.Top = 0;
            this.Width = PanelWidth;
            this.Height = PanelHeight;
        }

        public async void GetApps()
        {
            await Task.Factory.StartNew(() =>
           {

           try
           {
               Parallel.ForEach<JToken>(ap.appslist, p => appslist.AddRange(AppManager.GetItems().Where(u => u.name == p.Value<string>())));

               Parallel.ForEach<xApp>(appslist, l =>
               {
                   xApp item = l;
                   Dispatcher.Invoke(() =>
                   {
                       MenuItem s = new MenuItem();
                       s.Click += (object sender, RoutedEventArgs e) => { try { Process.Start(item.Path); } catch { MessageBox.Show("Unable to run item, make sure that the path is correct"); } };
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
                   });
               });
               }
               catch
               {
                   MessageBox.Show("Unable to run item! Make sure the path is correct");
               }
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
                    Console.WriteLine(e.NewValue);
                    defaultPlaybackDevice.Volume = e.NewValue*10;
                    if (e.NewValue > 8)
                    {
                        Dispatcher.Invoke(() => { SoundPanel.Children.Clear(); SoundPanel.Children.Add(new MahApps.Metro.IconPacks.PackIconMaterial() { Width = 15, Height = 15, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.VolumeHigh }); });
                    }
                    else if (e.NewValue < 8 && e.NewValue > 5)
                    {
                        Dispatcher.Invoke(() => { SoundPanel.Children.Clear(); SoundPanel.Children.Add(new MahApps.Metro.IconPacks.PackIconMaterial() { Width = 15, Height = 15, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.VolumeMedium }); });
                    }
                    else if( e.NewValue < 5 && e.NewValue > 0)
                    {
                        Dispatcher.Invoke(() => { SoundPanel.Children.Clear(); SoundPanel.Children.Add(new MahApps.Metro.IconPacks.PackIconMaterial() { Width = 15, Height = 15, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.VolumeLow }); });
                    }
                    else if(e.NewValue == 0) {
                        Dispatcher.Invoke(() => { SoundPanel.Children.Clear(); SoundPanel.Children.Add(new MahApps.Metro.IconPacks.PackIconMaterial() { Width = 15, Height = 15, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Center, Kind = MahApps.Metro.IconPacks.PackIconMaterialKind.VolumeOff }); });

                    }
                }

                catch(Exception ex){
                    Console.WriteLine(ex.ToString());
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
        public enum AppComandCode : uint
        {
            BASS_BOOST = 20,
            BASS_DOWN = 19,
            BASS_UP = 21,
            BROWSER_BACKWARD = 1,
            BROWSER_FAVORITES = 6,
            BROWSER_FORWARD = 2,
            BROWSER_HOME = 7,
            BROWSER_REFRESH = 3,
            BROWSER_SEARCH = 5,
            BROWSER_STOP = 4,
            LAUNCH_APP1 = 17,
            LAUNCH_APP2 = 18,
            LAUNCH_MAIL = 15,
            LAUNCH_MEDIA_SELECT = 16,
            MEDIA_NEXTTRACK = 11,
            MEDIA_PLAY_PAUSE = 14,
            MEDIA_PREVIOUSTRACK = 12,
            MEDIA_STOP = 13,
            TREBLE_DOWN = 22,
            TREBLE_UP = 23,
            VOLUME_DOWN = 9,
            VOLUME_MUTE = 8,
            VOLUME_UP = 10,
            MICROPHONE_VOLUME_MUTE = 24,
            MICROPHONE_VOLUME_DOWN = 25,
            MICROPHONE_VOLUME_UP = 26,
            CLOSE = 31,
            COPY = 36,
            CORRECTION_LIST = 45,
            CUT = 37,
            DICTATE_OR_COMMAND_CONTROL_TOGGLE = 43,
            FIND = 28,
            FORWARD_MAIL = 40,
            HELP = 27,
            MEDIA_CHANNEL_DOWN = 52,
            MEDIA_CHANNEL_UP = 51,
            MEDIA_FASTFORWARD = 49,
            MEDIA_PAUSE = 47,
            MEDIA_PLAY = 46,
            MEDIA_RECORD = 48,
            MEDIA_REWIND = 50,
            MIC_ON_OFF_TOGGLE = 44,
            NEW = 29,
            OPEN = 30,
            PASTE = 38,
            PRINT = 33,
            REDO = 35,
            REPLY_TO_MAIL = 39,
            SAVE = 32,
            SEND_MAIL = 41,
            SPELL_CHECK = 42,
            UNDO = 34,
            DELETE = 53,
            DWM_FLIP3D = 54
        }
        [DllImport("user32.dll")]
        static extern IntPtr SendMessageW(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);
        void AppCommand(AppComandCode commandCode)
        {
            int CommandID = (int)commandCode << 16;
            SendMessageW(Process.GetCurrentProcess().MainWindowHandle, 0x319, Process.GetCurrentProcess().MainWindowHandle, (IntPtr)CommandID);
        }
        private void Volume_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        //Currently doesn't work, I must find another way...
        private void BackWard(object sender, RoutedEventArgs e)
        {
            AppCommand(AppComandCode.MEDIA_PREVIOUSTRACK);
            Console.WriteLine("Back");
        }

        private void PlayPause(object sender, RoutedEventArgs e)
        {
            AppCommand(AppComandCode.MEDIA_PLAY_PAUSE);
        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            AppCommand(AppComandCode.MEDIA_NEXTTRACK);
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