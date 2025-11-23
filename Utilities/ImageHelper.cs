namespace Images.Utilities
{
    public static class ImageHelper
    {
        public static string ConvertToBase64(byte[] imageData, string contentType)
        {
            if (imageData == null || imageData.Length == 0)
                return string.Empty;

            return $"data:{contentType};base64,{Convert.ToBase64String(imageData)}";
        }

        public static bool IsValidImage(IFormFile file)
        {
            if (file == null) return false;

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return false;

            // Check file size (max 5MB)
            if (file.Length > 5 * 1024 * 1024)
                return false;

            return true;
        }
    }
}