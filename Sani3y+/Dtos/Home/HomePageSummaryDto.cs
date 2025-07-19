using Sani3y_.Dtos.User;

namespace Sani3y_.Dtos.Home
{
    public class HomePageSummaryDto
    {
        public string ProfilePicture {  get; set; }
        public string UserFullName { get; set; }
        public List<TopProfessionDto> TopProfessions { get; set; }
        public List<TopCraftsmanDto> TopCraftsmen { get; set; }
        public List<HomeUserRatingDto> RecentRatings { get; set; }
        public int TotalOrders { get; set; }
        public int TotalCraftsmen { get; set; }
       // public int TotalWebsiteVisitors { get; set; }
    }
}
