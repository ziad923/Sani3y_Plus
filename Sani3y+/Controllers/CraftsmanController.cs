using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sani3y_.Data;
using Sani3y_.Dtos.Account;
using Sani3y_.Dtos.Craftman;
using Sani3y_.Models;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CraftsmanController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CraftsmanController(UserManager<AppUser> userManager, IWebHostEnvironment webHostEnvironment)
        { 
            _userManager = userManager;
            _webHostEnvironment = webHostEnvironment;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromForm] CraftsmanRegisterDto craftsmanRegister)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);
                var existingUser = await _userManager.FindByEmailAsync(craftsmanRegister.Email);
                if (existingUser != null)
                    return BadRequest(new { Message = "Email already exists." });
                var cardImagePath = await SaveImage(craftsmanRegister.CardImage);
                var profileImagePath = await SaveImage(craftsmanRegister.ProfileImage);

                var appUser = new AppUser
                {
                    FirstName = craftsmanRegister.FirstName,
                    LastName = craftsmanRegister.LastName,
                    Governorate = craftsmanRegister.Governorate,
                    Location = craftsmanRegister.Location,
                    Profession = craftsmanRegister.Profession,
                    PhoneNumber = craftsmanRegister.PhoneNumber,
                    Email = craftsmanRegister.Email,
                    UserName = craftsmanRegister.Email, // Just to satisfy Identity's requirement
                    CardImagePath = cardImagePath,
                    ProfileImagePath = profileImagePath,
                    IsTrusted = false,
                    Role = "Craftsman"
                };

                var createUser = await _userManager.CreateAsync(appUser, craftsmanRegister.Password);
                if (createUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(appUser, "Craftsman");  // any want sign with email he is an user
                    if (roleResult.Succeeded)
                    {
                        return Ok("Craftman User Created Sucesfully");
                    }
                    else
                    {
                        return BadRequest(roleResult.Errors);
                    }
                }
                else
                {
                    return BadRequest(createUser.Errors);
                }

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex);
            }

        }
        private async Task<string> SaveImage(IFormFile image)
        {
            if (image == null || image.Length == 0)
                return null;


            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            return $"/images/{uniqueFileName}"; // Return relative URL
        }
    }
}