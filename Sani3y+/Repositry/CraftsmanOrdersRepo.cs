using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Enums;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class CraftsmanOrdersRepo : ICraftsmanOrdersRepo
    {
        private readonly AppDbContext _context;

        public CraftsmanOrdersRepo(AppDbContext appDbContext)
        {
            _context = appDbContext;
        }
        public async Task<List<CraftsmanOrderListDto>> GetNewOrdersAsync(string craftsmanId)
        {
            return await _context.ServiceRequests
                .AsNoTracking()
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
        public async Task<CraftsmanOrderDetailsDto?> GetOrderDetailsAsync(string requestNumber)
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
        public async Task<bool> AcceptOrderAsync(string requestNumber)
        {
            var request = await _context.ServiceRequests.FirstOrDefaultAsync(r => r.RequestNumber == requestNumber);
            if (request == null) return false;

            request.Status = Enums.OrderStatus.UnderImplementation;
            request.AcceptedDate = DateTime.UtcNow;
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

        public async Task<ServiceRequest?> GetOrderWithCraftsmanAndUserAsync(string requestNumber)
        {
            return await _context.ServiceRequests
                  .Include(r => r.Craftsman)
                  .Include(r => r.User)
                  .FirstOrDefaultAsync(r => r.RequestNumber == requestNumber);
        }
    }
}
