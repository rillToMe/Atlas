using System.Windows;
using System.Windows.Controls;
using Atlas.Models;
using Atlas.Services;
using WpfMessageBox = System.Windows.MessageBox;

namespace Atlas.Views
{
    public partial class PinSetupWindow : Window
    {
        private readonly SecurityService _securityService;
        private readonly ConfigService _configService;
        private readonly string _storagePath;

        public PinSetupWindow(string storagePath)
        {
            InitializeComponent();
            _securityService = new SecurityService();
            _configService = new ConfigService();
            _storagePath = storagePath;
        }

        private void PinBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            ValidateInputs();
        }

        private void ValidateInputs()
        {
            ErrorText.Visibility = Visibility.Collapsed;
            CreateButton.IsEnabled = false;

            var pin = PinBox.Password;
            var confirmPin = ConfirmPinBox.Password;

            if (string.IsNullOrEmpty(pin) || string.IsNullOrEmpty(confirmPin))
                return;

            if (!_securityService.IsValidPin(pin))
            {
                ErrorText.Text = "PIN must be 4-8 numeric digits only";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            if (pin != confirmPin)
            {
                ErrorText.Text = "PINs don't match!";
                ErrorText.Visibility = Visibility.Visible;
                return;
            }

            CreateButton.IsEnabled = true;
        }

        private void CreateButton_Click(object sender, RoutedEventArgs e)
        {
            var pin = PinBox.Password;
            var pinHash = _securityService.HashPin(pin);

            var config = new AppConfig
            {
                IsFirstRun = false,
                PinHash = pinHash,
                StoragePath = _storagePath
            };

            _configService.SaveConfig(config);

            WpfMessageBox.Show(
                "Setup complete! Welcome to Atlas 🎉",
                "Success",
                MessageBoxButton.OK,
                MessageBoxImage.Information
            );

            var mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void CreateButton_PreviewMouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (!CreateButton.IsEnabled)
            {
                if (string.IsNullOrEmpty(PinBox.Password) && string.IsNullOrEmpty(ConfirmPinBox.Password))
                {
                    WpfMessageBox.Show(
                        "Please enter your PIN first!",
                        "PIN Required",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
                else if (!string.IsNullOrEmpty(ErrorText.Text))
                {
                    WpfMessageBox.Show(
                        ErrorText.Text,
                        "Invalid PIN",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning
                    );
                }
                e.Handled = true;
            }
        }
    }
}