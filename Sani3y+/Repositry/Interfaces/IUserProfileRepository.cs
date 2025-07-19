using Sani3y_.Dtos.User;

namespace Sani3y_.Repositry.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfileDto> GetUserProfileAsync(string userId);
        Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDto updateProfileDto);
    }
}
