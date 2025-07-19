using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Admin;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class AdminRatingRepo : IAdminRatingRepo
    {
        private readonly AppDbContext _context;
        public AdminRatingRepo(AppDbContext appDbContext)
        {
            _context = appDbContext;   
        }
        public async Task<List<RatingListDto>> GetAllRatingsAsync()
        {
            return await _context.Ratings
                .Select(r => new RatingListDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    UserFullName = r.User.FirstName + " " + r.User.LastName,
                    CraftsmanId = r.CraftsmanId,
                    CraftsmanFullName = r.Craftsman.FirstName + " " + r.Craftsman.LastName,
                    Stars = r.Stars,
                    CreatedAt = r.CreatedAt,
                    Description = r.Description,
                    ServiceRequestId = r.ServiceRequestId
                })
                .ToListAsync();
        }

        public async Task<bool> DeleteRatingAsync(int ratingId)
        {
            var rating = await _context.Ratings.FindAsync(ratingId);
            if (rating == null) return false;
            _context.Ratings.Remove(rating);
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
