using System.Text;

namespace PdfToPngConverter
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Success
    }

    public static class Logger
    {
        private static readonly string LogFileName = "pdf_converter.log";
        private static readonly string LogPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LogFileName);
        private static bool _enableFileLogging = true;

        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            var logEntry = $"[{timestamp}] [{level}] {message}";

            // Console output with colors
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = level switch
            {
                LogLevel.Success => ConsoleColor.Green,
                LogLevel.Warning => ConsoleColor.Yellow,
                LogLevel.Error => ConsoleColor.Red,
                LogLevel.Info => ConsoleColor.White,
                _ => ConsoleColor.White
            };

            Console.WriteLine(logEntry);
            Console.ForegroundColor = originalColor;

            // File logging
            if (_enableFileLogging)
            {
                try
                {
                    File.AppendAllText(LogPath, logEntry + Environment.NewLine, Encoding.UTF8);
                }
                catch
                {
                    // Silently fail if we can't write to log file
                    _enableFileLogging = false;
                }
            }
        }

        public static void LogInfo(string message) => Log(message, LogLevel.Info);
        public static void LogWarning(string message) => Log(message, LogLevel.Warning);
        public static void LogError(string message) => Log(message, LogLevel.Error);
        public static void LogSuccess(string message) => Log(message, LogLevel.Success);

        public static void LogException(Exception ex, string context = "")
        {
            var message = string.IsNullOrEmpty(context) 
                ? $"Exception: {ex.Message}" 
                : $"Exception in {context}: {ex.Message}";
            
            Log(message, LogLevel.Error);
            
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                Log($"Stack trace: {ex.StackTrace}", LogLevel.Error);
            }
        }

        public static void InitializeLogging()
        {
            try
            {
                // Create or clear log file
                File.WriteAllText(LogPath, $"PDF to PNG Converter Log - Started at {DateTime.Now:yyyy-MM-dd HH:mm:ss}{Environment.NewLine}");
                Log($"Logging initialized. Log file: {LogPath}");
            }
            catch
            {
                _enableFileLogging = false;
                Log("Warning: Could not initialize file logging. Console logging only.", LogLevel.Warning);
            }
        }

        public static void LogSeparator()
        {
            Log(new string('=', 50));
        }

        public static void LogProgress(int current, int total, string item = "item")
        {
            var percentage = (double)current / total * 100;
            Log($"Progress: {current}/{total} ({percentage:F1}%) - {item}");
        }
    }
}
