using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface ITokenService
    {
        public string GenerateJwtToken(AppUser user);
    }
}
