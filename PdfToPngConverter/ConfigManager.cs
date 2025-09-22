using System.Text.Json;

namespace PdfToPngConverter
{
    public class AppConfig
    {
        public string SourceFolder { get; set; } = @"C:\Source\PDFs";
        public string DestinationFolder { get; set; } = @"C:\Output\PNGs";
        public int PngQuality { get; set; } = 300;
        public int RandomSeed { get; set; } = 12345;
        public bool OverwriteExisting { get; set; } = true;
        public bool RecursiveSearch { get; set; } = false;
    }

    public class ConfigManager
    {
        private const string ConfigFileName = "config.json";
        private readonly string _configPath;

        public ConfigManager()
        {
            _configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
        }

        public AppConfig LoadConfig()
        {
            try
            {
                if (!File.Exists(_configPath))
                {
                    Console.WriteLine($"Config file not found at {_configPath}. Creating default configuration...");
                    var defaultConfig = new AppConfig();
                    SaveConfig(defaultConfig);
                    return defaultConfig;
                }

                var jsonString = File.ReadAllText(_configPath);
                var config = JsonSerializer.Deserialize<AppConfig>(jsonString);
                
                if (config == null)
                {
                    Console.WriteLine("Failed to parse config file. Using default configuration...");
                    return new AppConfig();
                }

                Console.WriteLine($"Configuration loaded from {_configPath}");
                return config;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading configuration: {ex.Message}");
                Console.WriteLine("Using default configuration...");
                return new AppConfig();
            }
        }

        public void SaveConfig(AppConfig config)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true
                };
                
                var jsonString = JsonSerializer.Serialize(config, options);
                File.WriteAllText(_configPath, jsonString);
                Console.WriteLine($"Configuration saved to {_configPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving configuration: {ex.Message}");
            }
        }

        public bool ValidateConfig(AppConfig config)
        {
            var isValid = true;

            if (string.IsNullOrWhiteSpace(config.SourceFolder))
            {
                Console.WriteLine("Error: Source folder cannot be empty.");
                isValid = false;
            }
            else if (!Directory.Exists(config.SourceFolder))
            {
                Console.WriteLine($"Warning: Source folder does not exist: {config.SourceFolder}");
                Console.WriteLine("Please ensure the folder exists before running the application.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(config.DestinationFolder))
            {
                Console.WriteLine("Error: Destination folder cannot be empty.");
                isValid = false;
            }

            if (config.PngQuality < 72 || config.PngQuality > 600)
            {
                Console.WriteLine("Warning: PNG quality should be between 72 and 600 DPI. Using default value of 300.");
                config.PngQuality = 300;
            }

            return isValid;
        }

        public void DisplayConfig(AppConfig config)
        {
            Console.WriteLine("\n=== Current Configuration ===");
            Console.WriteLine($"Source Folder: {config.SourceFolder}");
            Console.WriteLine($"Destination Folder: {config.DestinationFolder}");
            Console.WriteLine($"PNG Quality: {config.PngQuality} DPI");
            Console.WriteLine($"Random Seed: {config.RandomSeed}");
            Console.WriteLine($"Overwrite Existing: {config.OverwriteExisting}");
            Console.WriteLine($"Recursive Search: {config.RecursiveSearch}");
            Console.WriteLine("===============================\n");
        }
    }
}
