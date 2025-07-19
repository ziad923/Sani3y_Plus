using Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.User;
using Sani3y_.Enums;
using Sani3y_.Helpers;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class CraftsmanQueryRepo : ICraftsmanQueryRepo
    {
        private readonly AppDbContext _context;

        public CraftsmanQueryRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task<(List<CraftsmanFilterDto> Craftsmen, int TotalCount)> GetFilteredCraftsmenAsync(QuereyObject query)
        {
            int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
            int pageSize = query.PageSize > 0 ? query.PageSize : 10;

            var craftsmen = _context.Users
            .Where(u => u.Role == "Craftsman")
            .Include(c => c.Ratings)
            .Include(c => c.Profession)
            .AsQueryable();

            if (query.ProfessionId.HasValue)
            {
                craftsmen = craftsmen.Where(c => c.ProfessionId == query.ProfessionId.Value);
            }

            if (!string.IsNullOrEmpty(query.Location))
            {
                craftsmen = craftsmen.Where(u => u.Location.Contains(query.Location));
            }

            if (query.MinRating.HasValue)
            {
                craftsmen = craftsmen.Where(u => u.Ratings.Any() &&
                    u.Ratings.Average(r => r.Stars) >= query.MinRating.Value);
            }

            if (query.IsTrusted.HasValue)
            {
                craftsmen = craftsmen.Where(u => u.IsTrusted == query.IsTrusted.Value);
            }
            int totalCount = await craftsmen.CountAsync();
            var pagination = await craftsmen
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new CraftsmanFilterDto
                {
                    Id = x.Id,
                    ProfileImagePath = x.ProfileImagePath,
                    FullName = x.FirstName + " " + x.LastName, 
                    Profession = x.Profession.Name,
                    Governorate = x.Governorate,
                    Location = x.Location,
                    IdentityVerified = x.IsTrusted,
                   AverageRating = _context.Ratings
                    .Where(r => r.ServiceRequest.CraftsmanId == x.Id)
                    .Select(r => (double?)r.Stars)
                    .Average() ?? 0
                }).ToListAsync();

            return (pagination, totalCount);

        }

        public async Task<DashboardStatistics> GetDashboardStatisticsAsync(string craftsmanId)
        {
            var statistics = new DashboardStatistics();
            var craftsman = await _context.Users
               .Where(u => u.Id == craftsmanId && u.Role == "Craftsman")
               .Select(u => new
               {
                   FullName = u.FirstName + " " + u.LastName,
                   ProfileImage = string.IsNullOrEmpty(u.ProfileImagePath) ? "/assets/guest avatar.png" : u.ProfileImagePath
               })
               .FirstOrDefaultAsync();

            if (craftsman == null)
            {
                throw new Exception("Craftsman not found");
            }
           
            var averageRating = await _context.Ratings
                .Where(r => r.ServiceRequest.CraftsmanId == craftsmanId)
                .Select(r => (int?)r.Stars)
                .AverageAsync() ?? 0;

            var areasWorkedIn = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId)
                .Select(sr => sr.Address)
                .Distinct()
                .CountAsync();

            var totalIncomingRequests = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId)
                .CountAsync();

            var requestsCompleted = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.Completed)
                .CountAsync();

            var requestsUnderImplementation = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.UnderImplementation)
                .CountAsync();

            var requestsRejected = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.Canceled)
                .CountAsync();

            var responseTimes = await _context.ServiceRequests
            .Where(sr => sr.CraftsmanId == craftsmanId && sr.AcceptedDate.HasValue && sr.RequestDate <= sr.AcceptedDate)
                .Select(sr => (double?)EF.Functions.DateDiffMinute(sr.RequestDate, sr.AcceptedDate.Value) / 60) 
                 .Where(rt => rt >= 0) 
                     .ToListAsync();

            var executionTimes = await _context.ServiceRequests
     .Where(sr => sr.CraftsmanId == craftsmanId && sr.CompletedDate.HasValue && sr.AcceptedDate.HasValue && sr.AcceptedDate <= sr.CompletedDate)
     .Select(sr => (double?)EF.Functions.DateDiffHour(sr.AcceptedDate.Value, sr.CompletedDate.Value) / 24) 
     .Where(et => et >= 0)
     .ToListAsync();

            var monthlyProjects = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId && sr.CompletedDate.HasValue)
                .GroupBy(sr => new { sr.CompletedDate.Value.Year, sr.CompletedDate.Value.Month })
                .Select(g => g.Count())
                .ToListAsync();

           
            statistics.FullName = craftsman.FullName;
            statistics.ProfileImage = craftsman.ProfileImage;
            statistics.AverageRating = Math.Round(averageRating, 2);
            statistics.AreasWorkedIn = areasWorkedIn;
            statistics.TotalIncomingRequests = totalIncomingRequests;
            statistics.ProjectsCompleted = requestsCompleted;
            statistics.RequestsCompleted = requestsCompleted;
            statistics.RequestsUnderImplementation = requestsUnderImplementation;
            statistics.RequestsRejected = requestsRejected;
            statistics.AverageResponseTime = responseTimes.Any()? Math.Round(responseTimes.Average(rt => rt.Value), 2): 0;
            statistics.AverageExecutionTime = executionTimes.Any()? Math.Max(1, Math.Round(executionTimes.Average(et => et.Value), 2)): 0;
            statistics.AverageMonthlyProjects = monthlyProjects.Any() ? (int)Math.Round(monthlyProjects.Average(), 0) : 0;

            return statistics;
        }
        public async Task<List<RatingResponseDto>> GetRatingsForCraftsmanAsync(string craftsmanId)
        {
            var ratings = await _context.Ratings
            .Where(r => r.CraftsmanId == craftsmanId)
            .Include(r => r.User) // Include the user who rated
            .Select(r => new RatingResponseDto
            {
               ImagePath = r.User.ProfileImagePath,
                UserFullName = $"{r.User.FirstName} {r.User.LastName}",
                CreatedAt = r.CreatedAt,
                RatingValue = r.Stars,
                Description = r.Description
            })
            .ToListAsync();

            return ratings;
        }




    }
}
