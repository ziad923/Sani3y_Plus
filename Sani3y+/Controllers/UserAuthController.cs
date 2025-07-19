using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using Sani3y_.Dtos.Account;
using Sani3y_.Helpers;
using Sani3y_.Models;
using Sani3y_.Repositry.Interfaces;
using Sani3y_.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserAuthController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;
        private readonly IEmailTemplateService _emailTemplateService;
        private readonly AvatarService _avatarService;

        public UserAuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration,
            IEmailService emailService,
            ITokenService tokenService,
            IEmailTemplateService emailTemplateService,
            AvatarService avatarService)
                                        
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _tokenService = tokenService;
            _emailTemplateService = emailTemplateService;
            _avatarService = avatarService;
        }
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                    return BadRequest(new { Message = "Email already exists." });

               
                var appUser = new AppUser
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    UserName = registerDto.Email,
                    PhoneNumber = registerDto.PhoneNumber,
                    ProfileImagePath = _avatarService.GetRandomAvatarPath(),
                    Role = "User"
                };

                var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (!createUserResult.Succeeded)
                    return BadRequest(createUserResult.Errors);

                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                if (!roleResult.Succeeded)
                    return BadRequest(roleResult.Errors);

               
                var token = _tokenService.GenerateJwtToken(appUser);
                var refreshToken = _tokenService.GenerateRefreshToken();

                appUser.RefreshToken = refreshToken;
                appUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); // 7 days validity
                await _userManager.UpdateAsync(appUser);

                return Ok(new TokenResponseDto
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    Role = appUser.Role
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }

        // Normal Sign-Up (Email/Password)
        //[HttpPost("signup")]
        //public async Task<IActionResult> SignUp([FromBody] RegisterDto registerDto)
        //{
        //    try
        //    {
        //        if (!ModelState.IsValid)
        //            return BadRequest(ModelState);

        //        // Check if the user already exists
        //        var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
        //        if (existingUser != null)
        //            return BadRequest(new { Message = "Email already exists." });

        //        // Create a new user
        //        var appUser = new AppUser
        //        {
        //            FirstName = registerDto.FirstName,
        //            LastName = registerDto.LastName,
        //            Email = registerDto.Email,
        //            UserName = registerDto.Email,
        //            PhoneNumber = registerDto.PhoneNumber,
        //            ProfileImagePath = _avatarService.GetRandomAvatarPath(),
        //            Role = "User" 
        //        };

        //        var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
        //        if (!createUserResult.Succeeded)
        //            return BadRequest(createUserResult.Errors);

        //        var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
        //        if (!roleResult.Succeeded)
        //            return BadRequest(roleResult.Errors);

        //        var token = _tokenService.GenerateJwtToken(appUser);
        //        return Ok(new { Token = token, Message = "User registered successfully." });
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
        //    }
        //}

        [HttpPost("google-signin")]
        public async Task<IActionResult> SignInWithGoogle([FromBody] GoogleLoginRequest request)
        {
            try
            {
                
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                });

                if (payload == null)
                    return BadRequest("Invalid Google Token");

              
                var existingUser = await _userManager.FindByEmailAsync(payload.Email);
                if (existingUser == null)
                {
                   
                    var newUser = new AppUser
                    {
                        UserName = payload.Email,
                        Email = payload.Email,
                        FirstName = payload.GivenName,
                        LastName = payload.FamilyName,
                        Role = "User"
                    };

                    var createUserResult = await _userManager.CreateAsync(newUser);
                    if (!createUserResult.Succeeded)
                        return BadRequest(createUserResult.Errors);

                    existingUser = newUser;
                }
   
                var token = _tokenService.GenerateJwtToken(existingUser);
                var refreshToken = _tokenService.GenerateRefreshToken();

                existingUser.RefreshToken = refreshToken;
                existingUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
                await _userManager.UpdateAsync(existingUser);

                return Ok(new TokenResponseDto
                {
                    AccessToken = token,
                    RefreshToken = refreshToken,
                    Role = existingUser.Role
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Google authentication failed.", Error = ex.Message });
            }
        }
        [HttpPost("signin-normal")]
        public async Task<IActionResult> SignInNormal([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrWhiteSpace(loginDto.EmailOrPhone) || string.IsNullOrWhiteSpace(loginDto.Password))
                return BadRequest(new { Message = "Email/Phone and Password are required" });

            // Find user by Email or Phone Number
            var user = await _userManager.Users
                .FirstOrDefaultAsync(u => u.Email == loginDto.EmailOrPhone || u.PhoneNumber == loginDto.EmailOrPhone);

            if (user == null)
                return Unauthorized(new { Message = "Invalid credentials" });

            // Verify password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);
            if (!result.Succeeded)
                return Unauthorized(new { Message = "Invalid credentials" });

            // Generate JWT Token
            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault() ?? "User";
            var token = _tokenService.GenerateJwtToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return Ok(new TokenResponseDto
            {
                AccessToken = token,
                RefreshToken = refreshToken,
                Role = roles.FirstOrDefault() ?? "User"
            });
        }
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RequestRefreshToken request)
        {
            try
            {
                var tokens = await _tokenService.RefreshTokenAsync(request.AccessToken, request.RefreshToken);
                return Ok(tokens);
            }
            catch (SecurityTokenException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid user");

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return NotFound("User not found");

                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddSeconds(-1); // Force immediate expiry
                await _userManager.UpdateAsync(user);

                return Ok(new { Message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred during logout");
            }
        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                    return BadRequest(new { Message = "If the email exists, a reset link has been sent." });

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var encodedToken = Uri.EscapeDataString(token);
                var encodedEmail = Uri.EscapeDataString(forgotPasswordDto.Email);
                // Generate a password reset URL (this could be a frontend URL for users to reset the password)
                var resetUrl = $"{_configuration["App:FrontendUrl"]}/reset-password?token={encodedToken}&email={encodedEmail}";

                var (subject, body) = _emailTemplateService.GetResetPasswordEmail(resetUrl);
                await _emailService.SendEmailAsync(forgotPasswordDto.Email, subject, body);

                return Ok(new { Message = "Password reset email sent." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
            
        }
        // Reset Password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(resetPasswordDto.Email);
                if (user == null)
                    return BadRequest(new { Message = "User not found." });

                if (resetPasswordDto.NewPassword != resetPasswordDto.ConfirmPassword)
                    return BadRequest(new { Message = "Passwords do not match." });

                // Reset the password
                var result = await _userManager.ResetPasswordAsync(user, resetPasswordDto.Token, resetPasswordDto.NewPassword);
                if (!result.Succeeded)
                    return BadRequest(new { Message = "Failed to reset password.", Errors = result.Errors });

                return Ok(new { Message = "Password reset successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return Unauthorized(new { Message = "User not found." });

                if (changePasswordDto.NewPassword != changePasswordDto.ConfirmPassword)
                    return BadRequest(new { Message = "Passwords do not match." });

                var result = await _userManager.ChangePasswordAsync(user, changePasswordDto.CurrentPassword, changePasswordDto.NewPassword);
                if (!result.Succeeded)
                    return BadRequest(new { Message = "Failed to change password.", Errors = result.Errors });

                return Ok(new { Message = "Password changed successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }
    }
}