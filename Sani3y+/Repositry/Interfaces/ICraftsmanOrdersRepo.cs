using Sani3y_.Dtos.Craftman;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface ICraftsmanOrdersRepo
    {
        Task<List<CraftsmanOrderListDto>> GetNewOrdersAsync(string craftsmanId);
        Task<CraftsmanOrderDetailsDto?> GetOrderDetailsAsync(string requestNumber);
        // Task<bool> UpdateOrderAsync(ServiceRequest request);
        Task<bool> AcceptOrderAsync(string requestNumber);
        Task<bool> RejectOrderAsync(string requestNumber);
        Task<List<CompletedOrderDto>> GetCompletedOrdersAsync(string craftsmanId);
        Task<List<UnderImplementationOrderDto>> GetUnderImplementationOrdersAsync(string craftsmanId);
        Task<ServiceRequest?> GetOrderWithCraftsmanAndUserAsync(string requestNumber);
    }
}
