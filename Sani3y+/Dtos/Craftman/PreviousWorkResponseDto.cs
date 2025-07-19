namespace Sani3y_.Dtos.Craftman
{
    public class PreviousWorkResponseDto
    {
        public int Id { get; set; }
        public string ProjectDescription { get; set; }
        public DateTime DateJobDone { get; set; }
        public List<string> PictureUrls { get; set; }
    }
}
