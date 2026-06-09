using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using Souqna.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souqna.Infrastructure.Repositories.Service
{
    public class ImageManagementService : IImageManagementService
    {
        private readonly IFileProvider fileProvider;
        private readonly IWebHostEnvironment env;

        public ImageManagementService(
            IFileProvider fileProvider,
            IWebHostEnvironment env)
        {
            this.fileProvider = fileProvider;
            this.env = env;
        }

        public async Task<List<string>> UploadImageAsync(IFormFileCollection files, string src)
        {
            List<string> savedImagePaths = new List<string>();
            var imageDirectory = Path.Combine(
      env.WebRootPath,
      "Images",
      src
  );
            if (!Directory.Exists(imageDirectory))
            {
                Directory.CreateDirectory(imageDirectory);
            }

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    var extension = Path.GetExtension(file.FileName);
                    var fileName = $"{Guid.NewGuid():N}{extension}";
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
            var fullPath = Path.Combine(
    env.WebRootPath,
    relativePath.TrimStart('/')
        .Replace("/", Path.DirectorySeparatorChar.ToString())
);

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
