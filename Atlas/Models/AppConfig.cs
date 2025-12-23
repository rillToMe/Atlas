namespace Atlas.Models
{
    public class AppConfig
    {
        public bool IsFirstRun { get; set; } = true;
        public string PinHash { get; set; } = string.Empty;
        public string StoragePath { get; set; } = string.Empty;
        public string Theme { get; set; } = "dark";
    }

    public class MediaItem
    {
        public string FileName { get; set; } = string.Empty;
        public string FilePath { get; set; } = string.Empty;
        public MediaType Type { get; set; }
        public DateTime DateAdded { get; set; }
        public long FileSize { get; set; }
        public string? ThumbnailPath { get; set; }
    }

    public enum MediaType
    {
        Image,
        Video
    }
}