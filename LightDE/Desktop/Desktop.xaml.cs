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
using LightDE.Settings;
using Newtonsoft.Json;
using System.Threading;
using GongSolutions.Wpf.DragDrop;
using System.Collections.ObjectModel;
using LightDE.Widgets;
using LightDE.Core;
using MaterialDesignThemes.Wpf;
using System.Text.RegularExpressions;
using System.Collections;

namespace LightDE.Desktop
{
    /// <summary>
    /// Interaction logic for Desktop.xaml
    /// </summary>
    /// 
    public partial class DesktopD : Window
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
            WindowState = WindowState.Maximized;

            ShowWelcomeScreen();
            
            DesktopItems.Items.Clear();
            DesktopItems.DataContext = new DesktopViewModel();
        }

        private void Window_StateChanged(object sender, EventArgs e)
        {
            AssignSize();
        }
        public void ShowWelcomeScreen()
        {
            new Thread(new ThreadStart(FetchRssFeed)).Start();
            new Thread(new ThreadStart(GetRecentFiles)).Start();

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
            try
            {
                RssFeed rs;
                foreach (string l in Config.Current.V1.DesktopD_RSS)
                {
                    rs = RssHelper.ReadFeed(@l);
                    foreach (RssItem r in rs.Items)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            news.Children.Add(new Separator() { Width = 8 });

                            Card t = new Card();
                            StackPanel Items = new StackPanel();

                            TextBlock text = new TextBlock();
                            text.Text = r.Title;
                            text.FontSize = 16;
                            text.TextWrapping = TextWrapping.Wrap;
                            text.FontFamily = new FontFamily(@"pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Fonts.xaml");
                            text.Height = 75;
                            text.Padding = new Thickness(5, 0, 5, 2);
                            Button b = new Button();
                            b.Style = (Style)Application.Current.Resources["MaterialDesignFlatButton"];
                            b.Click += (object sender, RoutedEventArgs e) => { Process.Start(r.Link); };
                            b.Content = "Read More...";
                            b.HorizontalAlignment = HorizontalAlignment.Right;
                            t.Height = news.Height - 15;
                            news.Width += t.Width;
                            Items.Width = 200;
                            Items.Height = 100;
                            Items.Children.Add(text);

                            Items.Children.Add(b);
                            t.Content = Items;

                            news.Children.Add(t);
                        });
                    }
                }
            }
            catch
            {
                // shall we repeat the code above or what?
            }
        }
        public void GetRecentFiles()
        {
            foreach (string f in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Recent)))
            {
                Dispatcher.Invoke(() =>
                {
                    lastused.Children.Add(new Separator() { Width = 8 } );
                    Card t = new Card() { Width = 150, Height = 150 } ;
                    StackPanel l = new StackPanel();
                    var Icon = System.Drawing.Icon.ExtractAssociatedIcon(f);
                    Image r = new Image();
                    r.Width = 50;
                    r.Height = 50;
                    r.Source = Imaging.CreateBitmapSourceFromHBitmap(Icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());


                    t.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => { try { Process.Start(f); } catch { } };
                    TextBlock text = new TextBlock();
                    text.Padding = new Thickness(2, 10, 2, 2);
                    text.FontSize = 24;
                    text.FontFamily = new FontFamily(@"pack://application:,,,/MaterialDesignThemes.MahApps;component/Themes/MaterialDesignTheme.MahApps.Fonts.xaml");
                    
                    text.Text = System.IO.Path.GetFileName(f).Split(new char[] {'.'})[0];
                    text.TextWrapping = TextWrapping.Wrap;
                    l.Children.Add(new Separator() { Height = 10, Width = 0 });
                    l.Children.Add(r);
                    l.Children.Add(text);
                    t.Content = l;
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
            ws.Sink();
            widgets.Children.Add(new Performance());
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
            if (Config.Current.V1.General_FirstRun)
            {
                Config.Current.V1.DesktopD_WallpaperPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Desktop\Wallpaper.jpg");
                Config.Current.V1.General_FirstRun = false;
            }
            BitmapImage bm = new BitmapImage(new Uri(Config.Current.V1.DesktopD_WallpaperPath));
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

            Scopes.Visibility = desktop ? Visibility.Hidden : Visibility.Visible;
            DesktopView.Visibility = desktop ? Visibility.Visible : Visibility.Hidden;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)InteropHelper.GetWindowLong(wndHelper.Handle, (int)InteropHelper.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)InteropHelper.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            InteropHelper.SetWindowLong(wndHelper.Handle, (int)InteropHelper.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }
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
    public class DesktopItem
    {
        public string Name { get; set; }
        public string path { get; set; }
        public ImageSource BitmapImage { get; set; }
        public void OpenApp(object sender, MouseButtonEventArgs e)
        {
            Process.Start(path);
        }
    }
    public class AppClickCommand : ICommand
    {
        private string _path;
        public AppClickCommand(string path)
        {
            _path = path;

        }

        public void Execute(object parameter)
        {
            if (parameter == null)
            {
                return;
            }
            var app = (string)parameter;
            Process.Start(app);
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;
    }
    public class CommandHandler : ICommand
    {
        private Action _action;
        private bool _canExecute;
        public CommandHandler(Action action, bool canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            Console.WriteLine("Can executes");

            return _canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            var app = (string)parameter;
            Process.Start(app);
            Console.WriteLine("Clicked");
            _action();
        }
    }
    public class DesktopViewModel : IDropTarget
    {
        public ObservableCollection<DesktopItem> DesktopCollection { get; set; }
        private ICommand _clickCommand;
        public ICommand ClickCommand
        {
            get
            {
                _clickCommand = new CommandHandler(() => MyAction(), _canExecute);
                Console.WriteLine("Click init");
                return _clickCommand;
            }
        }
        private bool _canExecute;
        public void MyAction()
        {
            Console.WriteLine("Clicked");
        }
 
        public DesktopViewModel()
        {
            DesktopCollection = new ObservableCollection<DesktopItem>();
            foreach (string f in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Desktop)))
            {
                var Icon = System.Drawing.Icon.ExtractAssociatedIcon(f);
                DesktopCollection.Add(new DesktopItem() { Name = System.IO.Path.GetFileName(f).Split(new char[] { '.' })[0], BitmapImage = Imaging.CreateBitmapSourceFromHBitmap(Icon.ToBitmap().GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()), path = f });
            }
        }
        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data is DesktopItem)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Highlight;
                dropInfo.Effects = DragDropEffects.Move;
            }
        }

        public void Drop(IDropInfo dropInfo)
        {
            DesktopItem msp = (DesktopItem)dropInfo.Data;
            ((IList)dropInfo.DragInfo.SourceCollection).Remove(msp);
        }
    }
}
