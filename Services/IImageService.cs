using DotPic.Models;

namespace DotPic.Services
{
    public interface IImageService
    {
        Task<List<StoredImage>> GetAllImagesAsync();
        Task<StoredImage?> GetImageAsync(string id);
        Task<StoredImage> UploadImageAsync(StoredImage image);
        Task DeleteImageAsync(string id);
        Task UpdateImageInfoAsync(string id, string description, string tags);
        Task<List<StoredImage>> SearchImagesAsync(string searchTerm);
    }
}