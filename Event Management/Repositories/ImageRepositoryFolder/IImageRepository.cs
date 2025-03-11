namespace Event_Management.Repositories.ImageRepositoryFolder
{
    public interface IImageRepository
    {
        Task<string> GenerateImageSource(IFormFile imageFile);
    }
}
