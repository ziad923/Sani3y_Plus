using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Repositry
{
    public class ServiceRequestRepository: IServiceRequestRepository
    {
        private readonly AppDbContext _context;

        public ServiceRequestRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceRequestDetailsDto?> GetRequestByCodeAsync(string requestCode)
        {
            return await _context.ServiceRequests
                      .Where(r => r.RequestNumber == requestCode)
                      .Select(r => new ServiceRequestDetailsDto
                      {
                          Id = r.Id,
                          RequestNumber = r.RequestNumber,
                          ServiceDescription = r.ServiceDescription,
                          Address = r.Address,
                          StartDate = r.StartDate,
                          PhoneNumber = r.PhoneNumber,
                          SecondPhoneNumber = r.SecondPhoneNumber,
                          MalfunctionImagePath = r.MalfunctionImagePath,
                          RequestDate = r.RequestDate,
                          Status = r.Status.ToString()
                      })
          .FirstOrDefaultAsync();
        }

        public async Task<ServiceRequest?> GetRequestWithCraftsmanAsync(string requestNumber)
        {
            return await _context.ServiceRequests
                .Include(r => r.Craftsman)
                .FirstOrDefaultAsync(r => r.RequestNumber == requestNumber);
        }
    }
}