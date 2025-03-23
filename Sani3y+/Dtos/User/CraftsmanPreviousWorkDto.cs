namespace Sani3y_.Dtos.User
{
    public class CraftsmanPreviousWorkDto
    {
        public string ProjectDescription { get; set; }
        public DateTime DateProjectDone { get; set; }
        public List<string> ProjectPictures { get; set; }  = new List<string>();
    }
}
