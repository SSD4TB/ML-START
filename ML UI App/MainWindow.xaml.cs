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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void Button_DeleteDB(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("скоро эта кнопка совершит величайший прикол =)");
        }

        private void Button_Connect(object sender, RoutedEventArgs e)
        {
            Log logwindow = new Log();
            if (logwindow.ShowDialog() == false)
            {
                Close();
            }
        }

        private void Button_Start(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {

        }
    }
}
