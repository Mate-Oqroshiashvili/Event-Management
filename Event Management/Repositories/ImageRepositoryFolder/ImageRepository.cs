using Event_Management.Data;
using Event_Management.Exceptions;

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

        public async Task<string> ChangeUserProfileImage(int userId, IFormFile formFile)
        {
            try
            {
                var user = await _context.Users.FindAsync(userId) ?? throw new NotFoundException("User not found!");
                
                var imageUrl = await GenerateImageSource(formFile);
                user.ProfilePicture = imageUrl;

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return "Profile picture updated successfully!";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<string> ChangeOrganizerLogoImage(int organizerId, IFormFile formFile)
        {
            try
            {
                var organizer = await _context.Organizers.FindAsync(organizerId) ?? throw new NotFoundException("Organizer not found!");

                var imageUrl = await GenerateImageSource(formFile);
                organizer.LogoUrl = imageUrl;

                _context.Organizers.Update(organizer);
                await _context.SaveChangesAsync();

                return "Organizer's logo updated successfully!";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }

        public async Task<string> ChangeEventImages(int eventId, List<string> existingImages, IEnumerable<IFormFile> formFiles)
        {
            try
            {
                var @event = await _context.Events.FindAsync(eventId) ?? throw new NotFoundException("Event not found!");

                @event.Images ??= new List<string>();

                @event.Images = @event.Images.Where(img => existingImages.Contains(img)).ToList();

                if (formFiles != null)
                {
                    foreach (var file in formFiles)
                    {
                        var imageUrl = await GenerateImageSource(file);
                        @event.Images.Add(imageUrl);
                    }
                }

                _context.Events.Update(@event);
                await _context.SaveChangesAsync();

                return "Event images updated successfully!";
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message, ex.InnerException);
            }
        }
    }
}
