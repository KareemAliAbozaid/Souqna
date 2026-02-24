using Microsoft.AspNetCore.Http;

namespace Souqna.Application.Interfaces.Services
{
    public interface IImageManagementService
    {
        Task<List<string>> UploadImageAsync(IFormFileCollection files, string src);
        void DeleteImageAsync(string src);
    }
}
