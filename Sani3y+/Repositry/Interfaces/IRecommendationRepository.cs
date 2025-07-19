using Sani3y_.Dtos.Admin;
using Sani3y_.Dtos.User;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IRecommendationRepository
    {
        Task<List<CraftsmanRecommendationResponseDto>> GetAllPendingRecommendationsAsync();
        Task<CraftsmanRecommendationResponseDto?> GetRecommendationByIdAsync(int id);
        Task<List<CraftsmanRecommendation>> GetAllApprovedRecommendationsAsync(string userId);
        Task AddRecommendationAsync(CraftsmanRecommendation recommendation);
        Task UpdateRecommendationAsync(CraftsmanRecommendation recommendation);
        Task<CraftsmanRecommendation?> FindRecommendationEntityByIdAsync(int id);
    }
}
