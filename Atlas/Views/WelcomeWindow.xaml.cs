using System.Windows;

namespace Atlas.Views
{
    public partial class WelcomeWindow : Window
    {
        public WelcomeWindow()
        {
            InitializeComponent();
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            var setupWindow = new SetupWindow();
            setupWindow.Show();
            this.Close();
        }
    }
}