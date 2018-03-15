using DE.WindowManagement;
using LightDE.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace LightDE
{
    /// <summary>
    /// Interaction logic for Dock.xaml
    /// </summary>
    public partial class Dock : Window
    {
        public Dock()
        {
            InitializeComponent();
            Width = System.Windows.SystemParameters.PrimaryScreenWidth / 4;
            Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2 - Width/2;
            
            Top = System.Windows.SystemParameters.PrimaryScreenHeight - 50;
            animationshow.From = System.Windows.SystemParameters.PrimaryScreenHeight - 10;
            animationshow.To = System.Windows.SystemParameters.PrimaryScreenHeight - 50;
            animationclose.From = System.Windows.SystemParameters.PrimaryScreenHeight - 50;
            animationclose.To = System.Windows.SystemParameters.PrimaryScreenHeight - 10;
            Topmost = true;
            MouseLeave += Dock_MouseLeave;
            MouseEnter += Dock_MouseEnter;
            try
            {
                WindowManager wm = new WindowManager(AddNewTaskItem);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            new Thread(new ThreadStart(() =>
            {
                Thread.Sleep(2000);
                Dispatcher.Invoke(() => Top = System.Windows.SystemParameters.PrimaryScreenHeight - 10);
            })).Start();
            
        }

        private void Dock_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse Enter");
            RefreshTasks();
        }
        void RefreshTasks()
        {
            for (int i = 0; i < ProcMenu.Items.Count; i++)
            {
                Dispatcher.Invoke(() =>
                {
                    var h = ProcMenu.Items[i] as ListBoxItem;
                    var p = h.Content as StackPanel;
                    var l = p.Children[1] as Label; l.Content = GetTitle(int.Parse(p.Tag.ToString()));
                    if (!InteropHelper.IsIconic((IntPtr)int.Parse(p.Tag.ToString())))
                    {
                        h.Background = new SolidColorBrush(new Color() { R = Colors.LightGray.R, G = Colors.LightGray.G, B = Colors.LightGray.B, A = 50 }   );
                    }
                    else
                    {
                        h.Background = new SolidColorBrush(Colors.Transparent);

                    }
                });
            }
        }
        private void Dock_MouseLeave(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse leave");
            new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(2000);
                    Dispatcher.Invoke(() => Top = System.Windows.SystemParameters.PrimaryScreenHeight - 10);
                })).Start();
            RefreshTasks();
        }

        public void Appear()
        {
            Width = System.Windows.SystemParameters.PrimaryScreenWidth / 4;
            Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2 - Width / 2;

            Top = System.Windows.SystemParameters.PrimaryScreenHeight - 40;
           
        }
        private GUIItem AddNewTaskItem(WinHandle window)
        {
                //window.TitleChanged += Window_TitleChanged;
                var g = new GUIItem();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        StackPanel s = new StackPanel();
                        ListBoxItem holder = new ListBoxItem();
                        holder.Focusable = false;
                        s.Tag = window.Ptr.ToString();

                        holder.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) =>
                        {
                            RefreshTasks();
                            window.MaximizeMinimize();
                        };
                        ContextMenu o = new ContextMenu();
                        var h = new MenuItem() { Header = "Close" };
                        h.Click += (object sender, RoutedEventArgs e) => { InteropHelper.CloseWindow((IntPtr)int.Parse(s.Tag.ToString())); };
                        o.Items.Add(h);
                        s.ContextMenu = o;
                        Label l = new Label();
                        l.Content = window.Title;
                        s.Width = 60;
                        l.Foreground = new SolidColorBrush(Colors.White);
                        l.FontSize = 10;
                        Image m = new Image();
                        var handle = window.WindowIcon.ToBitmap().GetHbitmap();
                        try
                        {
                            m.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            m.Width = 25;
                            m.Height = 25;
                            s.Children.Add(m);
                        }
                        finally { InteropHelper.DeleteObject(handle); }
                        s.Children.Add(l);
                        s.Height = 60;
                        g.Destroy = () => { Application.Current.Dispatcher.Invoke(() => { Left += s.Width; ProcMenu.Width -= s.Width; ProcMenu.Items.Remove(ProcMenu.Items.OfType<ListBoxItem>().Where(x => x.Tag == s.Tag).First()); }); };
                        holder.Content = s;
                        RefreshTasks();
                        ProcMenu.Items.Add(holder);
                        Console.WriteLine("Added");
                        ProcMenu.Width += s.Width;
                        Left -= s.Width;
                        Width = ProcMenu.Width;
                    }
                    catch
                    (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                    }
                });
                return g;
        }

        private void Window_TitleChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                ()=>
                {
                    for (int i = 0; i < ProcMenu.Items.Count; i++)
                    {
                        RefreshTasks();
                    }
                });
        }
        public string GetTitle(int hwnd)
        {

            StringBuilder title = new StringBuilder(256);
            InteropHelper.GetWindowText(hwnd, title, 256);
            return title.ToString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)InteropHelper.GetWindowLong(wndHelper.Handle, (int)InteropHelper.GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)InteropHelper.ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            InteropHelper.SetWindowLong(wndHelper.Handle, (int)InteropHelper.GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void ProcMenu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
