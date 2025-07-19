using Sani3y_.Dtos.Admin;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IAdminRatingRepo
    {
        Task<List<RatingListDto>> GetAllRatingsAsync();
        Task<bool> DeleteRatingAsync(int ratingId);
    }
}
