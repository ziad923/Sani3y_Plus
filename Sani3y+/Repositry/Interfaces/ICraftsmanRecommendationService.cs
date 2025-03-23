using Sani3y_.Dtos.User;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface ICraftsmanRecommendationService
    {
        Task<IEnumerable<CraftsmanRecommendation>> GetAllPendingRecommendationsAsync();
        Task<CraftsmanRecommendation> GetRecommendationByIdAsync(int id);
        Task AddRecommendationAsync(CraftsmanRecommendationDto recommendationDto, string userId);
        Task ApproveRecommendationAsync(int id);
        Task RejectRecommendationAsync(int id);
    }
}
