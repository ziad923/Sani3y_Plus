using Sani3y_.Dtos.Home;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IHomeService
    {
        Task<HomePageSummaryDto> GetHomePageSummaryAsync();
        Task<SystemStatsDto> GetSystemStatsAsync();
    }
}
