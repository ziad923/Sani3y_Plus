using Sani3y_.Dtos.Admin;
using Sani3y_.Dtos.User;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface ICraftsmanRecommendationService
    {
        Task<IEnumerable<CraftsmanRecommendationResponseDto>> GetAllPendingRecommendationsAsync();
        Task<CraftsmanRecommendationResponseDto?> GetRecommendationByIdAsync(int id);
        Task<List<RecommendedCraftsmanDto>> GetAllApprovedRecommendationsAsync(string userId);
        Task AddRecommendationAsync(CraftsmanRecommendationDto recommendationDto, string userId);
        Task ApproveRecommendationAsync(int id);
        Task RejectRecommendationAsync(int id);
    }
}
