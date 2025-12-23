using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Atlas.Views
{
    public partial class ImageViewerWindow : Window
    {
        public ImageViewerWindow(string imagePath)
        {
            InitializeComponent();

            TitleText.Text = Path.GetFileName(imagePath);

            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(imagePath);
            bitmap.EndInit();

            ImageDisplay.Source = bitmap;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}