using System.IO;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Atlas.Views
{
    public partial class VideoPlayerWindow : Window
    {
        private bool _isPlaying = false;
        private bool _isDragging = false;
        private DispatcherTimer _timer;

        public VideoPlayerWindow(string videoPath)
        {
            InitializeComponent();

            TitleText.Text = Path.GetFileName(videoPath);
            VideoPlayer.Source = new Uri(videoPath);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(100);
            _timer.Tick += Timer_Tick;

            VideoPlayer.MediaOpened += VideoPlayer_MediaOpened;
        }

        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalSeconds;
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (!_isDragging && VideoPlayer.NaturalDuration.HasTimeSpan)
            {
                ProgressSlider.Value = VideoPlayer.Position.TotalSeconds;
                UpdateTimeDisplay();
            }
        }

        private void UpdateTimeDisplay()
        {
            var current = VideoPlayer.Position;
            var total = VideoPlayer.NaturalDuration.HasTimeSpan
                ? VideoPlayer.NaturalDuration.TimeSpan
                : TimeSpan.Zero;

            TimeDisplay.Text = $"{current:mm\\:ss} / {total:mm\\:ss}";
        }

        private void PlayPauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (_isPlaying)
            {
                VideoPlayer.Pause();
                PlayPauseButton.Content = "▶️ Play";
                _timer.Stop();
                _isPlaying = false;
            }
            else
            {
                VideoPlayer.Play();
                PlayPauseButton.Content = "⏸️ Pause";
                _timer.Start();
                _isPlaying = true;
            }
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            PlayPauseButton.Content = "▶️ Play";
            _timer.Stop();
            _isPlaying = false;
            ProgressSlider.Value = 0;
            UpdateTimeDisplay();
        }

        private void ProgressSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_isDragging)
            {
                VideoPlayer.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
            }
        }

        private void ProgressSlider_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _isDragging = true;
        }

        private void ProgressSlider_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            VideoPlayer.Position = TimeSpan.FromSeconds(ProgressSlider.Value);
            _isDragging = false;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Stop();
            _timer.Stop();
            this.Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            VideoPlayer.Stop();
            _timer.Stop();
            base.OnClosed(e);
        }
    }
}