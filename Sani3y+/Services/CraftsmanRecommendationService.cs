using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Admin;
using Sani3y_.Dtos.User;
using Sani3y_.Enums;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public class CraftsmanRecommendationService : ICraftsmanRecommendationService
    {
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly IFileService _fileService;
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public CraftsmanRecommendationService(
            IRecommendationRepository recommendationRepository,
            IFileService fileService,
            AppDbContext context,
            UserManager<AppUser> userManager)
        {
            _recommendationRepository = recommendationRepository;
            _fileService = fileService;
            _context = context;
            _userManager = userManager;
        }

        public async Task<IEnumerable<CraftsmanRecommendationResponseDto>> GetAllPendingRecommendationsAsync()
        {
            return await _recommendationRepository.GetAllPendingRecommendationsAsync();
        }

        public async Task<CraftsmanRecommendationResponseDto?> GetRecommendationByIdAsync(int id)
        {
            return await _recommendationRepository.GetRecommendationByIdAsync(id);
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
                ProfessionId = recommendationDto.ProfessionId,
                PreviousWorkDescription = recommendationDto.PreviousWorkDescription,
                DateTheProjectDone = recommendationDto.DateTheProjectDone,
                UserId = userId,
                Status = RecommendationStatus.Pending
            };
            recommendation.PersonalPhotoPath = recommendationDto.PersonalPhoto != null
                ? await _fileService.SaveFileAsync(recommendationDto.PersonalPhoto)
                : null;

            if (recommendationDto.PreviousWorkPictures != null && recommendationDto.PreviousWorkPictures.Count > 0)
            {
                recommendation.PreviousWorkPicturePaths = new List<string>();
                foreach (var file in recommendationDto.PreviousWorkPictures)
                {
                    recommendation.PreviousWorkPicturePaths.Add(await _fileService.SaveFileAsync(file));
                }
            }

            await _recommendationRepository.AddRecommendationAsync(recommendation);
        }
        public async Task ApproveRecommendationAsync(int id)
        {
            var recommendation = await _recommendationRepository.FindRecommendationEntityByIdAsync(id);
            if (recommendation != null)
            {
                recommendation.Status = RecommendationStatus.Approved;
                await _recommendationRepository.UpdateRecommendationAsync(recommendation);
            }
        }
        public async Task RejectRecommendationAsync(int id)
        {
            var recommendation = await _recommendationRepository.FindRecommendationEntityByIdAsync(id);
            if (recommendation != null)
            {
                recommendation.Status = RecommendationStatus.Rejected;
                await _recommendationRepository.UpdateRecommendationAsync(recommendation);
            }
        }
        public async Task<List<RecommendedCraftsmanDto>> GetAllApprovedRecommendationsAsync(string userId)
        {
            var recommendations = await _recommendationRepository.GetAllApprovedRecommendationsAsync(userId);
            var result = new List<RecommendedCraftsmanDto>();

            foreach (var rec in recommendations)
            {
                // Check if the recommended craftsman has become a real craftsman (AppUser)
                var craftsmanUser = await _userManager.Users
                    .FirstOrDefaultAsync(u => u.PhoneNumber == rec.PhoneNumber && u.Role == "Craftsman");

                double avgRating = 0;
                bool isTrusted = false;
                string craftsmanId = rec.Id.ToString(); // fallback: recommendation id

                if (craftsmanUser != null)
                {
                    // Calculate average rating for the craftsman
                    var ratings = await _context.Ratings
                        .Where(r => r.CraftsmanId == craftsmanUser.Id)
                        .ToListAsync();

                    avgRating = ratings.Any() ? ratings.Average(r => r.Stars) : 0;
                    isTrusted = craftsmanUser.IsTrusted ?? false;
                    craftsmanId = craftsmanUser.Id; // real craftsman AppUser Id
                }

                result.Add(new RecommendedCraftsmanDto
                {
                    CraftsmanId = craftsmanId,
                    ProfileImage = rec.PersonalPhotoPath,
                    CraftsmanFullName = $"{rec.CraftsmanFirstName} {rec.CraftsmanLastName}",
                    Profession = rec.Profession?.Name,
                    Governate = rec.Governorate,
                    Location = rec.Location,
                    AvgRating = avgRating,
                    isTrusted = isTrusted
                });
            }

            return result;
        }
    }
}
