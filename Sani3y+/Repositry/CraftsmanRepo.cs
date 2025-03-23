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
    public class CraftsmanRepo : ICraftsmanRepo
    {
        private readonly AppDbContext _context;

        public CraftsmanRepo(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddPreviousWorkAsync(string craftsmanId, PreviousWorkDto previousWorkDto)
        {
            var previousWork = new PreviousWork
            {
                ProjectDescription = previousWorkDto.ProjectDescription,
                DateJobDone = previousWorkDto.DateJobDone,
                CraftsmanId = craftsmanId
            };
            if (previousWorkDto.Pictures != null && previousWorkDto.Pictures.Any())
            {
                foreach (var picture in previousWorkDto.Pictures)
                {
                    // Save the picture to the file system or cloud (example: save to local folder)
                    var pictureUrl = await SavePictureAsync(picture);
                    previousWork.PictureUrls.Add(pictureUrl);
                }
            }

            // Add the previous work to the database
            _context.PreviousWorks.Add(previousWork);
            await _context.SaveChangesAsync();
        }
        private async Task<string> SavePictureAsync(IFormFile picture)
        {
            // You can use any method to save the file. Below is an example of saving to a local folder.
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(picture.FileName);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await picture.CopyToAsync(stream);
            }

            return $"/uploads/{fileName}";  // or return a relative URL depending on your application structure
        }

        public async Task<List<PreviousWorkResponseDto>> GetPreviousWork(string craftsmanId)
        {
            return await _context.PreviousWorks
                     .Where(pw => pw.CraftsmanId == craftsmanId)
                     .Select(pw => new PreviousWorkResponseDto
                     {
                         ProjectDescription = pw.ProjectDescription,
                         DateJobDone = pw.DateJobDone,
                         PictureUrls = pw.PictureUrls
                     }).ToListAsync();
        }
        public async Task<bool> UpdatePreviousWorkAsync(int id, PreviousWorkDto dto, string craftsmanId)
        {
            var work = await _context.PreviousWorks.FirstOrDefaultAsync(pw => pw.Id == id && pw.CraftsmanId == craftsmanId);
            if (work == null) return false;

            work.ProjectDescription = dto.ProjectDescription;
            work.DateJobDone = dto.DateJobDone;

            if (dto.Pictures != null && dto.Pictures.Any())
            {
                foreach (var picture in dto.Pictures)
                {
                    var url = await SavePictureAsync(picture);
                    work.PictureUrls.Add(url);
                }
            }

            await _context.SaveChangesAsync();
            return true;
        }
            public async Task<bool> DeletePreviousWorkAsync(int id, string craftsmanId)
        {
            var work = await _context.PreviousWorks.FirstOrDefaultAsync(pw => pw.Id == id && pw.CraftsmanId == craftsmanId);
            if (work == null) return false;

            _context.PreviousWorks.Remove(work);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<(List<CraftsmanFilterDto> Craftsmen, int TotalCount)> GetFilteredCraftsmenAsync(QuereyObject query)
        {
            int pageNumber = query.PageNumber > 0 ? query.PageNumber : 1;
            int pageSize = query.PageSize > 0 ? query.PageSize : 10;

            var craftsmen = _context.Users
            .Where(u => u.Role == "Craftsman")
            .Include(c => c.Ratings)
            .AsQueryable();

            if (!string.IsNullOrEmpty(query.Profession))
            {
                craftsmen = craftsmen.Where(x => x.Profession.Contains(query.Profession));
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
                    Profession = x.Profession,
                    Location = x.Location,
                    IdentityVerified = x.IsTrusted,
                    AverageRating = x.Ratings.Any() ? x.Ratings.Average(r => r.Stars) : 0
                }).ToListAsync();

            return (pagination, totalCount);

        }

        public async Task<List<RatingResponseDto>> GetRatingsForCraftsmanAsync(string craftsmanId)
        {
            var ratings = await _context.Ratings
            .Where(r => r.CraftsmanId == craftsmanId)
            .Include(r => r.User) // Include the user who rated
            .Select(r => new RatingResponseDto
            {
                UserFullName = $"{r.User.FirstName} {r.User.LastName}",
                CreatedAt = r.CreatedAt,
                RatingValue = r.Stars,
                Description = r.Description
            })
            .ToListAsync();

            return ratings;
        }
        //public async Task<DashboardStatistics> GetDashboardStatisticsAsync(string craftsmanId)
        //{
        //    var statistics = new DashboardStatistics();

        //    // Average Rating
        //    statistics.AverageRating = await _context.Ratings
        //        .Where(r => r.ServiceRequest.CraftsmanId == craftsmanId)
        //        .AverageAsync(r => (double?)r.Stars) ?? 0;

        //    // Areas Worked In
        //    statistics.AreasWorkedIn = await _context.ServiceRequests
        //        .Where(sr => sr.CraftsmanId == craftsmanId)
        //        .Select(sr => sr.Address)
        //        .Distinct()
        //        .CountAsync();

        //    // Projects Completed
        //    statistics.ProjectsCompleted = await _context.ServiceRequests
        //        .CountAsync(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.Completed);

        //    // General Statistics
        //    statistics.TotalIncomingRequests = await _context.ServiceRequests
        //        .CountAsync(sr => sr.CraftsmanId == craftsmanId);
        //    statistics.RequestsCompleted = await _context.ServiceRequests
        //        .CountAsync(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.Completed);
        //    statistics.RequestsUnderImplementation = await _context.ServiceRequests
        //        .CountAsync(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.UnderImplementation);
        //    statistics.RequestsRejected = await _context.ServiceRequests
        //        .CountAsync(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.Canceled);

        //    // Average Response Time
        //    // ✅ Average Response Time (in hours) → AcceptedDate - RequestDate
        //    var responseTimes = await _context.ServiceRequests
        //        .Where(sr => sr.CraftsmanId == craftsmanId && sr.AcceptedDate.HasValue)
        //        .Select(sr => EF.Functions.DateDiffHour(sr.RequestDate, sr.AcceptedDate.Value))
        //        .ToListAsync();
        //    statistics.AverageResponseTime = responseTimes.Any() ? Math.Round(responseTimes.Average(), 2) : 0;

        //    // Average Execution Time
        //    // ✅ Average Execution Time (in days) → CompletedDate - AcceptedDate
        //    var executionTimes = await _context.ServiceRequests
        //        .Where(sr => sr.CraftsmanId == craftsmanId && sr.CompletedDate.HasValue && sr.AcceptedDate.HasValue)
        //        .Select(sr => EF.Functions.DateDiffDay(sr.AcceptedDate.Value, sr.CompletedDate.Value))
        //        .ToListAsync();
        //    statistics.AverageExecutionTime = executionTimes.Any() ? Math.Round(executionTimes.Average(), 2) : 0;

        //    // Average Monthly Projects
        //    var monthlyProjects = await _context.ServiceRequests
        //                         .Where(sr => sr.CraftsmanId == craftsmanId && sr.CompletedDate.HasValue)
        //                         .GroupBy(sr => new { sr.CompletedDate.Value.Year, sr.CompletedDate.Value.Month })
        //                         .Select(g => g.Count())
        //                         .ToListAsync();
        //    statistics.AverageMonthlyProjects = monthlyProjects.Any() ? Math.Round(monthlyProjects.Average(), 2) : 0;


        //    return statistics;
        //}
        public async Task<DashboardStatistics> GetDashboardStatisticsAsync(string craftsmanId)
        {
            var statistics = new DashboardStatistics();

            // 🎯 Execute queries sequentially to avoid DbContext concurrency issues
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
                .CountAsync(sr => sr.CraftsmanId == craftsmanId);

            var requestsCompleted = await _context.ServiceRequests
                .CountAsync(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.Completed);

            var requestsUnderImplementation = await _context.ServiceRequests
                .CountAsync(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.UnderImplementation);

            var requestsRejected = await _context.ServiceRequests
                .CountAsync(sr => sr.CraftsmanId == craftsmanId && sr.Status == OrderStatus.Canceled);

            var responseTimes = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId && sr.AcceptedDate.HasValue)
                .Select(sr => EF.Functions.DateDiffHour(sr.RequestDate, sr.AcceptedDate.Value))
                .ToListAsync();

            var executionTimes = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId && sr.CompletedDate.HasValue && sr.AcceptedDate.HasValue)
                .Select(sr => EF.Functions.DateDiffDay(sr.AcceptedDate.Value, sr.CompletedDate.Value))
                .ToListAsync();

            var monthlyProjects = await _context.ServiceRequests
                .Where(sr => sr.CraftsmanId == craftsmanId && sr.CompletedDate.HasValue)
                .GroupBy(sr => new { sr.CompletedDate.Value.Year, sr.CompletedDate.Value.Month })
                .Select(g => g.Count())
                .ToListAsync();

            // 📊 Assign results
            statistics.AverageRating = Math.Round(averageRating, 2);
            statistics.AreasWorkedIn = areasWorkedIn;
            statistics.TotalIncomingRequests = totalIncomingRequests;
            statistics.ProjectsCompleted = requestsCompleted;
            statistics.RequestsCompleted = requestsCompleted;
            statistics.RequestsUnderImplementation = requestsUnderImplementation;
            statistics.RequestsRejected = requestsRejected;
            statistics.AverageResponseTime = responseTimes.Any() ? Math.Round(responseTimes.Average(), 2) : 0;
            statistics.AverageExecutionTime = executionTimes.Any() ? Math.Round(executionTimes.Average(), 2) : 0;
            statistics.AverageMonthlyProjects = monthlyProjects.Any() ? Math.Round(monthlyProjects.Average(), 2) : 0;

            return statistics;
        }




        public async Task<List<CraftsmanOrderListDto>> GetNewOrdersAsync(string craftsmanId)
        {
            return await _context.ServiceRequests
            .Where(r => r.CraftsmanId == craftsmanId && r.Status == Enums.OrderStatus.WaitingForAcceptance)
            .Select(r => new CraftsmanOrderListDto
            {
                RequestNumber = r.RequestNumber,
                ClientFullName = r.User.FirstName + " " + r.User.LastName,
                Location = r.Address,
                ServiceDescription = r.ServiceDescription,
                StartDate = r.StartDate
            })
            .ToListAsync();
        }

        public async Task<CraftsmanOrderDetailsDto> GetOrderDetailsAsync(string requestNumber)
        {
            var request = await _context.ServiceRequests
           .Include(r => r.User)
           .FirstOrDefaultAsync(r => r.RequestNumber == requestNumber);

            if (request == null) return null;

            return new CraftsmanOrderDetailsDto
            {
                ServiceDescription = request.ServiceDescription,
                Location = request.Address,
                StartDate = request.StartDate,
                PhoneNumber = request.PhoneNumber,
                SecondPhoneNumber = request.SecondPhoneNumber,
                MalfunctionPictures = request.MalfunctionImagePath?.Split(',').ToList() ?? new List<string>(),
                RequestStatus = request.Status.ToString()
            };
        }

        public async Task<bool> AcceptOrderAsync(string requestNumber)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.RequestNumber == requestNumber);
            if (request == null) return false;

            request.Status = Enums.OrderStatus.UnderImplementation;
            request.AcceptedDate = DateTime.UtcNow.Date;
            _context.ServiceRequests.Update(request);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectOrderAsync(string requestNumber)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.RequestNumber == requestNumber);
            if (request == null) return false;

            _context.ServiceRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<List<CompletedOrderDto>> GetCompletedOrdersAsync(string craftsmanId)
        {
            return await _context.ServiceRequests
                .Where(r => r.CraftsmanId == craftsmanId && r.Status == OrderStatus.Completed)
                .Select(r => new CompletedOrderDto
                {
                    RequestNumber = r.RequestNumber,
                    ClientFullName = r.User.FirstName + " " + r.User.LastName,
                    Location = r.Address,
                    ServiceDescription = r.ServiceDescription,
                    AcceptedDate = r.AcceptedDate,
                    CompletedDate = r.CompletedDate ?? DateTime.MinValue
                })
                .ToListAsync();
        }
        public async Task<List<UnderImplementationOrderDto>> GetUnderImplementationOrdersAsync(string craftsmanId)
        {
            return await _context.ServiceRequests
                .Where(r => r.CraftsmanId == craftsmanId && r.Status == OrderStatus.UnderImplementation)
                .Select(r => new UnderImplementationOrderDto
                {
                    RequestNumber = r.RequestNumber,
                    ClientFullName = r.User.FirstName + " " + r.User.LastName,
                    Location = r.Address,
                    ServiceDescription = r.ServiceDescription,
                    OrderDate = r.StartDate,
                    AcceptedDate = r.AcceptedDate ?? DateTime.MinValue
                })
                .ToListAsync();
        }
    }
}
