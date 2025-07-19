using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Sani3y_.Data;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.Home;
using Sani3y_.Dtos.User;
using Sani3y_.Repositry.Interfaces;
using System.Security.Claims;

namespace Sani3y_.Services
{
    public class HomeService : IHomeService
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;
        private const string HomePageCacheKey = "HomePageSummary_Global";
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HomeService(AppDbContext context, IMemoryCache cache, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _cache = cache;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<SystemStatsDto> GetSystemStatsAsync()
        {
            int totalOrders = await _context.ServiceRequests.CountAsync();
            int totalCraftsmen = await _context.Users.CountAsync(u => u.Role == "Craftsman");

            return new SystemStatsDto
            {
                TotalOrders = totalOrders,
                TotalCraftsmen = totalCraftsmen
            };
        }
        public async Task<HomePageSummaryDto> GetHomePageSummaryAsync()
        {
            // Step 1: Get cached global data if exists
            if (!_cache.TryGetValue(HomePageCacheKey, out HomePageSummaryDto globalData))
            {
                // Not in cache, load from DB
                var desiredProfessionNames = new List<string>
                {
                    "سبّــاك",
                    "كهربائي",
                    "نقاش",
                    "نجار موبيليا",
                    "حــــدّاد كـريـتال",
                    "بــنّــــاء"
                };

                var topProfessionIds = await _context.ServiceRequests
                    .Where(sr => sr.Craftsman.ProfessionId != null)
                    .GroupBy(sr => sr.Craftsman.ProfessionId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .Take(6)
                    .ToListAsync();

                var topProfessions = await _context.Professions
                    .Where(p => topProfessionIds.Contains(p.Id))
                    .Select(p => new TopProfessionDto
                    {
                        ProfessionId = p.Id,
                        ProfessionName = p.Name,
                        ImagePath = p.ImagePath
                    })
                    .ToListAsync();

                // Fill from desiredProfessionNames if less than 6
                if (topProfessions.Count < 6)
                {
                    var existingIds = topProfessions.Select(p => p.ProfessionId).ToList();

                    var fallback = await _context.Professions
                        .Where(p => desiredProfessionNames.Contains(p.Name) && !existingIds.Contains(p.Id))
                        .Take(6 - topProfessions.Count)
                        .Select(p => new TopProfessionDto
                        {
                            ProfessionId = p.Id,
                            ProfessionName = p.Name,
                            ImagePath = p.ImagePath
                        })
                        .ToListAsync();

                    topProfessions.AddRange(fallback);
                }

                var topCraftsmenIds = await _context.ServiceRequests
                    .Where(sr => sr.CraftsmanId != null)
                    .GroupBy(sr => sr.CraftsmanId)
                    .OrderByDescending(g => g.Count())
                    .Select(g => g.Key)
                    .Take(4)
                    .ToListAsync();

                var topCraftsmen = await _context.Users
                    .Include(u => u.Profession)
                    .Where(u => topCraftsmenIds.Contains(u.Id) && u.Role == "Craftsman")
                    .ToListAsync();

                var ratings = await _context.Ratings
                    .Where(r => topCraftsmenIds.Contains(r.ServiceRequest.CraftsmanId))
                    .GroupBy(r => r.ServiceRequest.CraftsmanId)
                    .Select(g => new
                    {
                        CraftsmanId = g.Key,
                        AverageRating = g.Average(r => r.Stars)
                    })
                    .ToListAsync();

                var topCraftsmenDetails = topCraftsmen.Select(u => new TopCraftsmanDto
                {
                    UserId = u.Id,
                    ProfileImage = u.ProfileImagePath,
                    FullName = $"{u.FirstName} {u.LastName}",
                    Profession = u.Profession?.Name,
                    Governaute = u.Governorate,
                    Location = u.Location,
                    IsTrusted = u.IsTrusted,
                    AverageRating = ratings.FirstOrDefault(r => r.CraftsmanId == u.Id)?.AverageRating ?? 0
                }).ToList();

                var recentRatings = await _context.Ratings
                    .OrderByDescending(r => r.CreatedAt)
                    .Take(5)
                    .Select(r => new HomeUserRatingDto
                    {
                        ProfileImage = r.User.ProfileImagePath,
                        UserFullName = r.User.FirstName + " " + r.User.LastName,
                        Stars = r.Stars,
                        Description = r.Description
                    })
                    .ToListAsync();

                var systemStats = await GetSystemStatsAsync();
                globalData = new HomePageSummaryDto
                {
                    // Leave UserFullName empty here
                    UserFullName = string.Empty,
                    TopProfessions = topProfessions,
                    TopCraftsmen = topCraftsmenDetails,
                    RecentRatings = recentRatings,
                    TotalOrders = systemStats.TotalOrders,
                    TotalCraftsmen = systemStats.TotalCraftsmen,
                };

                // Cache for 10 mins
                _cache.Set(HomePageCacheKey, globalData, new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });
            }

            // Step 2: Get UserFullName fresh (not cached)
            string userFullName = "زائر";
            string userProfileImage = "/assets/guest avatar.png";
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                var user = await _context.Users.FindAsync(userId);
                if (user != null)
                {
                    userFullName = $"{user.FirstName} {user.LastName}";
                    userProfileImage = string.IsNullOrEmpty(user.ProfileImagePath)
                                      ? "/assets/guest avatar.png"
                                      : user.ProfileImagePath; 
                }
            }

            // Step 3: Return merged result (global + fresh UserFullName)
            return new HomePageSummaryDto
            {
                ProfilePicture = userProfileImage,
                UserFullName = userFullName,
                TopProfessions = globalData.TopProfessions,
                TopCraftsmen = globalData.TopCraftsmen,
                RecentRatings = globalData.RecentRatings,
                TotalOrders = globalData.TotalOrders,
                TotalCraftsmen = globalData.TotalCraftsmen,
            };
        }
    }
}
