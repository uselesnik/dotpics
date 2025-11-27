using StackExchange.Redis;
using System.Text.Json;
using DotPic.Models;

namespace DotPic.Services
{
    public class RedisService : IRedisService
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _db;
        private readonly string _instanceName;

        public RedisService(IConnectionMultiplexer redis, IConfiguration configuration)
        {
            try
            {
                _redis = redis;
                _db = redis.GetDatabase();
                _instanceName = configuration["Redis:InstanceName"] ?? "DotPic_";
                //Console.WriteLine("RedisService initialized successfully");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"RedisService initialization failed: {ex.Message}");
                throw;
            }
        }

        public async Task<long> IncrementViewCountAsync(string imageId)
        {
            try
            {
                var key = $"{_instanceName}views:{imageId}";
                //Console.WriteLine($"Incrementing view count for key: {key}");
                var result = await _db.StringIncrementAsync(key);
                //Console.WriteLine($"New view count: {result}");
                return result;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error incrementing view count for {imageId}: {ex.Message}");
                throw;
            }
        }

        public async Task<long> GetViewCountAsync(string imageId)
        {
            try
            {
                var key = $"{_instanceName}views:{imageId}";
                var result = await _db.StringGetAsync(key);
                var count = result.HasValue ? (long)result : 0;
                //Console.WriteLine($"GetViewCount for {imageId}: {count}");
                return count;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error getting view count for {imageId}: {ex.Message}");
                return 0;
            }
        }

        public async Task<Dictionary<string, long>> GetTopViewedImagesAsync(int count)
        {
            try
            {
                //Console.WriteLine("Getting top viewed images...");
                var server = _redis.GetServer(_redis.GetEndPoints().First());
                var keys = server.Keys(pattern: $"{_instanceName}views:*");
                
                //Console.WriteLine($"Found {keys.Count()} keys with view data");
                
                var viewCounts = new Dictionary<string, long>();
                
                foreach (var key in keys)
                {
                    var keyString = key.ToString();
                    var imageId = keyString.Replace($"{_instanceName}views:", "");
                    var views = await GetViewCountAsync(imageId);
                    viewCounts[imageId] = views;
                    //Console.WriteLine($"Image {imageId}: {views} views");
                }
                
                // Return top N by view count
                var topViewed = viewCounts
                    .OrderByDescending(x => x.Value)
                    .Take(count)
                    .ToDictionary(x => x.Key, x => x.Value);
                    
                //Console.WriteLine($"Returning {topViewed.Count} top viewed images");
                return topViewed;
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"Error getting top viewed images: {ex.Message}");
                return new Dictionary<string, long>();
            }
        }

        // Cache methods
        public async Task CacheImageAsync(StoredImage image)
        {
            var key = $"{_instanceName}image:{image.Id}";
            var serializedImage = JsonSerializer.Serialize(image);
            await _db.StringSetAsync(key, serializedImage, TimeSpan.FromMinutes(30)); // Cache for 30 minutes
        }

        public async Task<StoredImage?> GetCachedImageAsync(string imageId)
        {
            var key = $"{_instanceName}image:{imageId}";
            var cached = await _db.StringGetAsync(key);
            
            if (cached.HasValue)
            {
                return JsonSerializer.Deserialize<StoredImage>(cached!);
            }
            
            return null;
        }

        public async Task RemoveCachedImageAsync(string imageId)
        {
            var key = $"{_instanceName}image:{imageId}";
            await _db.KeyDeleteAsync(key);
        }

        // User activity tracking
        public async Task TrackUserActivityAsync(string userId, string activity)
        {
            var key = $"{_instanceName}activity:{userId}";
            var activityWithTimestamp = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {activity}";
            await _db.ListLeftPushAsync(key, activityWithTimestamp);
            await _db.ListTrimAsync(key, 0, 49); // Keep only last 50 activities
            await _db.KeyExpireAsync(key, TimeSpan.FromDays(7)); // Expire after 7 days
        }

        public async Task<List<string>> GetRecentActivitiesAsync(string userId, int count)
        {
            var key = $"{_instanceName}activity:{userId}";
            var activities = await _db.ListRangeAsync(key, 0, count - 1);
            return activities.Select(x => x.ToString()).ToList();
        }
    }
}