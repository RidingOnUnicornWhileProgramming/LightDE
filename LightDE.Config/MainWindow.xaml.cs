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

namespace LightDE.Config
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        public Dictionary<string, string> variables = new Dictionary<string, string>();
        static ConfigClient config;
        public MainWindow()
        {
            config = new ConfigClient();
            InitializeComponent();
            variables = JsonConvert.DeserializeObject<Dictionary<string, string>>(config.GetConfig());
            new Thread(new ThreadStart(FillConfig)).Start();
        }
        public void FillConfig()
        {
            //Parallel.ForEach<Dic
            //TODO: Make this fill stack panel with config.
            //When you click "save it setvars for every key"
            //by ipc 
            //I think that goes on github
        }
    }
}
