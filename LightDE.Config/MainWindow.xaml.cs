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
using System.Windows.Navigation;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using Newtonsoft.Json;
using System.Threading;
using System.Diagnostics;

namespace LightDE.Config
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public Dictionary<string, string> variables = new Dictionary<string, string>();
        internal static ConfigClient config;
        public MainWindow()
        {
            InitializeComponent();
            new Thread(new ThreadStart(Init)).Start();

        }
        void Init()
        {
            config = new ConfigClient();

            try
            {
                variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(config.GetConfig());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            FillConfig();
        }
        public void FillConfig()
        {
            Parallel.ForEach<KeyValuePair<string, string>>(variables, var =>
            {
                if (var.Key != "FirstRun")
                {
                    Dispatcher.Invoke(() => configBox.Children.Add(new Variable(var.Key, var.Value)));
                }
            });
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            foreach (Variable var in configBox.Children)
            {
                var.Save();
            }
        }
    }
}
