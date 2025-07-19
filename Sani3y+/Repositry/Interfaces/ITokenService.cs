using Sani3y_.Dtos.Account;
using Sani3y_.Models;

namespace Sani3y_.Repositry.Interfaces
{
    public interface ITokenService
    {
         string GenerateJwtToken(AppUser user);
        string GenerateRefreshToken();
        Task<TokenResponseDto> RefreshTokenAsync(string accessToken, string refreshToken);
    }
}
