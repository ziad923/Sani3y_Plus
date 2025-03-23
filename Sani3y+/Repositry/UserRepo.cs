using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
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
        public async Task<List<GetAllRequestsUser>> GetAllUserRequestsAsync(string userId)
        {
            var userRequests = await _context.ServiceRequests
                             .Where(sr => sr.UserId == userId)
                            .Select(sr => new GetAllRequestsUser
                            {
                                RequestNumber = sr.RequestNumber,
                                CraftsmanProfession = sr.Craftsman.Profession,
                                CraftsmanFullName = sr.Craftsman.FirstName + " " + sr.Craftsman.LastName,
                                ServiceDescription = sr.ServiceDescription,
                                OrderDate = sr.RequestDate,
                                OrderStatus = sr.Status
                            })
                            .ToListAsync();
            return userRequests;
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
        }

       
        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new UserProfileDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<List<UserRatingResponseDto>> GetUserRatingsAsync(string userId)
        {
            return await _context.Ratings
        .Where(r => r.UserId == userId)
        .Select(r => new UserRatingResponseDto
        {
           CraftsmanFullName  = r.Craftsman.FirstName + " " + r.Craftsman.LastName, // Combine First & Last Name
            CraftsmanProfession = r.Craftsman.Profession,
            Stars = r.Stars,
            Description = r.Description,
            CreatedAt = r.CreatedAt
        })
        .ToListAsync();
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Update only the modified fields
            if (!string.IsNullOrEmpty(updateProfileDto.FirstName))
            {
                user.FirstName = updateProfileDto.FirstName;
            }

            if (!string.IsNullOrEmpty(updateProfileDto.LastName))
            {
                user.LastName = updateProfileDto.LastName;
            }

            if (!string.IsNullOrEmpty(updateProfileDto.Email))
            {
                user.Email = updateProfileDto.Email;
                user.UserName = updateProfileDto.Email; // Update UserName if it's tied to Email
            }

            if (!string.IsNullOrEmpty(updateProfileDto.PhoneNumber))
            {
                user.PhoneNumber = updateProfileDto.PhoneNumber;
            }

            await _context.SaveChangesAsync();

            return true;
        }

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
        }

        public async Task<bool> CancelServiceRequestAsync(string requestId, string userId)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.RequestNumber == requestId && r.UserId == userId);
            if (request == null || request.Status != OrderStatus.WaitingForAcceptance) return false;

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkRequestAsCompleteAsync(string requestId, string userId)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.RequestNumber == requestId && r.UserId == userId);
            if (request == null || request.Status != OrderStatus.UnderImplementation) return false;

            request.Status = OrderStatus.Completed;
            request.CompletedDate = DateTime.UtcNow.Date;
            await _context.SaveChangesAsync();
            return true;
        }

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
        }

        public async Task<CraftsmanRatingsResponseDto> GetCraftsmanRatingsAsync(string craftsmanId)
        {
            var ratings = await _context.Ratings
                        .Where(r => r.CraftsmanId == craftsmanId)
                        .Include(r => r.User)  // Assuming 'User' is the navigation property for the user who rated
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
                FullName = $"{r.User.FirstName} {r.User.LastName}",
                DateOfRate = r.CreatedAt,
                RatingByStars = r.Stars,
                RatingDescription = r.Description
            }).ToList();

            return new CraftsmanRatingsResponseDto
            {
                AverageRating = Math.Round(averageRating, 2),
                Ratings = ratingsDto
            };
        }

        public async Task<List<CraftsmanRecommendation>> GetAllPendingRecommendationsAsync()
        {
            return await _context.CraftsmanRecommendations
            .Where(r => r.Status == RecommendationStatus.Pending)
            .ToListAsync();
        }

        public async Task<CraftsmanRecommendation?> GetRecommendationByIdAsync(int id)
        {
            return await _context.CraftsmanRecommendations.FindAsync(id);
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
    }
    
}
