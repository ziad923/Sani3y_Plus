using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Dtos.User;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IUserRepo
    {
        Task<RatingResponseDto> AddRatingAsync(string userId, string craftsmanId, UserRatingDto ratingDto);
        Task<List<UserRatingResponseDto>> GetUserRatingsAsync(string userId);
         Task<UserProfileDto> GetUserProfileAsync(string userId);
        Task<List<GetAllRequestsUser>> GetAllUserRequestsAsync(string userId);
         Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDto updateProfileDto);
        Task<ServiceResponseDto> CreateServiceRequestAsync(string userId,  ServiceRequestDto dto);
        Task<bool> AddRatingAsync(string requestId, int score, string? feedback);
        Task<bool> EditServiceRequestAsync(string requestId, string userId, ServiceRequestDto dto);
        Task<bool> CancelServiceRequestAsync(string requestId, string userId);
        Task<bool> MarkRequestAsCompleteAsync(string requestId, string userId);
        Task<List<CraftsmanPreviousWorkDto>> GetCraftsmanPreviousWorkAsync(string craftsmanId);
        Task<CraftsmanRatingsResponseDto> GetCraftsmanRatingsAsync(string craftsmanId);
        //
        Task<List<CraftsmanRecommendation>> GetAllPendingRecommendationsAsync();
        Task<CraftsmanRecommendation?> GetRecommendationByIdAsync(int id);
        Task AddRecommendationAsync(CraftsmanRecommendation recommendation);
        Task UpdateRecommendationAsync(CraftsmanRecommendation recommendation);
    }
}
