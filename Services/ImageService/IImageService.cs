namespace Caker.Services.ImageService
{
    public interface IImageService
    {
        Task<string> SaveImageAsync(IFormFile image, int userId);
    }
}
