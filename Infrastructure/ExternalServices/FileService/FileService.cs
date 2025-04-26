using Application.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.ExternalServices.FileService
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _uploadsBaseDirectory;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            // Use ContentRootPath instead of WebRootPath and create an 'Uploads' directory
            _uploadsBaseDirectory = Path.Combine(_webHostEnvironment.ContentRootPath, "Uploads");
            
            // Ensure the base uploads directory exists
            if (!Directory.Exists(_uploadsBaseDirectory))
            {
                Directory.CreateDirectory(_uploadsBaseDirectory);
            }
        }

        public void DeleteFile(string filePath)
        {
            // Convert the relative path to absolute
            string fullPath = Path.Combine(_uploadsBaseDirectory, filePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                return string.Empty;
            }

            // Create folder within uploads directory
            string directoryPath = Path.Combine(_uploadsBaseDirectory, folderName);
            
            // Create directory if it doesn't exist
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Generate unique filename
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(directoryPath, uniqueFileName);
            
            // Save file
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            // Return relative path (from uploads base directory)
            return Path.Combine(folderName, uniqueFileName);
        }
    }
}