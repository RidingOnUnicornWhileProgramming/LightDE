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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Reflection;
using MahApps.Metro.Controls;
//using Sloppycode.net;
using System.Net;
using System.IO;
using TNX.RssReader;
using System.Diagnostics;
using LightDE.Config;
using Newtonsoft.Json;
using System.Threading;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;

namespace LightDE.Desktop
{
    /// <summary>
    /// Interaction logic for Desktop.xaml
    /// </summary>
    /// 
    public partial class DesktopD : Window , ISaveable
    {
        WindowSinker ws;
        bool desktop;
        public string name
        {
            get
            {
                return "DesktopD";
            }
        }

        public DesktopD()
        {
            InitializeComponent();
            ws = new WindowSinker(this);
            ShowWelcomeScreen();
            
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            AssignSize();
        }
        public void ShowWelcomeScreen()
        {
            new Thread(new ThreadStart(FetchRssFeed)).Start();
            new Thread(new ThreadStart(GetRecentFiles)).Start();
            new Thread(new ThreadStart(FillDesktopView)).Start();

            Background.Visibility = Visibility.Hidden;
            Welcome.Visibility = Visibility.Visible;
            Welcome.IsEnabled = true;
            welcomeText.Content = "Welcome, " + Environment.UserName;// System.Security.Principal.WindowsIdentity.GetCurrent().Name.Split(new char[] { '\\' })[1];
        }
        public void AssignSize()
        {
            WindowState = WindowState.Maximized;
            Width = System.Windows.SystemParameters.PrimaryScreenWidth;
            Height = System.Windows.SystemParameters.PrimaryScreenHeight - 30;
            Top = 30;
            Left = 0;
            ws.Sink();
        }
        public void FetchRssFeed()
        {
            RssFeed rs;
            foreach (string l in MainWindow.config.GetVar("DesktopD", "rss") as Newtonsoft.Json.Linq.JArray)
            {

                rs = RssHelper.ReadFeed(@l);
                foreach (RssItem r in rs.Items)
                {
                    Dispatcher.Invoke(() =>
                    {
                        Tile t = new Tile();
                        t.Title = r.Title;
                        t.Content = r.Author;
                        t.Click += (object sender, RoutedEventArgs e) => { Process.Start(r.Link); };
                        t.Height = news.Height - 15;
                        news.Width += t.Width;
                        news.Children.Add(t);
                    });
                }
            }
        }
        public void GetRecentFiles()
        {
            foreach (string f in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Recent)))
            {
                Dispatcher.Invoke(() =>
                {
                    Tile t = new Tile();
                    var Icon = System.Drawing.Icon.ExtractAssociatedIcon(f);
                    Rectangle r = new Rectangle();
                    r.Width = 50;
                    r.Height = 50;
                    r.Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
                    t.Content = r;
                    t.Click += (object sender, RoutedEventArgs e) => { try { Process.Start(f); } catch { } };
                    t.Title = System.IO.Path.GetFileName(f).Split(new char[] {'.'})[0];
                    lastused.Width += t.Width;
                    lastused.Children.Add(t);
                });
            }
        }

        public void InitializeDesktop()
        {
            Background.Visibility = Visibility.Visible;

             Welcome.Visibility = Visibility.Hidden;
             Welcome.IsEnabled = false;
            SetWallpaper();
            AssignSize();

        }
        public void FillDesktopView()
        {
            ObservableCollection<Tile> tiles = new ObservableCollection<Tile>();
            foreach (string f in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)))
            {
                Dispatcher.Invoke(() =>
                {
                    Tile t = new Tile();
                    var Icon = System.Drawing.Icon.ExtractAssociatedIcon(f);
                    Rectangle r = new Rectangle();
                    t.Width = 80;
                    r.Width = 30;
                    t.Height = 80;
                    r.Height = 30;
                    r.Fill = new ImageBrush(Imaging.CreateBitmapSourceFromHBitmap(Icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
                    t.Content = r;
                    t.Click += (object sender, RoutedEventArgs e) => { try { Process.Start(f); } catch { } };
                    t.Title = System.IO.Path.GetFileName(f).Split(new char[] { '.' })[0];
                    DesktopItems.Items.Add(t);
                });
            }
            

        }
        public void SetWallpaper()
        {
            BitmapImage bm = new BitmapImage(new Uri((string)MainWindow.config.GetVar("DesktopD", "WallpaperPath")));
            Background.Background = new ImageBrush(bm);
        }

        private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            ScrollViewer scrollviewer = sender as ScrollViewer;
            if (e.Delta > 0)
                scrollviewer.LineLeft();
            else
                scrollviewer.LineRight();
            e.Handled = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void ToggleSwitchButton_Click(object sender, RoutedEventArgs e)
        {
            desktop = !desktop;
            if (!desktop)
            {
                Scopes.Visibility = Visibility.Visible;
                DesktopView.Visibility = Visibility.Hidden;
            }
            else {
                Scopes.Visibility = Visibility.Hidden;
                DesktopView.Visibility = Visibility.Visible;
            }
        }

 private void DesktopItems_Drop(object sender, DragEventArgs e)
        {
           // var source = e.Data.GetData(typeof(Tile)) as Tile;
           // var target = ((Tile)(sender)) as Tile;

           // int sourceIndex = DesktopItems.Items.IndexOf(source);
           // int targetIndex = DesktopItems.Items.IndexOf(target);
           //
//Move(source, sourceIndex, targetIndex);
        }
        private void Move(Tile source, int sourceIndex, int targetIndex)
        {
            //if (sourceIndex < targetIndex)
           // {
           //     DesktopItems.Items.Insert(targetIndex + 1, source);
           //     DesktopItems.Items.RemoveAt(sourceIndex);
           // }
          //  else
           // {
           //     int removeIndex = sourceIndex + 1;
            //    if (DesktopItems.Items.Count + 1 > removeIndex)
            //    {
             //       DesktopItems.Items.Insert(targetIndex, source);
//DesktopItems.Items.RemoveAt(removeIndex);
              //  }
           // }
        }
    }
    class DesktopViewModel : IDropTarget
    {
        public ObservableCollection<DesktopItemModel> Items;

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            DesktopItemModel sourceItem = dropInfo.Data as DesktopItemModel;
            DesktopItemModel targetItem = dropInfo.TargetItem as DesktopItemModel;

            if (sourceItem != null && targetItem != null && targetItem.CanAcceptChildren)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Copy;
            }
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            Tile sourceItem = dropInfo.Data as Tile;
            DesktopItemModel targetItem = dropInfo.TargetItem as DesktopItemModel;
            targetItem.Children.Add(sourceItem);
        }
    }

    class DesktopItemModel
    {
        public bool CanAcceptChildren { get; set; }
        public ObservableCollection<Tile> Children { get; private set; }
    }
    public class WindowSinker
    {
        #region Properties

        const UInt32 SWP_NOSIZE = 0x0001;
        const UInt32 SWP_NOMOVE = 0x0002;
        const UInt32 SWP_NOACTIVATE = 0x0010;
        const UInt32 SWP_NOZORDER = 0x0004;
        const int WM_ACTIVATEAPP = 0x001C;
        const int WM_ACTIVATE = 0x0006;
        const int WM_SETFOCUS = 0x0007;
        const int WM_WINDOWPOSCHANGING = 0x0046;

        static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        Window Window = null;

        #endregion

        #region WindowSinker

        public WindowSinker(Window Window)
        {
            this.Window = Window;
        }

        #endregion

        #region Methods

        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr DeferWindowPos(IntPtr hWinPosInfo, IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        static extern IntPtr BeginDeferWindowPos(int nNumWindows);

        [DllImport("user32.dll")]
        static extern bool EndDeferWindowPos(IntPtr hWinPosInfo);

        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var Handle = (new WindowInteropHelper(Window)).Handle;

            var Source = HwndSource.FromHwnd(Handle);
            Source.RemoveHook(new HwndSourceHook(WndProc));
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            var Hwnd = new WindowInteropHelper(Window).Handle;
            SetWindowPos(Hwnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);

            var Handle = (new WindowInteropHelper(Window)).Handle;

            var Source = HwndSource.FromHwnd(Handle);
            Source.AddHook(new HwndSourceHook(WndProc));
        }

        IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_SETFOCUS)
            {
                hWnd = new WindowInteropHelper(Window).Handle;
                SetWindowPos(hWnd, HWND_BOTTOM, 0, 0, 0, 0, SWP_NOSIZE | SWP_NOMOVE | SWP_NOACTIVATE);
                handled = true;
            }
            return IntPtr.Zero;
        }

        public void Sink()
        {
            Window.Loaded += OnLoaded;
            Window.Closing += OnClosing;
        }

        public void Unsink()
        {
            Window.Loaded -= OnLoaded;
            Window.Closing -= OnClosing;
        }

        #endregion
    }

    public static class WindowExtensions
    {
        #region Always On Bottom

        public static readonly DependencyProperty SinkerProperty = DependencyProperty.RegisterAttached("Sinker", typeof(WindowSinker), typeof(WindowExtensions), new UIPropertyMetadata(null));
        public static WindowSinker GetSinker(DependencyObject obj)
        {
            return (WindowSinker)obj.GetValue(SinkerProperty);
        }
        public static void SetSinker(DependencyObject obj, WindowSinker value)
        {
            obj.SetValue(SinkerProperty, value);
        }

        public static readonly DependencyProperty AlwaysOnBottomProperty = DependencyProperty.RegisterAttached("AlwaysOnBottom", typeof(bool), typeof(WindowExtensions), new UIPropertyMetadata(false, OnAlwaysOnBottomChanged));
        public static bool GetAlwaysOnBottom(DependencyObject obj)
        {
            return (bool)obj.GetValue(AlwaysOnBottomProperty);
        }
        public static void SetAlwaysOnBottom(DependencyObject obj, bool value)
        {
            obj.SetValue(AlwaysOnBottomProperty, value);
        }
        static void OnAlwaysOnBottomChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var Window = sender as Window;
            if (Window != null)
            {
                if ((bool)e.NewValue)
                {
                    var Sinker = new WindowSinker(Window);
                    Sinker.Sink();
                    SetSinker(Window, Sinker);
                }
                else
                {
                    var Sinker = GetSinker(Window);
                    Sinker.Unsink();
                    SetSinker(Window, null);
                }
            }
        }

        #endregion
    }
}
