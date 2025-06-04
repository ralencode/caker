namespace Caker.Services.ImageService
{
    public class FtpImageService : IImageService
    {
        private readonly string _ftpPath = "assets";

        public async Task<string> SaveImageAsync(IFormFile image, int userId)
        {
            var uploadsDir = Path.Combine(_ftpPath, userId.ToString());
            Directory.CreateDirectory(uploadsDir);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
            var filePath = Path.Combine(uploadsDir, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await image.CopyToAsync(stream);

            return $"assets/{userId}/{fileName}";
        }
    }
}
