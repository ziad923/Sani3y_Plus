using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Models
{
    public class Profession
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string? ImagePath { get; set; }
        public List<AppUser> Craftsmen { get; set; }
    }
}
