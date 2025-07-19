using System.ComponentModel.DataAnnotations;

namespace Sani3y_.Dtos.Craftman
{
	public class ProfessionCreateOrUpdateDto
	{
		[Required]
		public string Name { get; set; }

		public IFormFile?  ImagePath { get; set; } 
	}
}
