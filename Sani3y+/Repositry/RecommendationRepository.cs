using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Admin;
using Sani3y_.Enums;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class RecommendationRepository : IRecommendationRepository
    {
        private readonly AppDbContext _context;
        public RecommendationRepository(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<List<CraftsmanRecommendationResponseDto>> GetAllPendingRecommendationsAsync()
        {
            return await _context.CraftsmanRecommendations
                    .Include(r => r.Profession)
                    .Select(r => new CraftsmanRecommendationResponseDto
                    {
                        Id = r.Id,
                        UserId = r.User.Id,
                        CraftsmanFirstName = r.CraftsmanFirstName,
                        CraftsmanLastName = r.CraftsmanLastName,
                        Governorate = r.Governorate,
                        Location = r.Location,
                        PhoneNumber = r.PhoneNumber,
                        ProfessionName = r.Profession.Name,
                        PreviousWorkDescription = r.PreviousWorkDescription,
                        DateTheProjectDone = r.DateTheProjectDone,
                        PersonalPhotoPath = r.PersonalPhotoPath,
                        PreviousWorkPicturePaths = r.PreviousWorkPicturePaths,
                        Status = r.Status
                    }).ToListAsync();
                    }

        public async Task<CraftsmanRecommendationResponseDto?> GetRecommendationByIdAsync(int id)
        {
            return await _context.CraftsmanRecommendations
                  .Where(r => r.Id == id)
                  .Include(r => r.Profession)
                  .Select(r => new CraftsmanRecommendationResponseDto
                  {
                      Id = r.Id,
                      UserId = r.UserId,
                      CraftsmanFirstName = r.CraftsmanFirstName,
                      CraftsmanLastName = r.CraftsmanLastName,
                      Governorate = r.Governorate,
                      Location = r.Location,
                      PhoneNumber = r.PhoneNumber,
                      ProfessionName = r.Profession.Name,
                      PreviousWorkDescription = r.PreviousWorkDescription,
                      DateTheProjectDone = r.DateTheProjectDone,
                      PersonalPhotoPath = r.PersonalPhotoPath,
                      PreviousWorkPicturePaths = r.PreviousWorkPicturePaths,
                      Status = r.Status
                  })
                  .FirstOrDefaultAsync();
        }
        public async Task AddRecommendationAsync(CraftsmanRecommendation recommendation)
        {
            _context.CraftsmanRecommendations.Add(recommendation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateRecommendationAsync(CraftsmanRecommendation recommendation)
        {
            _context.CraftsmanRecommendations.Update(recommendation);
            await _context.SaveChangesAsync();
        }
        public async Task<CraftsmanRecommendation?> FindRecommendationEntityByIdAsync(int id)
        {
            return await _context.CraftsmanRecommendations.FindAsync(id);
        }
        public async Task<List<CraftsmanRecommendation>> GetAllApprovedRecommendationsAsync(string userId)
        {
            return await _context.CraftsmanRecommendations
                .Include(r => r.Profession)
                .Where(r => r.UserId == userId && r.Status == RecommendationStatus.Approved)
                .ToListAsync();
        }
    }
}
