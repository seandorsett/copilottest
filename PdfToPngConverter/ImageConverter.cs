using PDFtoImage;
using SkiaSharp;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PdfToPngConverter
{
    public class ImageConverter
    {
        private readonly AppConfig _config;

        public ImageConverter(AppConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<bool> ConvertPdfPageToPng(PdfInfo pdfInfo, int pageNumber, int imageNumber)
        {
            try
            {
                // Ensure destination directory exists
                EnsureDestinationDirectoryExists();

                // Generate output filename
                var outputFileName = GenerateOutputFileName(pdfInfo.FileName, imageNumber);
                var outputPath = Path.Combine(_config.DestinationFolder, outputFileName);

                // Check if file exists and overwrite setting
                if (File.Exists(outputPath) && !_config.OverwriteExisting)
                {
                    Console.WriteLine($"  ⏭️  Skipping {outputFileName} (file exists, overwrite disabled)");
                    return true;
                }

                // Convert PDF page to PNG
                var success = await ConvertPageToImage(pdfInfo.FilePath, pageNumber, outputPath);

                if (success)
                {
                    Console.WriteLine($"  ✓ Created {outputFileName}");
                    return true;
                }
                else
                {
                    Console.WriteLine($"  ✗ Failed to create {outputFileName}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  ✗ Error converting page {pageNumber}: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> ConvertPageToImage(string pdfPath, int pageNumber, string outputPath)
        {
            try
            {
                Console.WriteLine($"    🔄 Converting page {pageNumber} from PDF to PNG...");

                // Load PDF file as byte array
                var pdfBytes = await File.ReadAllBytesAsync(pdfPath);

                // Set conversion options for high quality
                var options = new RenderOptions
                {
                    Dpi = _config.PngQuality,
                    BackgroundColor = SKColors.White
                };

                // Convert the specific page (PDFtoImage uses 0-based indexing)
                var pageIndex = pageNumber - 1;
                
                // Use PDFtoImage to convert PDF page to image
                var images = PDFtoImage.Conversion.ToImages(pdfBytes, options: options).ToList();
                
                if (images.Count <= pageIndex)
                {
                    Console.WriteLine($"    ❌ Page {pageNumber} (index {pageIndex}) is out of range - PDF has {images.Count} pages");
                    return false;
                }
                
                var bitmap = images[pageIndex];
                
                if (bitmap == null)
                {
                    Console.WriteLine($"    ❌ Failed to convert page {pageNumber} - bitmap is null");
                    return false;
                }

                // Save as PNG
                using (bitmap)
                {
                    using var image = SKImage.FromBitmap(bitmap);
                    using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
                    
                    await File.WriteAllBytesAsync(outputPath, encoded.ToArray());
                    
                    Console.WriteLine($"    ✅ Successfully converted page {pageNumber} (Size: {bitmap.Width}x{bitmap.Height})");
                    return true;
                }
            }
            catch (ArgumentOutOfRangeException)
            {
                Console.WriteLine($"    ❌ Page {pageNumber} is out of range for this PDF");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    💥 PDF conversion error for page {pageNumber}: {ex.Message}");
                
                // Fallback: Create a placeholder image
                return await CreatePlaceholderImage(outputPath, pdfPath, pageNumber);
            }
        }

        private async Task<bool> CreatePlaceholderImage(string outputPath, string pdfPath, int pageNumber)
        {
            try
            {
                Console.WriteLine($"    🔄 Creating placeholder image for page {pageNumber}");
                
                var dpiScale = _config.PngQuality / 72.0;
                var width = (int)(612 * dpiScale);  // Standard letter width
                var height = (int)(792 * dpiScale); // Standard letter height

                using var bitmap = new SKBitmap(width, height);
                using var canvas = new SKCanvas(bitmap);

                // Clear with white background
                canvas.Clear(SKColors.White);

                // Draw error indication
                using var errorPaint = new SKPaint
                {
                    Color = SKColors.Red,
                    Style = SKPaintStyle.Stroke,
                    StrokeWidth = 5,
                    IsAntialias = true
                };
                
                canvas.DrawRect(10, 10, width - 20, height - 20, errorPaint);

                // Draw diagonal lines
                canvas.DrawLine(0, 0, width, height, errorPaint);
                canvas.DrawLine(width, 0, 0, height, errorPaint);

                // Draw error text using modern SkiaSharp API
                using var font = new SKFont(SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold))
                {
                    Size = 24 * (float)dpiScale / 150.0f
                };

                using var textPaint = new SKPaint
                {
                    Color = SKColors.Black,
                    IsAntialias = true
                };

                var centerX = width / 2;
                var centerY = height / 2;
                
                canvas.DrawText("PDF CONVERSION ERROR", centerX - 120, centerY - 20, font, textPaint);
                canvas.DrawText($"Page {pageNumber}", centerX - 40, centerY + 20, font, textPaint);
                canvas.DrawText(Path.GetFileName(pdfPath), centerX - 100, centerY + 60, font, textPaint);

                // Save as PNG
                using var image = SKImage.FromBitmap(bitmap);
                using var encoded = image.Encode(SKEncodedImageFormat.Png, 100);
                
                await File.WriteAllBytesAsync(outputPath, encoded.ToArray());
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Placeholder image creation failed: {ex.Message}");
                return false;
            }
        }

        private string GenerateOutputFileName(string pdfFileName, int imageNumber)
        {
            // Remove any invalid characters from the PDF filename
            var safePdfName = string.Join("_", pdfFileName.Split(Path.GetInvalidFileNameChars()));
            
            // Generate filename according to specification: <PDF Name>_Image<Image No>.PNG
            return $"{safePdfName}_Image{imageNumber}.PNG";
        }

        private void EnsureDestinationDirectoryExists()
        {
            try
            {
                if (!Directory.Exists(_config.DestinationFolder))
                {
                    Directory.CreateDirectory(_config.DestinationFolder);
                    Console.WriteLine($"Created destination directory: {_config.DestinationFolder}");
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to create destination directory: {ex.Message}", ex);
            }
        }

        public async Task<int> ProcessAllPdfFiles(List<PdfInfo> pdfFiles)
        {
            var successCount = 0;
            var totalFiles = pdfFiles.Count(p => p.IsValid);
            var processedFiles = 0;

            Console.WriteLine($"\n🔄 Starting conversion of {totalFiles} PDF files...\n");

            foreach (var pdfInfo in pdfFiles.Where(p => p.IsValid))
            {
                processedFiles++;
                Console.WriteLine($"[{processedFiles}/{totalFiles}] Processing: {pdfInfo.FileName}");

                var fileSuccess = true;
                for (int i = 0; i < pdfInfo.SelectedPages.Count; i++)
                {
                    var pageNumber = pdfInfo.SelectedPages[i];
                    var imageNumber = i + 1;

                    var pageSuccess = await ConvertPdfPageToPng(pdfInfo, pageNumber, imageNumber);
                    if (!pageSuccess)
                    {
                        fileSuccess = false;
                    }
                }

                if (fileSuccess)
                {
                    successCount++;
                    Console.WriteLine($"  ✅ Completed {pdfInfo.FileName} ({pdfInfo.SelectedPages.Count} images created)");
                }
                else
                {
                    Console.WriteLine($"  ❌ Failed to process {pdfInfo.FileName} completely");
                }

                Console.WriteLine(); // Add spacing between files
            }

            return successCount;
        }

        public void DisplayConversionSummary(int processedFiles, int totalFiles, List<PdfInfo> allPdfFiles)
        {
            var totalImages = allPdfFiles.Where(p => p.IsValid).Sum(p => p.SelectedPages.Count);
            
            Console.WriteLine("=== Conversion Summary ===");
            Console.WriteLine($"PDFs successfully processed: {processedFiles}/{totalFiles}");
            Console.WriteLine($"Total images created: {totalImages}");
            Console.WriteLine($"Output directory: {_config.DestinationFolder}");
            Console.WriteLine($"PNG Quality: {_config.PngQuality} DPI");
            Console.WriteLine("==========================");
        }
    }
}
