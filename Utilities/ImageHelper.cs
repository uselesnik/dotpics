using Microsoft.AspNetCore.Components.Forms;

namespace DotPic.Utilities
{
    public static class ImageHelper
    {
        public static string ConvertToBase64(byte[]? imageData, string? contentType)
        {
            if (imageData == null || imageData.Length == 0 || string.IsNullOrEmpty(contentType))
                return string.Empty;

            return $"data:{contentType};base64,{Convert.ToBase64String(imageData)}";
        }

        public static bool IsValidImage(IBrowserFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.Name).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return false;

            // Check file size (max 2MB for demo)
            if (file.Size > 2 * 1024 * 1024)
                return false;

            return true;
        }

        public static string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}