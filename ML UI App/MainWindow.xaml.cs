using ML_UI_App.ConnectionService;
using System.Windows;

namespace ML_UI_App
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
            ConService.Connect();
            MessageBox.Show("законекчено");
        }

        private void Button_Stop(object sender, RoutedEventArgs e)
        {
            ConService.Disconnect();
        }

        private void testcon_Click(object sender, RoutedEventArgs e)
        {
            testTextBlock.Text = ConService.TestSendMessage();
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ConService.Disconnect();
        }
    }
}
