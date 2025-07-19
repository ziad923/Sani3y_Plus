using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.ServiceRequest;
using Sani3y_.Dtos.User;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IServiceRequestService
    {
        Task<List<CraftsmanOrderListDto>> GetNewOrdersAsync(string craftsmanId);
        Task<CraftsmanOrderDetailsDto> GetOrderDetailsAsync(string requestNumber);
        Task<bool> AcceptOrderAsync(string requestNumber);
        Task<bool> RejectOrderAsync(string requestNumber);
        Task<List<CompletedOrderDto>> GetCompletedOrdersAsync(string craftsmanId);
        Task<List<UnderImplementationOrderDto>> GetUnderImplementationOrdersAsync(string craftsmanId);
        Task<(bool Success, string? CraftsmanName, string? UserId)> AcceptOrderWithInfoAsync(string requestNumber);
        Task<ServiceRequestDetailsDto?> GetRequestByCodeAsync(string requestCode);
        Task<ServiceRequest?> GetRequestWithCraftsmanAsync(string requestNumber);
    }
}
