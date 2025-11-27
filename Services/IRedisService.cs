
using DotPic.Models;

namespace DotPic.Services
{
    public interface IRedisService
    {
        // View counter methods
        Task<long> IncrementViewCountAsync(string imageId);
        Task<long> GetViewCountAsync(string imageId);
        Task<Dictionary<string, long>> GetTopViewedImagesAsync(int count);
        
        // Cache methods
        Task CacheImageAsync(StoredImage image);
        Task<StoredImage?> GetCachedImageAsync(string imageId);
        Task RemoveCachedImageAsync(string imageId);
        
        // Session methods
        Task TrackUserActivityAsync(string userId, string activity);
        Task<List<string>> GetRecentActivitiesAsync(string userId, int count);
    }
}