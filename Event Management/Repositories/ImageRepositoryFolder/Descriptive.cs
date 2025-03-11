namespace Event_Management.Repositories.ImageRepositoryFolder
{
    public class Descriptive
    {
        public static void WriteImageInFile(string newFileName, IFormFile imageFile)
        {
            var fileStream = new FileStream(newFileName, FileMode.Create);

            imageFile.CopyToAsync(fileStream);

        }

        public static string GenerateImageSourceWithExtention(string path, IFormFile formFile)
        {
            List<string> extentions = new List<string>() { ".jpg", ".png", "PNG", ".jpeg", ".jfif" };

            string ext = Path.GetExtension(formFile.FileName);

            if (!extentions.Contains(ext))
                throw new Exception("not valid extention");

            string newFileName = Guid.NewGuid().ToString();

            return newFileName + ext;
        }


        public static string CreateImageDirectory(string contentPath, string folderName)
        {
            var path = Path.Combine(contentPath, folderName);

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}
