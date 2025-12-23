using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Atlas.Models;
using Atlas.Services;
using WpfMessageBox = System.Windows.MessageBox;
using WpfOpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace Atlas.Views
{
    public partial class MainWindow : Window
    {
        private readonly ConfigService _configService;
        private readonly StorageService _storageService;
        private AppConfig _config;
        private List<MediaItem> _mediaItems;

        public MainWindow()
        {
            InitializeComponent();
            _configService = new ConfigService();
            _storageService = new StorageService();
            _config = _configService.LoadConfig();
            _mediaItems = new List<MediaItem>();

            LoadMedia();
        }

        private void LoadMedia()
        {
            _mediaItems = _storageService.GetMediaItems(_config.StoragePath);
            RefreshMediaGrid();
        }

        private void RefreshMediaGrid()
        {
            MediaPanel.Children.Clear();

            if (_mediaItems.Count == 0)
            {
                EmptyState.Visibility = Visibility.Visible;
                MediaScroller.Visibility = Visibility.Collapsed;
                MediaCountText.Text = "0 items";
                return;
            }

            EmptyState.Visibility = Visibility.Collapsed;
            MediaScroller.Visibility = Visibility.Visible;
            MediaCountText.Text = $"{_mediaItems.Count} items";

            foreach (var item in _mediaItems)
            {
                var card = CreateMediaCard(item);
                MediaPanel.Children.Add(card);
            }
        }

        private Border CreateMediaCard(MediaItem item)
        {
            var border = new Border
            {
                Width = 200,
                Height = 200,
                Margin = new Thickness(10),
                Background = (SolidColorBrush)FindResource("SurfaceBrush"),
                BorderBrush = (SolidColorBrush)FindResource("BorderBrush"),
                BorderThickness = new Thickness(1),
                CornerRadius = new CornerRadius(8),
                Cursor = System.Windows.Input.Cursors.Hand
            };

            var grid = new Grid();

            // Thumbnail
            if (item.Type == MediaType.Image)
            {
                try
                {
                    var img = new System.Windows.Controls.Image
                    {
                        Source = new BitmapImage(new Uri(item.FilePath)),
                        Stretch = Stretch.UniformToFill
                    };
                    grid.Children.Add(img);
                }
                catch
                {
                    var placeholder = new TextBlock
                    {
                        Text = "🖼️",
                        FontSize = 48,
                        HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                        VerticalAlignment = System.Windows.VerticalAlignment.Center
                    };
                    grid.Children.Add(placeholder);
                }
            }
            else
            {
                var videoIcon = new TextBlock
                {
                    Text = "🎬",
                    FontSize = 48,
                    HorizontalAlignment = System.Windows.HorizontalAlignment.Center,
                    VerticalAlignment = System.Windows.VerticalAlignment.Center,
                    Foreground = (SolidColorBrush)FindResource("TextBrush")
                };
                grid.Children.Add(videoIcon);
            }

            // Overlay
            var overlay = new Border
            {
                Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(180, 30, 30, 30)),
                VerticalAlignment = VerticalAlignment.Bottom,
                Padding = new Thickness(10)
            };

            var fileName = new TextBlock
            {
                Text = item.FileName,
                Foreground = System.Windows.Media.Brushes.White,
                FontSize = 12,
                TextTrimming = TextTrimming.CharacterEllipsis
            };

            overlay.Child = fileName;
            grid.Children.Add(overlay);

            border.Child = grid;

            border.MouseLeftButtonUp += (s, e) => OpenMedia(item);

            return border;
        }

        private void OpenMedia(MediaItem item)
        {
            try
            {
                if (item.Type == MediaType.Image)
                {
                    var viewer = new ImageViewerWindow(item.FilePath);
                    viewer.ShowDialog();
                }
                else if (item.Type == MediaType.Video)
                {
                    var player = new VideoPlayerWindow(item.FilePath);
                    player.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                WpfMessageBox.Show($"Failed to open media: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ImportButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new WpfOpenFileDialog
            {
                Multiselect = true,
                Filter = "Media Files|*.jpg;*.jpeg;*.png;*.gif;*.bmp;*.mp4;*.avi;*.mkv;*.mov;*.wmv|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                ImportFiles(dialog.FileNames);
            }
        }

        private void Window_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(System.Windows.DataFormats.FileDrop);
                ImportFiles(files);
            }
        }

        private void Window_DragEnter(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(System.Windows.DataFormats.FileDrop))
            {
                e.Effects = System.Windows.DragDropEffects.Copy;
            }
            else
            {
                e.Effects = System.Windows.DragDropEffects.None;
            }
        }

        private void ImportFiles(string[] files)
        {
            int imported = 0;
            StatusText.Text = "Importing...";

            foreach (var file in files)
            {
                try
                {
                    _storageService.ImportFile(file, _config.StoragePath);
                    imported++;
                }
                catch (Exception ex)
                {
                    WpfMessageBox.Show($"Failed to import {Path.GetFileName(file)}: {ex.Message}",
                        "Import Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            LoadMedia();
            StatusText.Text = $"Imported {imported} file(s)";
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}