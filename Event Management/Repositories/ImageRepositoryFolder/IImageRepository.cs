namespace Event_Management.Repositories.ImageRepositoryFolder
{
    public interface IImageRepository
    {
        Task<string> GenerateImageSource(IFormFile imageFile); // Generates a source URL for the given image file
        Task<string> ChangeUserProfileImage(int userId, IFormFile formFile); // Changes the profile image of a user
        Task<string> ChangeOrganizerLogoImage(int organizerId, IFormFile formFile); // Changes the logo image of an organizer
        Task<string> ChangeEventImages(int eventId, List<string> existingImages, IEnumerable<IFormFile> formFiles); // Changes the images of an event
    }
}
