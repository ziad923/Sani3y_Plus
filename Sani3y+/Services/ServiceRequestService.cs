using Microsoft.EntityFrameworkCore;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Models;
using Sani3y_.Repositry;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{

    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ICraftsmanQueryRepo _craftsmanRepo;
        private readonly ICraftsmanOrdersRepo _craftsmanOrdersRepo;
        private readonly IServiceRequestRepository _serviceRequestRepository;
        public ServiceRequestService(ICraftsmanQueryRepo repository,
                                    ICraftsmanOrdersRepo craftsmanOrdersRepo,
                                    IServiceRequestRepository serviceRequestRepository)
        {
            _craftsmanRepo = repository;
           _craftsmanOrdersRepo = craftsmanOrdersRepo;
            _serviceRequestRepository = serviceRequestRepository;
        }

        public Task<List<CraftsmanOrderListDto>> GetNewOrdersAsync(string craftsmanId) => _craftsmanOrdersRepo.GetNewOrdersAsync(craftsmanId);

        public Task<CraftsmanOrderDetailsDto> GetOrderDetailsAsync(string requestNumber) => _craftsmanOrdersRepo.GetOrderDetailsAsync(requestNumber);

        public Task<bool> AcceptOrderAsync(string requestNumber) => _craftsmanOrdersRepo.AcceptOrderAsync(requestNumber);

        public Task<bool> RejectOrderAsync(string requestNumber) => _craftsmanOrdersRepo.RejectOrderAsync(requestNumber);
        public Task<List<CompletedOrderDto>> GetCompletedOrdersAsync(string craftsmanId) =>
     _craftsmanOrdersRepo.GetCompletedOrdersAsync(craftsmanId);
        public Task<List<UnderImplementationOrderDto>> GetUnderImplementationOrdersAsync(string craftsmanId)
       => _craftsmanOrdersRepo.GetUnderImplementationOrdersAsync(craftsmanId);

        public async Task<(bool Success, string? CraftsmanName, string? UserId)> AcceptOrderWithInfoAsync(string requestNumber)
        {
            var request = await _craftsmanOrdersRepo.GetOrderWithCraftsmanAndUserAsync(requestNumber);
            if (request == null)
                return (false, null, null);

            var success = await _craftsmanOrdersRepo.AcceptOrderAsync(requestNumber);
            if (!success)
                return (false, null, null);

            var craftsmanName = $"{request.Craftsman.FirstName} {request.Craftsman.LastName}";
            return (true, craftsmanName, request.UserId);
        }

        public async Task<ServiceRequestDetailsDto?> GetRequestByCodeAsync(string requestCode)
        {
            return await _serviceRequestRepository.GetRequestByCodeAsync(requestCode);
        }

        public async Task<ServiceRequest?> GetRequestWithCraftsmanAsync(string requestNumber)
        {
            return await _serviceRequestRepository.GetRequestWithCraftsmanAsync(requestNumber);
        }
    }
}
