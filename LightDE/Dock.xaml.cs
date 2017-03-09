using DE.WindowManagement;
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
            Topmost = true;
            MouseLeave += Dock_MouseLeave;
            MouseEnter += Dock_MouseEnter;
            try
            {
                WindowManager wm = new WindowManager(AddNewTaskItem);
            }
            catch { }
        }

        private void Dock_MouseEnter(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse Enter");
            Top = System.Windows.SystemParameters.PrimaryScreenHeight - 50;
        }

        private void Dock_MouseLeave(object sender, MouseEventArgs e)
        {
            Console.WriteLine("Mouse leave");
            new Thread(new ThreadStart(() =>
                {
                    Thread.Sleep(2000);
                    Dispatcher.Invoke(() => Top = System.Windows.SystemParameters.PrimaryScreenHeight - 10);
                })).Start();
        }

        public void Appear()
        {
            Width = System.Windows.SystemParameters.PrimaryScreenWidth / 4;
            Left = System.Windows.SystemParameters.PrimaryScreenWidth / 2 - Width / 2;

            Top = System.Windows.SystemParameters.PrimaryScreenHeight - 40;
        }
        private GUIItem AddNewTaskItem(WinHandle window)
        {
                window.TitleChanged += Window_TitleChanged;
                var g = new GUIItem();
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        StackPanel s = new StackPanel();
                        s.MouseLeftButtonUp += (object sender, MouseButtonEventArgs e) => { window.MaximizeMinimize(); };
                        Label l = new Label();
                        l.Content = window.Title;
                        s.Width = 40;
                        l.Foreground = new SolidColorBrush(Colors.White);
                        l.FontSize = 10;
                        s.Tag = window.Ptr.ToString();
                        Image m = new Image();
                        var handle = window.WindowIcon.ToBitmap().GetHbitmap();
                        try
                        {
                            m.Source = Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                            m.Width = 25;
                            m.Height = 25;
                            s.Children.Add(m);
                        }
                        finally { MainWindow.DeleteObject(handle); }
                        s.Children.Add(l);

                        g.Destroy = () => { Application.Current.Dispatcher.Invoke(() => ProcMenu.Children.Remove(ProcMenu.Children.Cast<StackPanel>().Where(x => x.Tag == s.Tag).First())); };
                        ProcMenu.Children.Add(s);
                    }
                    catch { }
                });
                return g;
        }

        private void Window_TitleChanged(object sender, EventArgs e)
        {
            Dispatcher.Invoke(
                ()=>
                {
                    for (int i = 0; i < ProcMenu.Children.Count; i++)
                    {
                        var p = ProcMenu.Children[i] as StackPanel;
                        Dispatcher.Invoke(() => { var l = p.Children[1] as Label; l.Content = GetTitle(int.Parse(p.Tag.ToString())); });
                    }
                });
        }
        public string GetTitle(int hwnd)
        {

            StringBuilder title = new StringBuilder(256);
            MainWindow.GetWindowText(hwnd, title, 256);
            return title.ToString();
        }
    }
}
