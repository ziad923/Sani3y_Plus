using Sani3y_.Data;
using Sani3y_.Dtos.User;
using Sani3y_.Repositry.Interfaces;
using Sani3y_.Services;

namespace Sani3y_.Repositry
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        public UserProfileRepository(AppDbContext appDbContext, IFileService fileService)
        {
            _context = appDbContext;
            _fileService = fileService;

        }
        public async Task<UserProfileDto> GetUserProfileAsync(string userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            return new UserProfileDto
            {
                ProfileImagePath = string.IsNullOrEmpty(user.ProfileImagePath)
                                    ? "/avatars/Rectangle 21.png"
                                    : user.ProfileImagePath,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };
        }

        public async Task<bool> UpdateUserProfileAsync(string userId, UpdateProfileDto updateProfileDto)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Update profile picture if a new one is uploaded
            if (updateProfileDto.ProfileImage != null)
            {
                // Delete old image unless it is the default avatar
                if (!string.IsNullOrEmpty(user.ProfileImagePath) && !user.ProfileImagePath.Contains("/avatars"))
                {
                    _fileService.DeleteFile(user.ProfileImagePath);
                }

                // Save new image
                user.ProfileImagePath = await _fileService.SaveFileAsync(updateProfileDto.ProfileImage);
            }

            // Update only the modified fields
            if (!string.IsNullOrEmpty(updateProfileDto.FirstName))
            {
                user.FirstName = updateProfileDto.FirstName;
            }

            if (!string.IsNullOrEmpty(updateProfileDto.LastName))
            {
                user.LastName = updateProfileDto.LastName;
            }

            if (!string.IsNullOrEmpty(updateProfileDto.Email))
            {
                user.Email = updateProfileDto.Email;
                user.UserName = updateProfileDto.Email; // Update UserName if it's tied to Email
            }

            if (!string.IsNullOrEmpty(updateProfileDto.PhoneNumber))
            {
                user.PhoneNumber = updateProfileDto.PhoneNumber;
            }

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
