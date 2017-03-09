using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace LightDE.Widgets
{
    /// <summary>
    /// Interaction logic for Performance.xaml
    /// </summary>
    public partial class Performance : UserControl
    {
        PerformanceCounter cpuCounter;
        PerformanceCounter ramCounter;


        public Performance()
        {
            InitializeComponent();
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            System.Timers.Timer nt = new System.Timers.Timer(500);
            nt.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) => { Dispatcher.Invoke(() => { cpu.Content = "CPU: " + cpuCounter.NextValue() + "%"; ram.Content = "Free Ram: " + ramCounter.NextValue() + "MB"; }); };
            nt.Start();
        }
    }
}
