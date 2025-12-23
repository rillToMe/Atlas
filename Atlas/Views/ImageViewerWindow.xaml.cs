using Atlas.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // Ditambahkan agar .Where() dan .OrderBy() bekerja
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input; // Namespace utama untuk Mouse/Key EventArgs di WPF
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace Atlas.Views
{
    public partial class ImageViewerWindow : Window
    {
        private readonly StorageService _storageService;
        private readonly string _storagePath;
        private List<string> _imageFiles = new List<string>(); // Inisialisasi awal untuk hindari CS8618
        private int _currentIndex;
        private BitmapImage? _currentBitmap; // Dijadikan nullable dengan tanda '?'
        private double _zoomLevel = 1.0;
        private bool _isFullScreen = false;
        private WindowState _previousWindowState;
        private WindowStyle _previousWindowStyle;

        public ImageViewerWindow(string imagePath)
        {
            InitializeComponent();

            _storageService = new StorageService();

            // Get storage path from config
            var configService = new ConfigService();
            var config = configService.LoadConfig();
            _storagePath = config.StoragePath;

            // Load all images from storage
            LoadImageList(imagePath);

            // Show current image
            ShowCurrentImage();

            // Setup mouse hover for navigation buttons
            this.MouseMove += Window_MouseMove;
        }

        private void LoadImageList(string currentImagePath)
        {
            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };

            if (Directory.Exists(_storagePath))
            {
                _imageFiles = Directory.GetFiles(_storagePath)
                    .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
                    .OrderBy(f => f)
                    .ToList();
            }

            _currentIndex = _imageFiles.IndexOf(currentImagePath);
            if (_currentIndex == -1) _currentIndex = 0;

            UpdateNavigationVisibility();
        }

        private void UpdateNavigationVisibility()
        {
            if (_imageFiles.Count <= 1)
            {
                PrevButtonContainer.Visibility = Visibility.Collapsed;
                NextButtonContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void ShowCurrentImage()
        {
            if (_imageFiles.Count == 0) return;

            var imagePath = _imageFiles[_currentIndex];
            TitleText.Text = $"{Path.GetFileName(imagePath)} ({_currentIndex + 1}/{_imageFiles.Count})";

            _currentBitmap = new BitmapImage();
            _currentBitmap.BeginInit();
            _currentBitmap.CacheOption = BitmapCacheOption.OnLoad;
            _currentBitmap.UriSource = new Uri(imagePath);
            _currentBitmap.EndInit();

            ImageDisplay.Source = _currentBitmap;

            // Reset zoom
            _zoomLevel = 1.0;
            UpdateZoom();

            // Auto-fit logic
            if (_currentBitmap.PixelWidth > this.ActualWidth ||
                _currentBitmap.PixelHeight > this.ActualHeight - 150)
            {
                FitToWindow();
            }

            UpdateImageInfo();
        }

        private void UpdateImageInfo()
        {
            if (_currentBitmap != null && _imageFiles.Count > 0)
            {
                var fileInfo = new FileInfo(_imageFiles[_currentIndex]);
                var sizeKB = fileInfo.Length / 1024.0;
                var sizeText = sizeKB > 1024 ? $"{sizeKB / 1024:F1} MB" : $"{sizeKB:F0} KB";

                ImageInfo.Text = $"{_currentBitmap.PixelWidth} × {_currentBitmap.PixelHeight} | {sizeText}";
            }
        }

        private void UpdateZoom()
        {
            ImageScale.ScaleX = _zoomLevel;
            ImageScale.ScaleY = _zoomLevel;
            ZoomText.Text = $"{_zoomLevel * 100:F0}%";
        }

        private void FitToWindow()
        {
            if (_currentBitmap == null) return;

            var availableWidth = ImageScroller.ActualWidth - 40;
            var availableHeight = ImageScroller.ActualHeight - 40;

            var scaleX = availableWidth / _currentBitmap.PixelWidth;
            var scaleY = availableHeight / _currentBitmap.PixelHeight;

            _zoomLevel = Math.Min(scaleX, scaleY);
            _zoomLevel = Math.Min(_zoomLevel, 1.0);
            UpdateZoom();
        }

        // --- Event Handlers dengan tipe parameter yang spesifik ---

        private void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex > 0)
            {
                _currentIndex--;
                ShowCurrentImage();
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentIndex < _imageFiles.Count - 1)
            {
                _currentIndex++;
                ShowCurrentImage();
            }
        }

        private void ZoomIn_Click(object sender, RoutedEventArgs e)
        {
            _zoomLevel = Math.Min(_zoomLevel * 1.2, 5.0);
            UpdateZoom();
        }

        private void ZoomOut_Click(object sender, RoutedEventArgs e)
        {
            _zoomLevel = Math.Max(_zoomLevel / 1.2, 0.1);
            UpdateZoom();
        }

        private void FitToWindow_Click(object sender, RoutedEventArgs e) => FitToWindow();

        private void ActualSize_Click(object sender, RoutedEventArgs e)
        {
            _zoomLevel = 1.0;
            UpdateZoom();
        }

        private void FullScreen_Click(object sender, RoutedEventArgs e) => ToggleFullScreen();

        private void ToggleFullScreen()
        {
            if (_isFullScreen)
            {
                this.WindowState = _previousWindowState;
                this.WindowStyle = _previousWindowStyle;
                _isFullScreen = false;
            }
            else
            {
                _previousWindowState = this.WindowState;
                _previousWindowStyle = this.WindowStyle;
                this.WindowStyle = WindowStyle.None;
                this.WindowState = WindowState.Maximized;
                _isFullScreen = true;
            }
        }

        // Perbaikan: Spesifik menggunakan System.Windows.Input.KeyEventArgs
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Left:
                    PrevButton_Click(sender, e);
                    break;
                case Key.Right:
                    NextButton_Click(sender, e);
                    break;
                case Key.Add:
                case Key.OemPlus:
                    ZoomIn_Click(sender, e);
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    ZoomOut_Click(sender, e);
                    break;
                case Key.D0:
                case Key.NumPad0:
                    ActualSize_Click(sender, e);
                    break;
                case Key.F11:
                    ToggleFullScreen();
                    break;
                case Key.Escape:
                    if (_isFullScreen) ToggleFullScreen();
                    else this.Close();
                    break;
            }
        }

        // Perbaikan: Spesifik menggunakan System.Windows.Input.MouseEventArgs
        private void Window_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (_imageFiles == null || _imageFiles.Count <= 1) return;

            var mousePos = e.GetPosition(this);
            var windowWidth = this.ActualWidth;

            AnimateOpacity(PrevButtonContainer, (mousePos.X < windowWidth * 0.2) ? 1.0 : 0.0);
            AnimateOpacity(NextButtonContainer, (mousePos.X > windowWidth * 0.8) ? 1.0 : 0.0);
        }

        private void NavButton_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (sender is Border border)
            {
                AnimateOpacity(border, 1.0);
            }
        }

        private void NavButton_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Handled by Window_MouseMove
        }

        private void AnimateOpacity(UIElement element, double toOpacity)
        {
            if (element.Opacity == toOpacity) return;

            var animation = new DoubleAnimation
            {
                To = toOpacity,
                Duration = TimeSpan.FromMilliseconds(200),
                EasingFunction = new QuadraticEase()
            };
            element.BeginAnimation(UIElement.OpacityProperty, animation);
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e) => this.Close();
    }
}