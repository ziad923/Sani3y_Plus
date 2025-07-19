using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.User;
using Sani3y_.Enums;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class UserRatingRepository : IUserRatingRepository
    {
        private readonly AppDbContext _context;
        public UserRatingRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<bool> AddRatingAsync(string requestId, int score, string? feedback)
        {
            var serviceRequest = await _context.ServiceRequests
          .Include(sr => sr.Rating)
          .Include(a => a.Craftsman)
          .Include(s => s.User)
          .FirstOrDefaultAsync(sr => sr.RequestNumber == requestId && sr.Status == OrderStatus.Completed); // Ensure consistent naming

            if (serviceRequest == null) return false;

            var rating = new Rating
            {
                Stars = score,
                Description = feedback,
                ServiceRequestId = serviceRequest.Id,
                CraftsmanId = serviceRequest.Craftsman.Id,
                UserId = serviceRequest.User.Id
            };

            _context.Ratings.Add(rating);
            serviceRequest.Rating = rating; // Use navigation property

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<UserRatingResponseDto>> GetUserRatingsAsync(string userId)
        {
            return await _context.Ratings
                    .Where(r => r.UserId == userId)
                    .Include(r => r.Craftsman)
                    .ThenInclude(c => c.Profession)
                    .Select(r => new UserRatingResponseDto
                    {
                        CraftsamanProfilePicture = r.Craftsman.ProfileImagePath,
                        CraftsmanFullName = r.Craftsman.FirstName + " " + r.Craftsman.LastName,
                        CraftsmanProfession = r.Craftsman.Profession.Name,
                        Stars = r.Stars,
                        Description = r.Description,
                        CreatedAt = r.CreatedAt
                    })
                .ToListAsync();
        }
    }
}
