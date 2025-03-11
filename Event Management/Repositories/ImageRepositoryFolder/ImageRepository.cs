using Event_Management.Data;

namespace Event_Management.Repositories.ImageRepositoryFolder
{
    public class ImageRepository : IImageRepository
    {
        private static IHttpContextAccessor _contextAccessor;

        private readonly IWebHostEnvironment _enviroment;
        private readonly DataContext _context;
        public ImageRepository(IWebHostEnvironment enviroment, IHttpContextAccessor httpContextAccessor, DataContext context)
        {
            _enviroment = enviroment;
            _context = context;
            _contextAccessor = httpContextAccessor;
        }

        public async Task<string> GenerateImageSource(IFormFile imageFile)
        {
            string contentPath = _enviroment.ContentRootPath;

            string folder = Descriptive.CreateImageDirectory(contentPath, "Uploads\\");

            string newFileName = Descriptive.GenerateImageSourceWithExtention(folder, imageFile);

            var request = _contextAccessor.HttpContext.Request;

            string currentLocalHostUrl = $"{request.Scheme}://{request.Host}";

            string forWritingInFIle = folder + newFileName;

            string final = $"{currentLocalHostUrl}/Uploads/{newFileName}";

            Descriptive.WriteImageInFile(forWritingInFIle, imageFile);

            return final;
        }
    }
}
