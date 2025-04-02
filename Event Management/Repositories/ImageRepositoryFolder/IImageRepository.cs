namespace Event_Management.Repositories.ImageRepositoryFolder
{
    public interface IImageRepository
    {
        Task<string> GenerateImageSource(IFormFile imageFile);
        Task<string> ChangeUserProfileImage(int userId, IFormFile formFile);
        Task<string> ChangeOrganizerLogoImage(int organizerId, IFormFile formFile);
        Task<string> ChangeEventImages(int eventId, IEnumerable<IFormFile> formFile);
    }
}
