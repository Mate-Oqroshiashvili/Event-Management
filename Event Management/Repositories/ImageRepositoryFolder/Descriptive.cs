namespace Event_Management.Repositories.ImageRepositoryFolder
{
    public class Descriptive
    {
        // This class contains methods for handling image files, including generating file names, creating directories, and writing files to disk.

        // It is used to manage image uploads in the application.
        public static void WriteImageInFile(string newFileName, IFormFile imageFile)
        {
            var fileStream = new FileStream(newFileName, FileMode.Create);

            imageFile.CopyToAsync(fileStream);

        }

        // This method generates a new file name for an image based on the provided path and the uploaded file.
        public static string GenerateImageSourceWithExtention(string path, IFormFile formFile)
        {
            List<string> extentions = new List<string>() { ".jpg", ".png", "PNG", ".jpeg", ".jfif" };

            string ext = Path.GetExtension(formFile.FileName);

            if (!extentions.Contains(ext))
                throw new Exception("not valid extention");

            string newFileName = Guid.NewGuid().ToString();

            return newFileName + ext;
        }

        // This method creates a directory for storing images if it does not already exist.
        public static string CreateImageDirectory(string contentPath, string folderName)
        {
            var path = Path.Combine(contentPath, folderName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
