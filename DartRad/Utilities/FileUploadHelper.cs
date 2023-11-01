namespace DartRad.Utilities
{
    public static class FileUploadHelper
    {
        public static async Task<string> SaveFile(IFormFile file, string rootPath, string fileDirectory)
        {
            try
            {
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;

                string filePath = Path.Combine(rootPath, fileDirectory.Substring(1));
                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
                string imageFilePath = Path.Combine(filePath, uniqueFileName);

                using (var fileStream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return uniqueFileName;
            }
            catch (Exception ex)
            {
                // log
            }
            return "";
        }

        public static async Task RemoveFile(string filename, string directory, string rootPath)
        {
            if (string.IsNullOrEmpty(filename))
            {
                return;
            }
            string fullPath = Path.Combine(rootPath, directory, filename);
            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch (Exception ex)
                {
                    // log
                }
            }
        }
    }
}
