using MongoDB.Driver;
using DotPic.Models;

namespace DotPic.Services
{
    public class ImageService : IImageService
    {
        private readonly IMongoCollection<StoredImage> _images;

        public ImageService(MongoDbSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);
            _images = database.GetCollection<StoredImage>("Images");
        }

        public async Task<List<StoredImage>> GetAllImagesAsync()
        {
            return await _images.Find(image => true)
                .SortByDescending(image => image.UploadDate)
                .ToListAsync();
        }

        public async Task<StoredImage?> GetImageAsync(string id)
        {
            return await _images.Find(image => image.Id == id).FirstOrDefaultAsync();
        }

        public async Task<StoredImage> UploadImageAsync(StoredImage image)
        {
            await _images.InsertOneAsync(image);
            return image;
        }

        public async Task DeleteImageAsync(string id)
        {
            await _images.DeleteOneAsync(image => image.Id == id);
        }

        public async Task UpdateImageInfoAsync(string id, string description, string tags)
        {
            var update = Builders<StoredImage>.Update
                .Set(image => image.Description, description)
                .Set(image => image.Tags, tags);

            await _images.UpdateOneAsync(image => image.Id == id, update);
        }

        public async Task<List<StoredImage>> SearchImagesAsync(string searchTerm)
        {
            var filter = Builders<StoredImage>.Filter.Or(
                Builders<StoredImage>.Filter.Regex(image => image.FileName, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<StoredImage>.Filter.Regex(image => image.Description, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i")),
                Builders<StoredImage>.Filter.Regex(image => image.Tags, new MongoDB.Bson.BsonRegularExpression(searchTerm, "i"))
            );

            return await _images.Find(filter)
                .SortByDescending(image => image.UploadDate)
                .ToListAsync();
        }
    }
}