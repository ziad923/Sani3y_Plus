namespace Sani3y_.Repositry.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<string> SavePictureAsync(IFormFile picture);
        void DeleteFile(string filePath);

	}
}
