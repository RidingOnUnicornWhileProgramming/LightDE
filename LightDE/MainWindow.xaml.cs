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
using LightDE.Desktop;
using MaterialDesignThemes;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using LightDE.Core;
using System.IO;
using LightDE.UI;
using LightDE.UI.Windows;

namespace LightDE.UI
{
    /// <summary>
    /// Main Class for Panel
    /// </summary>
    /// //TODO END RADIAL MENU
    /// 
    public partial class MainWindow : Window
    {        
        private NotifyIconManager _notifyiconmanager; // keep alive callbacks
        AppDrawer _drawer ;

        private AppFetcher _Fetcher = new AppFetcher();
        public CoreAudioDevice defaultPlaybackDevice = new CoreAudioController().DefaultPlaybackDevice;
        public PanelPos PanelPosition = PanelPos.Top;
        public int PanelHeight = 30;
        public Core.Config _config = new Core.Config();
        public int PanelWidth = (int)System.Windows.SystemParameters.PrimaryScreenWidth;
        public System.Timers.Timer ClockTimer = new System.Timers.Timer(1000);
        public static MainWindow _current;
        bool shown = false;
        public MainWindow()
        {
            InitializeComponent();

            Init();
        }
        void Init()
        {
            Console.WriteLine("Started Init...");
            _current = this;
            _Fetcher.GenerateFiles(false);
            _notifyiconmanager = new NotifyIconManager(AddNewNotification);
            SetPanelPos(PanelPosition);
            //Dispatcher.Invoke(() => SetTopMost());
            //_drawer = new AppDrawer();
            DesktopD d = new DesktopD();
            Dock dd = new Dock();
            dd.Show();
            d.Show();
            ClockTimer.Elapsed += ClockTimer_Elapsed;
            ClockTimer.Start();
            d.InitializeDesktop();
            _drawer = new AppDrawer();
            
        }
        private void ClockTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => Clock.Header = DateTime.Now.ToString(Core.Config._current.GetVar("TimeDateFormat")));
        }

        public void SetTopMost()
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)InteropHelper.GetWindowLong(wndHelper.Handle, (int)InteropHelper.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)InteropHelper.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            InteropHelper.SetWindowLong(wndHelper.Handle, (int)InteropHelper.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
            _current = this;
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
        public string GetTitle(int hwnd)
        {

            StringBuilder title = new StringBuilder(256);
            InteropHelper.GetWindowText(hwnd, title, 256);
            return title.ToString();
        }
        public void SetPanelPos(PanelPos panelPos)// Sets working area and checks if panels arent overlaying
        {
            SpaceReserver.MakeNewDesktopArea(0, 32, 0, 0);
            Dispatcher.Invoke(
                () =>
                {
                    PanelPosition = PanelPos.Top;
                    this.Left = 0;
                    this.Top = 0;
                    this.Width = PanelWidth;
                    this.Height = PanelHeight;

                });
            InteropHelper.SetWindowPos(Process.GetCurrentProcess().MainWindowHandle, InteropHelper.HWND_TOPMOST, 0, 0, 0, 0, InteropHelper.TOPMOST_FLAGS);

        }

        public void GetApps()
        {
           
                
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

        private void Volume_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
        private void BackWard(object sender, RoutedEventArgs e)
        {
            KeyManager.AppCommand(KeyManager.AppComandCode.MEDIA_PREVIOUSTRACK);

        }

        private void PlayPause(object sender, RoutedEventArgs e)
        {
            KeyManager.AppCommand(KeyManager.AppComandCode.MEDIA_PLAY_PAUSE);

        }

        private void Forward(object sender, RoutedEventArgs e)
        {
            KeyManager.AppCommand(KeyManager.AppComandCode.MEDIA_NEXTTRACK);
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.Topmost = true;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)InteropHelper.GetWindowLong(wndHelper.Handle, (int)InteropHelper.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)InteropHelper.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            InteropHelper.SetWindowLong(wndHelper.Handle, (int)InteropHelper.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }
        private void Shutdown(object sender, RoutedEventArgs e)
        {
            var psi = new ProcessStartInfo("shutdown", "/s /t 0");
            psi.CreateNoWindow = true;
            psi.UseShellExecute = false;
            Process.Start(psi);
        }

        private void Logout(object sender, RoutedEventArgs e)
        {
            InteropHelper.ExitWindowsEx(0, 0);
        }

        private void Lock(object sender, RoutedEventArgs e)
        {
            InteropHelper.LockWorkStation();
        }

        private void menu_Click(object sender, RoutedEventArgs e)
        {
            shown = !shown;
            if (shown)
            {
                _drawer.Show();
            }
            else
            {
                _drawer.Hide();
            }
        }

        private void MenuItem_Click_5(object sender, RoutedEventArgs e)
        {
            new AppChooserWindow().Show();
        }

        private void OpenBar(object sender, RoutedEventArgs e)
        {

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