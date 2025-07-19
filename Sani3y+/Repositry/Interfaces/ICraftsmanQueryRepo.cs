using Sani3y_.Dtos.Craftman;
using Sani3y_.Dtos.User;
using Sani3y_.Helpers;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface ICraftsmanQueryRepo
    {
        Task<(List<CraftsmanFilterDto> Craftsmen, int TotalCount)> GetFilteredCraftsmenAsync(QuereyObject query); // move this 
        Task<DashboardStatistics> GetDashboardStatisticsAsync(string craftsmanId);
      
        Task<List<RatingResponseDto>> GetRatingsForCraftsmanAsync(string craftsmanId);


    }
}
