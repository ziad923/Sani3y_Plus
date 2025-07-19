namespace Sani3y_.Helpers
{
    public class AvatarService
    {
        private readonly string[] _defaultAvatars = new[]
    {
        "/avatars/Rectangle 21.png",
        "/avatars/Rectangle 22.png",
        "/avatars/Rectangle 23.png",
        "/avatars/Rectangle 24.png",
        "/avatars/Rectangle 25.png",
        "/avatars/Rectangle 26.png",
        "/avatars/Rectangle 27.png",
        "/avatars/Rectangle 28.png",
        "/avatars/Rectangle 29.png",
        "/avatars/Rectangle 30.png",
        "/avatars/Rectangle 31.png",
        "/avatars/Rectangle 32.png",
        "/avatars/Rectangle 33.png",
        "/avatars/Rectangle 34.png",
        "/avatars/Rectangle 35.png",
        "/avatars/Rectangle 36.png",
        "/avatars/Rectangle 37.png",
        "/avatars/Rectangle 38.png",
        "/avatars/Rectangle 39.png",
        "/avatars/Rectangle 40.png",
        "/avatars/Rectangle 41.png",
        "/avatars/Rectangle 42.png"
    };

        public string GetRandomAvatarPath()
        {
            var rnd = new Random();
            int index = rnd.Next(_defaultAvatars.Length);
            return _defaultAvatars[index]; 
        }
    }
}
