using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.Craftman
{
    public class PreviousWorkDto
    {
        [Required]
        public string ProjectDescription { get; set; }
        [Required]
        public DateTime DateJobDone { get; set; }
        public List<IFormFile> Pictures { get; set; }  // List of uploaded pictures (to be stored on the server)
    }
}
