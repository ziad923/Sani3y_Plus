using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IServiceRequestRepository
    {
        Task<ServiceRequestDetailsDto?> GetRequestByCodeAsync(string requestCode);
        Task<ServiceRequest?> GetRequestWithCraftsmanAsync(string requestNumber);
    }
}
