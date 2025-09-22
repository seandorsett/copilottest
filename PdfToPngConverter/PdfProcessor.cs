using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

namespace PdfToPngConverter
{
    public class PdfInfo
    {
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int PageCount { get; set; }
        public List<int> SelectedPages { get; set; } = new List<int>();
        public bool IsValid { get; set; } = true;
        public string ErrorMessage { get; set; } = string.Empty;
    }

    public class PdfProcessor
    {
        private readonly AppConfig _config;
        private readonly Random _random;

        public PdfProcessor(AppConfig config)
        {
            _config = config;
            _random = new Random(_config.RandomSeed);
        }

        public List<PdfInfo> DiscoverPdfFiles()
        {
            var pdfFiles = new List<PdfInfo>();

            try
            {
                var searchOption = _config.RecursiveSearch ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                var pdfPaths = Directory.GetFiles(_config.SourceFolder, "*.pdf", searchOption);

                Console.WriteLine($"Found {pdfPaths.Length} PDF files in {_config.SourceFolder}");

                foreach (var pdfPath in pdfPaths)
                {
                    var pdfInfo = ProcessPdfFile(pdfPath);
                    pdfFiles.Add(pdfInfo);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error discovering PDF files: {ex.Message}");
            }

            return pdfFiles;
        }

        private PdfInfo ProcessPdfFile(string filePath)
        {
            var pdfInfo = new PdfInfo
            {
                FilePath = filePath,
                FileName = Path.GetFileNameWithoutExtension(filePath)
            };

            try
            {
                using (var pdfReader = new PdfReader(filePath))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    pdfInfo.PageCount = pdfDocument.GetNumberOfPages();
                    pdfInfo.SelectedPages = SelectRandomPages(pdfInfo.PageCount);

                    Console.WriteLine($"✓ {pdfInfo.FileName}: {pdfInfo.PageCount} pages, selected pages: [{string.Join(", ", pdfInfo.SelectedPages)}]");
                }
            }
            catch (Exception ex)
            {
                pdfInfo.IsValid = false;
                pdfInfo.ErrorMessage = ex.Message;
                Console.WriteLine($"✗ Error processing {pdfInfo.FileName}: {ex.Message}");
            }

            return pdfInfo;
        }

        private List<int> SelectRandomPages(int totalPages)
        {
            var selectedPages = new List<int>();
            var pagesToSelect = Math.Min(3, totalPages);

            if (totalPages <= 3)
            {
                // If 3 or fewer pages, select all pages
                for (int i = 1; i <= totalPages; i++)
                {
                    selectedPages.Add(i);
                }
            }
            else
            {
                // Randomly select 3 pages
                var availablePages = Enumerable.Range(1, totalPages).ToList();
                
                for (int i = 0; i < 3; i++)
                {
                    var randomIndex = _random.Next(availablePages.Count);
                    selectedPages.Add(availablePages[randomIndex]);
                    availablePages.RemoveAt(randomIndex);
                }

                selectedPages.Sort();
            }

            return selectedPages;
        }

        public byte[] ExtractPageAsImage(string pdfPath, int pageNumber)
        {
            try
            {
                using (var pdfReader = new PdfReader(pdfPath))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    if (pageNumber < 1 || pageNumber > pdfDocument.GetNumberOfPages())
                    {
                        throw new ArgumentOutOfRangeException(nameof(pageNumber), 
                            $"Page number {pageNumber} is out of range. PDF has {pdfDocument.GetNumberOfPages()} pages.");
                    }

                    var page = pdfDocument.GetPage(pageNumber);
                    
                    // Get page dimensions
                    var pageSize = page.GetPageSize();
                    var width = pageSize.GetWidth();
                    var height = pageSize.GetHeight();

                    // Calculate DPI scaling
                    var dpiScale = _config.PngQuality / 72.0f; // 72 DPI is default
                    var scaledWidth = (int)(width * dpiScale);
                    var scaledHeight = (int)(height * dpiScale);

                    // This is a simplified approach - in reality, you'd need to render the PDF page
                    // For now, we'll return a placeholder that indicates the page info
                    // In a real implementation, you'd use a PDF rendering library like PDFSharp or similar
                    
                    // Create a simple text representation for this demonstration
                    var pageInfo = $"PDF: {Path.GetFileName(pdfPath)}\nPage: {pageNumber}\nSize: {scaledWidth}x{scaledHeight}\nDPI: {_config.PngQuality}";
                    return System.Text.Encoding.UTF8.GetBytes(pageInfo);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to extract page {pageNumber} from PDF: {ex.Message}", ex);
            }
        }

        public bool ValidatePdfFile(string filePath)
        {
            try
            {
                using (var pdfReader = new PdfReader(filePath))
                using (var pdfDocument = new PdfDocument(pdfReader))
                {
                    // Try to read the first page to validate the PDF
                    if (pdfDocument.GetNumberOfPages() > 0)
                    {
                        var firstPage = pdfDocument.GetPage(1);
                        return firstPage != null;
                    }
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public List<PdfInfo> GetValidPdfFiles(List<PdfInfo> allPdfFiles)
        {
            return allPdfFiles.Where(pdf => pdf.IsValid).ToList();
        }

        public void DisplayProcessingSummary(List<PdfInfo> pdfFiles)
        {
            var validFiles = pdfFiles.Where(p => p.IsValid).ToList();
            var invalidFiles = pdfFiles.Where(p => !p.IsValid).ToList();

            Console.WriteLine("\n=== Processing Summary ===");
            Console.WriteLine($"Total PDF files found: {pdfFiles.Count}");
            Console.WriteLine($"Valid PDF files: {validFiles.Count}");
            Console.WriteLine($"Invalid/Error files: {invalidFiles.Count}");

            if (invalidFiles.Any())
            {
                Console.WriteLine("\nFiles with errors:");
                foreach (var invalid in invalidFiles)
                {
                    Console.WriteLine($"  ✗ {invalid.FileName}: {invalid.ErrorMessage}");
                }
            }

            var totalPagesToProcess = validFiles.Sum(p => p.SelectedPages.Count);
            Console.WriteLine($"Total pages to convert: {totalPagesToProcess}");
            Console.WriteLine("==========================\n");
        }
    }
}
