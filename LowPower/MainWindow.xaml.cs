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
using System.Windows.Media.Animation;

namespace LowPower
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public bool IS_BEING_SHOWING = false;

        public MainWindow()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
            Opacity = 0;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Storyboard sb = FindResource("hideMe") as Storyboard;
            sb.Begin(this);
            IS_BEING_SHOWING = false;
        }
    }
}
