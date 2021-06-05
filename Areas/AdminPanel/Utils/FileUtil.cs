using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Fiorello.Areas.AdminPanel.Utils
{
    public static class FileUtil
    {
        public static async Task<string> GenerateFileAsync(string folderPath,IFormFile formFile)
        {
            var fileName = $"{Guid.NewGuid()}-{formFile.FileName}";
            var filePath = Path.Combine(folderPath, fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return fileName;
        }
        
        public static async Task<string> GenerateFileAsync(string folderPath,string secondFolderPath,IFormFile formFile)
        {
            var fileName = $"{Guid.NewGuid()}-{formFile.FileName}";
            var filePath = Path.Combine(folderPath, secondFolderPath, fileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(fileStream);
            }

            return fileName;
        }
    }
}
