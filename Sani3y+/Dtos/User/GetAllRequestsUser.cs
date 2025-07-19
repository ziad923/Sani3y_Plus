using Sani3y_.Enums;

namespace Sani3y_.Dtos.User
{
    public class GetAllRequestsUser
    {
        public int Id { get; set; }
        public string RequestNumber { get; set; }
        public string CraftsmanProfession { get; set; }
        public string CraftsmanFullName { get; set; }
        public string ServiceDescription { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus OrderStatus { get; set; }
    }
}
