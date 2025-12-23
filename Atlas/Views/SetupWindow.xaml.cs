using System.Windows;
using Atlas.Services;
using System.Windows.Forms;
using WpfMessageBox = System.Windows.MessageBox;

namespace Atlas.Views
{
    public partial class SetupWindow : Window
    {
        private readonly StorageService _storageService;
        private string _selectedPath = string.Empty;

        public SetupWindow()
        {
            InitializeComponent();
            _storageService = new StorageService();

            // Set default path on load
            _selectedPath = _storageService.GetDefaultStoragePath();
            PathTextBlock.Text = _selectedPath;
            NextButton.IsEnabled = true;
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Select where to store your hidden media files";
                dialog.UseDescriptionForTitle = true;

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    _selectedPath = System.IO.Path.Combine(dialog.SelectedPath, "AtlasStorage");
                    PathTextBlock.Text = _selectedPath;
                    NextButton.IsEnabled = true;
                }
            }
        }

        private void UseDefaultButton_Click(object sender, RoutedEventArgs e)
        {
            _selectedPath = _storageService.GetDefaultStoragePath();
            PathTextBlock.Text = _selectedPath;
            NextButton.IsEnabled = true;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _storageService.CreateHiddenFolder(_selectedPath);

                var pinWindow = new PinSetupWindow(_selectedPath);
                pinWindow.Show();
                this.Close();
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show(
                    $"Failed to create storage folder: {ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }
    }
}