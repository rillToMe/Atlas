using System.Diagnostics;
using System.Windows;
using System.Windows.Navigation;

namespace Atlas.Views
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            CopyrightText.Text = $"© {DateTime.Now.Year} DitDev. All rights reserved.";
        }

        private void AboutButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
                e.Handled = true;
            }
            catch
            {

            }
        }
    }
}