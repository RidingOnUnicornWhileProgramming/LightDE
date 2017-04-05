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
using MahApps.Metro.Controls;
using MadMilkman.Ini;
using System.IO;
using System.Threading;

namespace LightDE.UI.Windows
{
    /// <summary>
    /// Interaction logic for AppChooserWindow.xaml
    /// </summary>
    public partial class AppChooserWindow : MetroWindow
    {
        public AppChooserWindow()
        {
            InitializeComponent();
            new Thread(new ThreadStart(FillListBox)).Start();
        }
        public void FillListBox()
        {

            Parallel.ForEach<string>(System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All\\", "*.app", System.IO.SearchOption.AllDirectories), a =>
            {
                IniOptions iniop = new IniOptions();
                IniFile file = new IniFile();
                try
                {
                    using (Stream stream = File.OpenRead(a))
                        file.Load(stream);

                    Dispatcher.Invoke(() => Apps.Items.Add(new ListBoxItem() { Content = file.Sections[0].Keys["name"].Value, IsSelected = File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\" + file.Sections[0].Keys["name"].Value + ".app") }));

                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }

            });
        }
        private void Save_Click(object sender, RoutedEventArgs e)
        {

            Parallel.ForEach<string>(System.IO.Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\"), f => File.Delete(f));
            new Thread(new ThreadStart(() =>
            {
                foreach (ListBoxItem a in Apps.Items.Cast<ListBoxItem>())
                {

                    if (Dispatcher.Invoke(() => a.IsSelected))
                    {
                        File.Copy(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\All\\" + Dispatcher.Invoke(() => a.Content) + ".app", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\LightDE" + "\\Apps\\Menu\\" + Dispatcher.Invoke(() => a.Content) + ".app");
                    }
                }
            })).Start() ;

            this.Close();
        }

    }
}
