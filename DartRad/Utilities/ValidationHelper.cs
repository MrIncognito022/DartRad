namespace DartRad.Utilities
{
    public static class ValidationHelper
    {
        public static bool IsImageFile(IFormFile file)
        {
            // Check if the Content-Type starts with "image/"
            return file.ContentType.StartsWith("image/");
        }
    }
}
