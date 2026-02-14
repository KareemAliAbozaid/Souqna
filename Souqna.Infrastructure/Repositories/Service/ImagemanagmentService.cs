using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Souqna.Domin.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souqna.Infrastructure.Repositories.Service
{
    public class ImagemanagmentService : IImagemanagmentService
    {
        private readonly IFileProvider fileProvider;

        public ImagemanagmentService(IFileProvider fileProvider)
        {
            this.fileProvider = fileProvider;
        }

      public async Task<List<string>> UploadImageAsync(IFormFileCollection files, string src)
        {
            List<string> savedImagePaths = new List<string>();
            var imageDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Images", src);
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var savePath = Path.Combine(imageDirectory, fileName);
                    var imageUrl = $"/Images/{src}/{fileName}";

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    savedImagePaths.Add(imageUrl);
                }
            }

            return savedImagePaths;
        }
      public void DeleteImageAsync(string relativePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            else
            {
                throw new FileNotFoundException($"The file '{relativePath}' does not exist.");
            }
        }
    }
}
