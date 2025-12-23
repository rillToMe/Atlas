using System.IO;
using System.Text.Json;
using Atlas.Models;

namespace Atlas.Services
{
    public class ConfigService
    {
        private readonly string _configPath;
        private readonly string _appDataFolder;

        public ConfigService()
        {
            _appDataFolder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "Atlas"
            );

            if (!Directory.Exists(_appDataFolder))
            {
                Directory.CreateDirectory(_appDataFolder);
            }

            _configPath = Path.Combine(_appDataFolder, "config.json");
        }

        public AppConfig LoadConfig()
        {
            if (!File.Exists(_configPath))
            {
                return new AppConfig();
            }

            try
            {
                var json = File.ReadAllText(_configPath);
                return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
            }
            catch
            {
                return new AppConfig();
            }
        }

        public void SaveConfig(AppConfig config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            File.WriteAllText(_configPath, json);
        }

        public void ResetConfig()
        {
            if (File.Exists(_configPath))
            {
                File.Delete(_configPath);
            }
        }
    }
}