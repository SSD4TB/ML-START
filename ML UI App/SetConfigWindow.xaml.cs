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

namespace ML_UI_App
{
    /// <summary>
    /// Логика взаимодействия для SetConfigWindow.xaml
    /// </summary>
    public partial class SetConfigWindow : Window
    {
        public SetConfigWindow()
        {
            InitializeComponent();
        }

        private void Button_SetConfigure(object sender, RoutedEventArgs e)
        {
            
        }

        public int NumN
        {
            get { return int.Parse(textBoxNumN.Text); }
        }

        public int NumL
        {
            get { return int.Parse(textBoxNumL.Text); }
        }

        public int Delay
        {
            get { return int.Parse(textBoxDelay.Text); }
        }
    }
}
