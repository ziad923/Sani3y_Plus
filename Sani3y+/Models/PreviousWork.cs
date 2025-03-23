namespace Sani3y_.Models
{
    public class PreviousWork
    {
        public int Id { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime DateJobDone { get; set; }
        public string CraftsmanId { get; set; }  // Foreign key to AppUser (Craftsman)
        public AppUser Craftsman { get; set; }   // Navigation property

        // To store picture URLs or file names (depending on where you store the pictures)
        public List<string> PictureUrls { get; set; } = new List<string>();
    }
}
