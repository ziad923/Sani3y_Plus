namespace Sani3y_.Dtos.Craftman
{
    public class CraftsmanFilterDto
    {
        public string? Profession { get; set; }
        public string? Location { get; set; }
        public double? AverageRating { get; set; } // Minimum rating filter
        public bool? IdentityVerified { get; set; }
    }
}
