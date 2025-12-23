using System.IO;
using Atlas.Models;

namespace Atlas.Services
{
    public class StorageService
    {
        public void CreateHiddenFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                var dirInfo = Directory.CreateDirectory(path);
                dirInfo.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            else
            {
                var dirInfo = new DirectoryInfo(path);
                dirInfo.Attributes |= FileAttributes.Hidden;
            }
        }

        public string GetDefaultStoragePath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Atlas",
                "Storage"
            );
        }

        public bool IsValidStoragePath(string path)
        {
            try
            {
                var fullPath = Path.GetFullPath(path);
                return !string.IsNullOrWhiteSpace(fullPath);
            }
            catch
            {
                return false;
            }
        }

        public List<MediaItem> GetMediaItems(string storagePath)
        {
            var items = new List<MediaItem>();

            if (!Directory.Exists(storagePath))
                return items;

            var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
            var videoExtensions = new[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv" };

            var files = Directory.GetFiles(storagePath);

            foreach (var file in files)
            {
                var ext = Path.GetExtension(file).ToLower();
                var fileInfo = new FileInfo(file);

                MediaType? type = null;

                if (imageExtensions.Contains(ext))
                    type = MediaType.Image;
                else if (videoExtensions.Contains(ext))
                    type = MediaType.Video;

                if (type.HasValue)
                {
                    items.Add(new MediaItem
                    {
                        FileName = Path.GetFileName(file),
                        FilePath = file,
                        Type = type.Value,
                        DateAdded = fileInfo.CreationTime,
                        FileSize = fileInfo.Length
                    });
                }
            }

            return items.OrderByDescending(x => x.DateAdded).ToList();
        }

        public void ImportFile(string sourcePath, string destinationFolder)
        {
            var fileName = Path.GetFileName(sourcePath);
            var destPath = Path.Combine(destinationFolder, fileName);

            // Handle duplicate names
            int counter = 1;
            while (File.Exists(destPath))
            {
                var nameWithoutExt = Path.GetFileNameWithoutExtension(sourcePath);
                var ext = Path.GetExtension(sourcePath);
                fileName = $"{nameWithoutExt}_{counter}{ext}";
                destPath = Path.Combine(destinationFolder, fileName);
                counter++;
            }

            File.Copy(sourcePath, destPath);
        }
    }
}