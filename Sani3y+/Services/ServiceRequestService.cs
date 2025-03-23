using Sani3y_.Dtos.Craftman;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Services
{
    public interface IServiceRequestService // check this again later 
    {
        Task<List<CraftsmanOrderListDto>> GetNewOrdersAsync(string craftsmanId);
        Task<CraftsmanOrderDetailsDto> GetOrderDetailsAsync(string requestNumber);
        Task<bool> AcceptOrderAsync(string requestNumber);
        Task<bool> RejectOrderAsync(string requestNumber);
        Task<List<CompletedOrderDto>> GetCompletedOrdersAsync(string craftsmanId);
        Task<List<UnderImplementationOrderDto>> GetUnderImplementationOrdersAsync(string craftsmanId);
    }

    public class ServiceRequestService : IServiceRequestService
    {
        private readonly ICraftsmanRepo _repository;

        public ServiceRequestService(ICraftsmanRepo repository)
        {
            _repository = repository;
        }

        public Task<List<CraftsmanOrderListDto>> GetNewOrdersAsync(string craftsmanId) => _repository.GetNewOrdersAsync(craftsmanId);

        public Task<CraftsmanOrderDetailsDto> GetOrderDetailsAsync(string requestNumber) => _repository.GetOrderDetailsAsync(requestNumber);

        public Task<bool> AcceptOrderAsync(string requestNumber) => _repository.AcceptOrderAsync(requestNumber);

        public Task<bool> RejectOrderAsync(string requestNumber) => _repository.RejectOrderAsync(requestNumber);
        public Task<List<CompletedOrderDto>> GetCompletedOrdersAsync(string craftsmanId) =>
     _repository.GetCompletedOrdersAsync(craftsmanId);
        public Task<List<UnderImplementationOrderDto>> GetUnderImplementationOrdersAsync(string craftsmanId)
       => _repository.GetUnderImplementationOrdersAsync(craftsmanId);
    }
}
