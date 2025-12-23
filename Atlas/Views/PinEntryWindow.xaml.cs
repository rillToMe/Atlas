using System.Windows;
using System.Windows.Input;
using Atlas.Services;
using WpfMessageBox = System.Windows.MessageBox;

namespace Atlas.Views
{
    public partial class PinEntryWindow : Window
    {
        private readonly SecurityService _securityService;
        private readonly ConfigService _configService;
        private int _attemptCount = 0;

        public PinEntryWindow()
        {
            InitializeComponent();
            _securityService = new SecurityService();
            _configService = new ConfigService();
            PinBox.Focus();
        }

        private void PinBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                UnlockButton_Click(sender, e);
            }
        }

        private void UnlockButton_Click(object sender, RoutedEventArgs e)
        {
            ErrorText.Visibility = Visibility.Collapsed;

            var pin = PinBox.Password;

            if (string.IsNullOrEmpty(pin))
            {
                ShowError("Please enter your PIN");
                return;
            }

            var config = _configService.LoadConfig();

            if (_securityService.VerifyPin(pin, config.PinHash))
            {
                var mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                _attemptCount++;
                PinBox.Clear();
                PinBox.Focus();

                if (_attemptCount >= 3)
                {
                    ShowError($"Incorrect PIN! ({_attemptCount} attempts)");
                }
                else
                {
                    ShowError("Incorrect PIN, try again");
                }
            }
        }

        private void ForgotPin_Click(object sender, RoutedEventArgs e)
        {
            var result = WpfMessageBox.Show(
                "Resetting your PIN will delete ALL app data including your storage location.\n\n" +
                "Your media files will remain on disk but you'll need to set everything up again.\n\n" +
                "Continue with reset?",
                "Reset Atlas",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning
            );

            if (result == MessageBoxResult.Yes)
            {
                _configService.ResetConfig();

                WpfMessageBox.Show(
                    "Atlas has been reset. Restarting setup...",
                    "Reset Complete",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );

                var welcomeWindow = new WelcomeWindow();
                welcomeWindow.Show();
                this.Close();
            }
        }

        private void ShowError(string message)
        {
            ErrorText.Text = message;
            ErrorText.Visibility = Visibility.Visible;
        }
    }
}