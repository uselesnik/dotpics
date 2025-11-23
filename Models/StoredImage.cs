using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotPic.Models
{
    public class StoredImage
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string FileName { get; set; } = string.Empty;
        public string OriginalFileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public byte[] ImageData { get; set; } = Array.Empty<byte>();
        public long FileSize { get; set; }
        
        public string? Description { get; set; }
        public string? Tags { get; set; } // Comma-separated tags for searching
        
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public DateTime? LastAccessed { get; set; }
        
        public int Width { get; set; }
        public int Height { get; set; }
        
        // User info (if you want to track who uploaded)
        public string? UploadedBy { get; set; }
    }
}