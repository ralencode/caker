namespace Caker.Services.ImageService
{
    public class LocalFileImageService : IImageService
    {
        private readonly string _storagePath = "/app/assets";

        public async Task<string> SaveImageAsync(IFormFile image, int userId)
        {
            var userDir = Path.Combine(_storagePath, userId.ToString());
            Directory.CreateDirectory(userDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(userDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"https://caker.ralen.top/assets/{userId}/{fileName}";
        }
    }
}
