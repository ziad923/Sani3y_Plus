using Microsoft.AspNetCore.Hosting;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/uploads/{uniqueFileName}";
        }
        public async Task<string> SavePictureAsync(IFormFile picture)
        {
            if (picture == null || picture.Length == 0)
            {
                throw new ArgumentException("Invalid file");
            }

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";
        }
		public void DeleteFile(string filePath)
		{
			if (string.IsNullOrWhiteSpace(filePath))
				return;

			var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));

			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}
		}
		//public void DeleteOldProfileImage(string oldImagePath)
		//{
		//    if (string.IsNullOrEmpty(oldImagePath) || oldImagePath.Contains(DefaultAvatar))
		//        return;

		//    var fullPath = Path.Combine(_environment.WebRootPath, oldImagePath.TrimStart('/'));

		//    if (File.Exists(fullPath))
		//    {
		//        File.Delete(fullPath);
		//    }
		//}
	}
}
