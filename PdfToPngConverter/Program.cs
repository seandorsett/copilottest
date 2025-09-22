using PdfToPngConverter;

namespace PdfToPngConverter
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            try
            {
                Console.WriteLine("🔄 PDF to PNG Converter");
                Console.WriteLine("=======================");
                Console.WriteLine();

                // Initialize configuration
                var configManager = new ConfigManager();
                var config = configManager.LoadConfig();
                
                // Display and validate configuration
                configManager.DisplayConfig(config);
                
                if (!configManager.ValidateConfig(config))
                {
                    Console.WriteLine("❌ Configuration validation failed. Please check your settings.");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return 1;
                }

                // Initialize processors
                var pdfProcessor = new PdfProcessor(config);
                var imageConverter = new ImageConverter(config);

                // Discover and process PDF files
                Console.WriteLine("🔍 Discovering PDF files...");
                var pdfFiles = pdfProcessor.DiscoverPdfFiles();
                
                if (!pdfFiles.Any())
                {
                    Console.WriteLine("❌ No PDF files found in the source directory.");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return 1;
                }

                // Display processing summary
                pdfProcessor.DisplayProcessingSummary(pdfFiles);
                
                var validPdfFiles = pdfProcessor.GetValidPdfFiles(pdfFiles);
                if (!validPdfFiles.Any())
                {
                    Console.WriteLine("❌ No valid PDF files found to process.");
                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();
                    return 1;
                }

                // Ask user for confirmation before processing
                Console.WriteLine($"📋 Ready to process {validPdfFiles.Count} PDF files.");
                Console.Write("Continue? (Y/N): ");
                var response = Console.ReadLine()?.Trim().ToUpper();
                
                if (response != "Y" && response != "YES")
                {
                    Console.WriteLine("Operation cancelled by user.");
                    return 0;
                }

                // Process all PDF files
                var startTime = DateTime.Now;
                var successCount = await imageConverter.ProcessAllPdfFiles(validPdfFiles);
                var endTime = DateTime.Now;
                var duration = endTime - startTime;

                // Display final summary
                Console.WriteLine();
                imageConverter.DisplayConversionSummary(successCount, validPdfFiles.Count, pdfFiles);
                Console.WriteLine($"⏱️  Total processing time: {duration:mm\\:ss}");
                
                if (successCount == validPdfFiles.Count)
                {
                    Console.WriteLine("🎉 All PDF files processed successfully!");
                    return 0;
                }
                else
                {
                    Console.WriteLine($"⚠️  {validPdfFiles.Count - successCount} files had processing errors.");
                    return 2;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"💥 Fatal error: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
                return 3;
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
    }
}
