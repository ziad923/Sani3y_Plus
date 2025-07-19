using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Admin;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Dtos.User;
using Sani3y_.Enums;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class UserRepo : IUserRepo // Add Logging - MiddleWares 
    {
        private readonly AppDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;
        public UserRepo(AppDbContext context, UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _context = context;
            _userManager = userManager;
            _env = env;
        }
        public async Task<RatingResponseDto> AddRatingAsync(string craftsmanId, string userId, UserRatingDto ratingDto)
        {
            var rating = new Rating
            {
                CraftsmanId = craftsmanId,
                UserId = userId,
                Stars = ratingDto.Stars,
                Description = ratingDto.Description
            };
            _context.Ratings.Add(rating);
            await _context.SaveChangesAsync();

            var user = await _context.Users.FindAsync(userId);

            return new RatingResponseDto
            {
                UserFullName = $"{user.FirstName} {user.LastName}",
                RatingValue = rating.Stars,
                Description = rating.Description,
                CreatedAt = DateTime.UtcNow
            };
        } // g

        public async Task<ServiceResponseDto> CreateServiceRequestAsync(string userId, ServiceRequestDto dto)
        {
            string requestNumber = $"REQ-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
            string malfunctionPictureUrl = null;
            if (dto.ImageFile != null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dto.ImageFile.FileName);
                var filePath = Path.Combine(_env.WebRootPath, "malfunction-pictures", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dto.ImageFile.CopyToAsync(stream);
                }

                malfunctionPictureUrl = $"/malfunction-pictures/{fileName}";
            }
            var serviceRequest = new ServiceRequest
            {
                UserId = userId,
                CraftsmanId = dto.CraftsmanId,
                ServiceDescription = dto.ServiceDescription,
                Address = dto.Address,
                StartDate = dto.StartDate,
                PhoneNumber = dto.PhoneNumber,
                SecondPhoneNumber = dto.SecondPhoneNumber,
                MalfunctionImagePath = malfunctionPictureUrl,
                RequestNumber = requestNumber,
                Status = OrderStatus.WaitingForAcceptance
            };
            _context.ServiceRequests.Add(serviceRequest);
            await _context.SaveChangesAsync();

            // Return the request number
            return new ServiceResponseDto
            {
                RequestNumber = requestNumber
            };

        } // g

        public async Task<bool> EditServiceRequestAsync(string requestId, string userId, ServiceRequestDto dto)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.RequestNumber == requestId && r.UserId == userId);
            if (request == null || request.Status != OrderStatus.WaitingForAcceptance) return false;

            request.ServiceDescription = dto.ServiceDescription;
            request.Address = dto.Address;
            request.StartDate = dto.StartDate;
            request.PhoneNumber = dto.PhoneNumber;
            request.SecondPhoneNumber = dto.SecondPhoneNumber;

            await _context.SaveChangesAsync();
            return true;
        } // g

        public async Task<bool> CancelServiceRequestAsync(string requestId, string userId)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.RequestNumber == requestId && r.UserId == userId);
            if (request == null || request.Status != OrderStatus.WaitingForAcceptance) return false;

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        } // g

        public async Task<bool> MarkRequestAsCompleteAsync(string requestId, string userId)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.RequestNumber == requestId && r.UserId == userId);
            if (request == null || request.Status != OrderStatus.UnderImplementation) return false;

            request.Status = OrderStatus.Completed;
            request.CompletedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        } // g

        public async Task<List<CraftsmanPreviousWorkDto>> GetCraftsmanPreviousWorkAsync(string craftsmanId)
        {
            // Get the craftsman's previous works
            var previousWorks = await _context.PreviousWorks
                .Where(p => p.CraftsmanId == craftsmanId)
                .Select(p => new CraftsmanPreviousWorkDto
                {
                    ProjectDescription = p.ProjectDescription,
                    DateProjectDone = p.DateJobDone,
                    ProjectPictures = p.PictureUrls != null ? p.PictureUrls : new List<string>() // Assuming PictureUrls is already a List<string>
                })
                .ToListAsync();
            return previousWorks;
        } // g

        public async Task<CraftsmanRatingsResponseDto> GetCraftsmanRatingsAsync(string craftsmanId)
        {
            var ratings = await _context.Ratings
                        .Where(r => r.CraftsmanId == craftsmanId)
                        .Include(r => r.User)
                        .Include(c => c.Craftsman)// Assuming 'User' is the navigation property for the user who rated
                        .ToListAsync();

            if (!ratings.Any())
            {
                return new CraftsmanRatingsResponseDto
                {
                    AverageRating = 0,
                    Ratings = new List<CraftsmanRatingDto>()
                };
            }

            var averageRating = ratings.Average(r => r.Stars);

            var ratingsDto = ratings.Select(r => new CraftsmanRatingDto
            {
                ProfilePicture = r.User.ProfileImagePath,
                FullName = $"{r.User.FirstName} {r.User.LastName}",
                DateOfRate = r.CreatedAt,
                RatingByStars = r.Stars,
                RatingDescription = r.Description
            }).ToList();

            // Compute rating distribution
            var totalRatings = ratings.Count;
            var distribution = ratings
                .GroupBy(r => r.Stars)
                .Select(g => new RatingDistributionDto
                {
                    Stars = g.Key,
                    Percentage = Math.Round((double)g.Count() / totalRatings * 100, 2)
                })
                .ToList();

            // Ensure all 1 to 5 are present (even if 0%)
            for (int i = 1; i <= 5; i++)
            {
                if (!distribution.Any(d => d.Stars == i))
                {
                    distribution.Add(new RatingDistributionDto
                    {
                        Stars = i,
                        Percentage = 0
                    });
                }
            }

            // Sort by Stars descending to match your frontend order
            distribution = distribution.OrderByDescending(d => d.Stars).ToList();
            return new CraftsmanRatingsResponseDto
            {
                AverageRating = Math.Round(averageRating, 2),
                RatingDistribution = distribution,
                Ratings = ratingsDto 
            };
        } // g

        public async Task<ServiceRequest?> GetServiceRequestByIdAsync(string requestId)
        {
            return await _context.ServiceRequests.FindAsync(requestId);
        }

        public async Task<List<GetAllRequestsUser>> GetAllRequestsAsync(string userId)
        {
            var userRequests = await _context.ServiceRequests
                           .Where(sr => sr.UserId == userId)
                           .Include(x => x.Craftsman)
                           .ThenInclude(c => c.Profession)
                          .Select(sr => new GetAllRequestsUser
                          {
                              Id = sr.Id,
                              RequestNumber = sr.RequestNumber,
                              CraftsmanProfession = sr.Craftsman.Profession.Name,
                              CraftsmanFullName = sr.Craftsman.FirstName + " " + sr.Craftsman.LastName,
                              ServiceDescription = sr.ServiceDescription,
                              OrderDate = sr.RequestDate,
                              OrderStatus = sr.Status
                          })
                          .ToListAsync();
            return userRequests;
        } //g

        public async Task<List<UserListDto>> GetAllUsersAsync()
        {
            return await _context.Users
                  .Where(u => u.Role == "User")
                  .Select(u => new UserListDto
                  {
                      Id = u.Id,
                      FullName = u.FirstName + " " + u.LastName,
                      ProfileImage = u.ProfileImagePath,
                      Email = u.Email,
                      PhoneNumer = u.PhoneNumber
                  }).ToListAsync();
        }

        public async Task<List<CraftsmanListDto>> GetAllCraftsmenAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Craftsman")
                .Include(u => u.Profession)
                .Include(u => u.Ratings)
                .Select(u => new CraftsmanListDto
                {
                    Id = u.Id,
                    FullName = u.FirstName + " " + u.LastName,
                    Email = u.Email,
                    PhoneNumer = u.PhoneNumber,
                    ProfileImage = u.ProfileImagePath,
                    Governorate = u.Governorate,
                    Location = u.Location,
                    CardImagePath = u.CardImagePath,
                    IsTrusted = u.IsTrusted,
                    Profession = u.Profession.Name,
                    AverageRating = _context.Ratings
                                        .Where(r => r.CraftsmanId == u.Id)
                                        .Any() ? _context.Ratings
                                        .Where(r => r.CraftsmanId == u.Id)
                                        .Average(r => r.Stars) : 0
                }).ToListAsync();
        }

        public async Task<bool> DeleteUserAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CraftsmanCardDto> GetCraftsmanProfileByIdAsync(string craftsmanId)
        {
            var craftsman = await _context.Users
                .Include(u => u.Profession)
                .FirstOrDefaultAsync(u => u.Id == craftsmanId && u.Role == "Craftsman");

            if (craftsman == null) return null;

            // Get ratings FROM the Ratings table WHERE this craftsman is rated
            var ratings = await _context.Ratings
                                .Where(r => r.CraftsmanId == craftsmanId)
                                .ToListAsync();

            double averageRating = 0;

            if (ratings.Any())
            {
                averageRating = Math.Round(ratings.Average(r => r.Stars), 2);
            }

            return new CraftsmanCardDto
            {
                ProfilePicture = craftsman.ProfileImagePath,
                FullName = $"{craftsman.FirstName} {craftsman.LastName}",
                Profession = craftsman.Profession?.Name,
                Governorate = craftsman.Governorate,
                Location = craftsman.Location,
                AverageRating = averageRating,
                isTrusted = craftsman.IsTrusted
            };
        }


        //public async Task AddServiceRequestAsync(ServiceRequest request)
        //{
        //    _context.ServiceRequests.Add(request);
        //    await _context.SaveChangesAsync();
        //}

        //public async Task<bool> UpdateServiceRequestAsync(ServiceRequest request)
        //{
        //    _context.ServiceRequests.Update(request);
        //    return await _context.SaveChangesAsync() > 0;
        //}
    }

}
