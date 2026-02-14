using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Souqna.Domin.Services
{
    public interface IImagemanagmentService
    {
        Task<List<string>> UploadImageAsync(IFormFileCollection files,string src);
        void DeleteImageAsync(string src);
    }
}
