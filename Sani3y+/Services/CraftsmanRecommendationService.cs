using Microsoft.AspNetCore.Hosting;
using Sani3y_.Dtos.User;
using Sani3y_.Enums;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public class CraftsmanRecommendationService : ICraftsmanRecommendationService
    {
        private readonly IUserRepo _userRepo;
        private readonly IWebHostEnvironment _environment;

        public CraftsmanRecommendationService(IUserRepo repository, IWebHostEnvironment environment)
        {
            _userRepo = repository;
            _environment = environment;
        }

        public async Task<IEnumerable<CraftsmanRecommendation>> GetAllPendingRecommendationsAsync()
        {
            return await _userRepo.GetAllPendingRecommendationsAsync();
        }

        public async Task<CraftsmanRecommendation> GetRecommendationByIdAsync(int id)
        {
            return await _userRepo.GetRecommendationByIdAsync(id);
        }
        public async Task AddRecommendationAsync(CraftsmanRecommendationDto recommendationDto, string userId)
        {
            var recommendation = new CraftsmanRecommendation
            {
                CraftsmanFirstName = recommendationDto.CraftsmanFirstName,
                CraftsmanLastName = recommendationDto.CraftsmanLastName,
                Governorate = recommendationDto.Governorate,
                Location = recommendationDto.Location,
                PhoneNumber = recommendationDto.PhoneNumber,
                Profession = recommendationDto.Profession,
                PreviousWorkDescription = recommendationDto.PreviousWorkDescription,
                DateTheProjectDone = recommendationDto.DateTheProjectDone,
                UserId = userId,
                Status = RecommendationStatus.Pending
            };

            // ✅ File Uploads
            recommendation.PersonalPhotoPath = recommendationDto.PersonalPhoto != null
                ? await SaveFileAsync(recommendationDto.PersonalPhoto)
                : null;

            if (recommendationDto.PreviousWorkPictures != null && recommendationDto.PreviousWorkPictures.Count > 0)
            {
                recommendation.PreviousWorkPicturePaths = new List<string>();
                foreach (var file in recommendationDto.PreviousWorkPictures)
                {
                    recommendation.PreviousWorkPicturePaths.Add(await SaveFileAsync(file));
                }
            }

            await _userRepo.AddRecommendationAsync(recommendation);
        }
        private async Task<string> SaveFileAsync(IFormFile file)
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
        public async Task ApproveRecommendationAsync(int id)
        {
            var recommendation = await _userRepo.GetRecommendationByIdAsync(id);
            if (recommendation != null)
            {
                recommendation.Status = RecommendationStatus.Approved;
                await _userRepo.UpdateRecommendationAsync(recommendation);
            }
        }
        public async Task RejectRecommendationAsync(int id)
        {
            var recommendation = await _userRepo.GetRecommendationByIdAsync(id);
            if (recommendation != null)
            {
                recommendation.Status = RecommendationStatus.Rejected;
                await _userRepo.UpdateRecommendationAsync(recommendation);
            }
        }
    }
}
