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

namespace PR67_VP.Pages
{
    /// <summary>
    /// Логика взаимодействия для In.xaml
    /// </summary>
    public partial class In : Page
    {
        public string WelcomeMessage { get; set; }
        public In()
        {
            InitializeComponent();
        }
    }
}
