using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Account;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Helpers;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CraftsmanAuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IFileService _fileService;
        private readonly AvatarService _avatarService;

        public CraftsmanAuthController(UserManager<AppUser> userManager,
                                        ITokenService tokenService,
                                        IFileService fileService,
                                        AvatarService avatarService)
        { 
            _userManager = userManager;
            _tokenService = tokenService;
            _fileService = fileService;
            _avatarService = avatarService;
        }
        [HttpPost("signup")]
        [AllowAnonymous]
        public async Task<IActionResult> SignUp([FromForm] CraftsmanRegisterDto craftsmanRegister)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existingUser = await _userManager.FindByEmailAsync(craftsmanRegister.Email);
                if (existingUser != null)
                    return BadRequest(new { Message = "Email already exists." });

                // Save card image if provided
                var cardImagePath = craftsmanRegister.CardImage != null
                    ? await _fileService.SavePictureAsync(craftsmanRegister.CardImage)
                    : null;

                // Save profile image if provided, else get random avatar
                var profileImagePath = craftsmanRegister.ProfileImage != null
                    ? await _fileService.SavePictureAsync(craftsmanRegister.ProfileImage)
                    : _avatarService.GetRandomAvatarPath();

                var appUser = new AppUser
                {
                    FirstName = craftsmanRegister.FirstName,
                    LastName = craftsmanRegister.LastName,
                    Governorate = craftsmanRegister.Governorate,
                    Location = craftsmanRegister.Location,
                    ProfessionId = craftsmanRegister.ProfessionId,
                    PhoneNumber = craftsmanRegister.PhoneNumber,
                    Email = craftsmanRegister.Email,
                    UserName = craftsmanRegister.Email,
                    CardImagePath = cardImagePath,
                    ProfileImagePath = profileImagePath,
                    IsTrusted = false,
                    Role = "Craftsman"
                };

                var createUser = await _userManager.CreateAsync(appUser, craftsmanRegister.Password);
                if (createUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "Craftsman");
                    if (!roleResult.Succeeded)
                        return BadRequest(roleResult.Errors);

                   
                    var accessToken = _tokenService.GenerateJwtToken(appUser);
                    var refreshToken = _tokenService.GenerateRefreshToken();

                   
                    appUser.RefreshToken = refreshToken;
                    appUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                    await _userManager.UpdateAsync(appUser);

                    return Ok(new
                    {
                        Message = "Craftsman User Created Successfully",
                        AccessToken = accessToken,
                        RefreshToken = refreshToken,
                        Role = appUser.Role
                    });
                }
                else
                {
                    return BadRequest(createUser.Errors);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
            }
        }


        //[HttpPost("signup")]
        //[AllowAnonymous]
        //public async Task<IActionResult> SignUp([FromForm] CraftsmanRegisterDto craftsmanRegister)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        var existingUser = await _userManager.FindByEmailAsync(craftsmanRegister.Email);
        //        if (existingUser != null)
        //            return BadRequest(new { Message = "Email already exists." });

        //        // Save card image if provided
        //        var cardImagePath = craftsmanRegister.CardImage != null
        //            ? await _fileService.SavePictureAsync(craftsmanRegister.CardImage)
        //            : null;

        //        // Save profile image if provided, else get random avatar
        //        var profileImagePath = craftsmanRegister.ProfileImage != null
        //            ? await _fileService.SavePictureAsync(craftsmanRegister.ProfileImage)
        //            : _avatarService.GetRandomAvatarPath();

        //        var appUser = new AppUser
        //        {
        //            FirstName = craftsmanRegister.FirstName,
        //            LastName = craftsmanRegister.LastName,
        //            Governorate = craftsmanRegister.Governorate,
        //            Location = craftsmanRegister.Location,
        //            ProfessionId = craftsmanRegister.ProfessionId,
        //            PhoneNumber = craftsmanRegister.PhoneNumber,
        //            Email = craftsmanRegister.Email,
        //            UserName = craftsmanRegister.Email, 
        //            CardImagePath = cardImagePath,  
        //            ProfileImagePath = profileImagePath, 
        //            IsTrusted = false,
        //            Role = "Craftsman"
        //        };

        //        var createUser = await _userManager.CreateAsync(appUser, craftsmanRegister.Password);
        //        if (createUser.Succeeded)
        //        {
        //            var roleResult = await _userManager.AddToRoleAsync(appUser, "Craftsman");
        //            var token = _tokenService.GenerateJwtToken(appUser);
        //            if (roleResult.Succeeded)
        //            {
        //                return Ok(new
        //                {
        //                    Message = "Craftsman User Created Successfully",
        //                    Token = token  
        //                });
        //            }
        //            else
        //            {
        //                return BadRequest(roleResult.Errors);
        //            }
        //        }
        //        else
        //        {
        //            return BadRequest(createUser.Errors);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "An error occurred", Error = ex.Message });
        //    }
        //}

    }
}