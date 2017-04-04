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

namespace LightDE.Config
{
    /// <summary>
    /// Interaction logic for Variable.xaml
    /// </summary>
    public partial class Variable : UserControl
    {
        public Variable(string varname, string var)
        {
            InitializeComponent();
            varName.Content = varname;
            Var.Text = var;
        }
        public void Save()
        {
            
        }
    }
}
