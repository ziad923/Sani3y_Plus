namespace Sani3y_.Helpers
{
    public class QuereyObject
    {
        public string? Profession { get; set; }
        public string? Location { get; set; }
        public double? MinRating { get; set; }  // Optional to filter by rating (1-5)
        public bool? IsTrusted { get; set; }  // Optional to filter by trusted statu
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 10;
    }
}
