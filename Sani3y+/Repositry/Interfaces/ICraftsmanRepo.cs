using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.User;
using Sani3y_.Helpers;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface ICraftsmanRepo
    {
        Task<(List<CraftsmanFilterDto> Craftsmen, int TotalCount)> GetFilteredCraftsmenAsync(QuereyObject query); // move this 
        Task AddPreviousWorkAsync(string craftsmanId, PreviousWorkDto previousWorkDto);
        Task<List<CraftsmanOrderListDto>> GetNewOrdersAsync(string craftsmanId);
        Task<DashboardStatistics> GetDashboardStatisticsAsync(string craftsmanId);
        Task<CraftsmanOrderDetailsDto> GetOrderDetailsAsync(string requestNumber);
        Task<bool> AcceptOrderAsync(string requestNumber);
        Task<bool> RejectOrderAsync(string requestNumber);
        Task<List<CompletedOrderDto>> GetCompletedOrdersAsync(string craftsmanId);
        Task<List<UnderImplementationOrderDto>> GetUnderImplementationOrdersAsync(string craftsmanId);
        Task<List<RatingResponseDto>> GetRatingsForCraftsmanAsync(string craftsmanId);

        Task<List<PreviousWorkResponseDto>> GetPreviousWork(string craftsmanId);
        public Task<bool> UpdatePreviousWorkAsync(int id, PreviousWorkDto dto, string craftsmanId);
        public Task<bool> DeletePreviousWorkAsync(int id, string craftsmanId);

    }
}
