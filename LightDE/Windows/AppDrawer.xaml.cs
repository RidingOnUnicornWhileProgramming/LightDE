using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro;
using System.IO;
using MadMilkman.Ini;
using System.Threading;

namespace LightDE.UI
{
    /// <summary>
    /// Interaction logic for AppDrawer.xaml
    /// </summary>
    public partial class AppDrawer : Window
    {
        //TODO ADD SEARCHING FOR APPS AND MAKING ICON OUT OF IT

        public AppDrawer()
        {
            InitializeComponent();
            new Thread(new ThreadStart(Fill)).Start();

            Height = System.Windows.SystemParameters.FullPrimaryScreenHeight - 500;
            Width = System.Windows.SystemParameters.FullPrimaryScreenHeight - 500;
            Top = 30;
            Left = 0;
            Console.WriteLine("Launching...");
        }
        public void Fill()
        {
            Dispatcher.Invoke(() => Items.Items.Clear()); 
            Parallel.ForEach<string>(System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\", "*.app", System.IO.SearchOption.AllDirectories), a =>
            {
                IniOptions iniop = new IniOptions();
                IniFile file = new IniFile();
                try
                {
                    using (Stream stream = File.OpenRead(a))
                        file.Load(stream);
                    Dispatcher.Invoke(() => Items.Items.Add(new appIcon(file.Sections[0].Keys["name"].Value, file.Sections[0].Keys["iconPath"].Value, file.Sections[0].Keys["path"].Value)));
                    
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            });
      

        }
        /* public void FillLastUsed()
         {
             Parallel.ForEach<string>(System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.Recent),"*.*", System.IO.SearchOption.AllDirectories), a =>
             {
                 try
                 {
                     Dispatcher.Invoke(() => Items.Items.Add(new appIcon(file.Sections[0].Keys["name"].Value, file.Sections[0].Keys["iconPath"].Value, file.Sections[0].Keys["path"].Value)));

                 }
                 catch (Exception ex) { Console.WriteLine(ex.ToString()); }

             });
         }
         */
        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
           // this.Hide();
        }

        private void Apps_MouseDown(object sender, MouseButtonEventArgs e)
        {
            new Thread(new ThreadStart(Fill)).Start();
          
        }

        private void Documents_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                var l = Items.Items[0] as appIcon;
                l.Run();
            }
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            
            Hide();
        }
    }
}
