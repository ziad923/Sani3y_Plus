using Sani3y_.Dtos.User;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IUserRatingRepository
    {
        Task<bool> AddRatingAsync(string requestId, int score, string? feedback);
        Task<List<UserRatingResponseDto>> GetUserRatingsAsync(string userId);
    }
}
