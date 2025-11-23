using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotPic.Models
{
    public class User
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        
        // Image properties
        public byte[]? ProfileImage { get; set; }
        public string? ImageContentType { get; set; }
        public string? FileName { get; set; }
        public long? FileSize { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}