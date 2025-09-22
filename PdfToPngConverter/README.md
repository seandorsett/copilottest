# PDF to PNG Converter - Installation & Usage Guide

## Overview
A Windows console application that processes multiple PDF documents by extracting random pages and converting them to PNG images.

## Features
✅ Batch processing of multiple PDF files  
✅ Configurable source and destination folders  
✅ Random page selection (up to 3 pages per PDF)  
✅ High-quality PNG output with configurable DPI  
✅ Standardized naming convention: `<PDF Name>_Image<Image No>.PNG`  
✅ Graceful error handling for corrupted or invalid PDFs  
✅ Progress tracking and detailed logging  
✅ Configuration via JSON file  

## System Requirements
- Windows 10/11
- .NET 9.0 Runtime
- Sufficient disk space for output images

## Installation

### Option 1: Run from Source
1. Ensure you have .NET 9.0 SDK installed
2. Clone or download the project
3. Navigate to the project directory
4. Run: `dotnet build`
5. Run: `dotnet run`

### Option 2: Publish Executable
1. Navigate to the project directory
2. Run: `dotnet publish -c Release -o publish`
3. Copy the `publish` folder to your desired location
4. Run `PdfToPngConverter.exe`

## Configuration

### config.json
The application uses a JSON configuration file that is automatically created on first run:

```json
{
  "SourceFolder": "C:\\Source\\PDFs",
  "DestinationFolder": "C:\\Output\\PNGs", 
  "PngQuality": 300,
  "RandomSeed": 12345,
  "OverwriteExisting": true,
  "RecursiveSearch": false
}
```

### Configuration Options

| Setting | Description | Default |
|---------|-------------|---------|
| `SourceFolder` | Directory containing PDF files to process | `C:\Source\PDFs` |
| `DestinationFolder` | Directory where PNG files will be saved | `C:\Output\PNGs` |
| `PngQuality` | Output image DPI (72-600) | `300` |
| `RandomSeed` | Seed for random page selection (for consistency) | `12345` |
| `OverwriteExisting` | Whether to overwrite existing PNG files | `true` |
| `RecursiveSearch` | Search subdirectories for PDF files | `false` |

## Usage

### Basic Usage
1. Place PDF files in your configured source folder
2. Run the application: `PdfToPngConverter.exe`
3. Review the configuration displayed
4. Confirm processing when prompted
5. Find output PNG files in the destination folder

### Command Line
```cmd
cd "C:\path\to\PdfToPngConverter"
PdfToPngConverter.exe
```

### Sample Output
```
🔄 PDF to PNG Converter
=======================

Configuration loaded from C:\Apps\PdfToPngConverter\config.json

=== Current Configuration ===
Source Folder: C:\Source\PDFs
Destination Folder: C:\Output\PNGs
PNG Quality: 300 DPI
Random Seed: 12345
Overwrite Existing: True
Recursive Search: False
===============================

🔍 Discovering PDF files...
Found 5 PDF files in C:\Source\PDFs
✓ Document1: 10 pages, selected pages: [2, 7, 9]
✓ Report2023: 25 pages, selected pages: [5, 12, 18]
✓ Manual: 2 pages, selected pages: [1, 2]
✗ Error processing CorruptedFile: PDF header not found.

=== Processing Summary ===
Total PDF files found: 4
Valid PDF files: 3
Invalid/Error files: 1
Total pages to convert: 8
==========================

📋 Ready to process 3 PDF files.
Continue? (Y/N): Y

🔄 Starting conversion of 3 PDF files...

[1/3] Processing: Document1
  ✓ Created Document1_Image1.PNG
  ✓ Created Document1_Image2.PNG
  ✓ Created Document1_Image3.PNG
  ✅ Completed Document1 (3 images created)

[2/3] Processing: Report2023
  ✓ Created Report2023_Image1.PNG
  ✓ Created Report2023_Image2.PNG
  ✓ Created Report2023_Image3.PNG
  ✅ Completed Report2023 (3 images created)

[3/3] Processing: Manual
  ✓ Created Manual_Image1.PNG
  ✓ Created Manual_Image2.PNG
  ✅ Completed Manual (2 images created)

=== Conversion Summary ===
PDFs successfully processed: 3/3
Total images created: 8
Output directory: C:\Output\PNGs
PNG Quality: 300 DPI
==========================
⏱️  Total processing time: 00:12
🎉 All PDF files processed successfully!

Press any key to exit...
```

## Output Files

### Naming Convention
- **Format**: `<PDF Name>_Image<Image No>.PNG`
- **Examples**:
  - `Annual Report 2023_Image1.PNG`
  - `User Manual_Image2.PNG`
  - `Technical Spec_Image3.PNG`

### Image Quality
- **DPI**: Configurable (default 300 DPI)
- **Format**: PNG with transparency support
- **Color**: RGB with alpha channel

## Error Handling

### Common Issues

| Issue | Description | Solution |
|-------|-------------|----------|
| Source folder not found | Configured folder doesn't exist | Create the folder or update config |
| No PDF files found | Source folder is empty | Add PDF files to source folder |
| PDF parsing errors | Corrupted or password-protected PDFs | Check PDF files, replace corrupted ones |
| Disk space issues | Insufficient space for output | Free up disk space |
| Permission errors | No write access to destination | Check folder permissions |

### Log Files
The application creates a log file `pdf_converter.log` in the application directory with detailed information about:
- Configuration loading
- PDF file discovery
- Processing progress  
- Error details
- Performance metrics

## Advanced Usage

### Batch Processing Tips
1. **Large volumes**: For 100+ PDFs, consider processing in smaller batches
2. **Performance**: Higher DPI settings will increase processing time
3. **Storage**: Plan for ~500KB-2MB per output image at 300 DPI

### Integration
The application returns exit codes for automation:
- `0`: Success - all files processed
- `1`: Configuration error
- `2`: Some files failed processing  
- `3`: Fatal application error

### Example Batch Script
```batch
@echo off
echo Starting PDF to PNG conversion...
cd "C:\Tools\PdfToPngConverter"
PdfToPngConverter.exe

if %ERRORLEVEL% == 0 (
    echo All files processed successfully!
) else (
    echo Some files failed. Check logs for details.
)
pause
```

## Troubleshooting

### Performance Issues
- Reduce PNG quality for faster processing
- Process fewer files at once
- Ensure sufficient RAM and disk space

### Output Quality Issues  
- Increase PNG quality setting (up to 600 DPI)
- Check source PDF quality
- Verify sufficient disk space

### File Access Issues
- Run as Administrator if needed
- Check folder permissions
- Ensure folders exist before running

## Support
For issues or questions:
1. Check the log file for detailed error information
2. Verify configuration settings
3. Test with a small number of PDF files first
4. Ensure all system requirements are met

## Version Information
- **Version**: 1.0
- **Framework**: .NET 9.0
- **Dependencies**: iText7, ImageSharp, PDFSharp
- **Platform**: Windows x64
