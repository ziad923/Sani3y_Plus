using Google.Apis.Auth;
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

        public UserAuthController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IConfiguration configuration, 
            EmailService emailService,
            ITokenService tokenService)
                                        
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        // Normal Sign-Up (Email/Password)
        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] RegisterDto registerDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Check if the user already exists
                var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
                if (existingUser != null)
                    return BadRequest(new { Message = "Email already exists." });

                // Create a new user
                var appUser = new AppUser
                {
                    FirstName = registerDto.FirstName,
                    LastName = registerDto.LastName,
                    Email = registerDto.Email,
                    UserName = registerDto.Email, // Username is required by Identity
                    PhoneNumber = registerDto.PhoneNumber,
                    Role = "User" // Assign a default role
                };
                    // its me and her in the morning ya he is big and dumb as a man can come but stronger tha
                // Create the user in the database
                var createUserResult = await _userManager.CreateAsync(appUser, registerDto.Password);
                if (!createUserResult.Succeeded)
                    return BadRequest(createUserResult.Errors);

                // Assign the "User" role to the new user
                var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                if (!roleResult.Succeeded)
                    return BadRequest(roleResult.Errors);

                // Generate a JWT token for the new user
                var token = _tokenService.GenerateJwtToken(appUser);
                return Ok(new { Token = token, Message = "User registered successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "An error occurred.", Error = ex.Message });
            }
        }

        [HttpPost("google-signin")]
        public async Task<IActionResult> GoogleSignIn([FromBody] GoogleLoginRequest request)
        {
            try
            {
                // Validate the Google ID Token
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                });

                if (payload == null)
                    return BadRequest("Invalid Google Token");

                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(payload.Email);
                if (existingUser == null)
                {
                    // Create a new user
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

                // Generate JWT token for the user
                var token = _tokenService.GenerateJwtToken(existingUser);

                return Ok(new { Token = token, Message = "User logged in successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = "Google authentication failed.", Error = ex.Message });
            }
        }
        // Normla Log in
        [HttpPost("signin-Normal")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
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
            var token = _tokenService.GenerateJwtToken(user);

            return Ok(new
            {
                Token = token,
                Role = roles,
                Message = "Login successful"
            });
        }
        // Forgot Password Endpoint
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto forgotPasswordDto)
        {
            try
            {
                if(!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _userManager.FindByEmailAsync(forgotPasswordDto.Email);
                if (user == null)
                    return BadRequest(new { Message = "User not found." });

                // Generate a password reset token (this is the part we would use later for actual password reset)
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var encodedToken = Uri.EscapeDataString(token);
                var encodedEmail = Uri.EscapeDataString(forgotPasswordDto.Email);
                // Generate a password reset URL (this could be a frontend URL for users to reset the password)
                var resetUrl = $"{_configuration["App:FrontendUrl"]}/reset-password?token={encodedToken}&email={encodedEmail}";
                // Send the reset URL to the user's email
                var subject = "Password Reset Request";
                var body = $"<p>To reset your password, please click the following link:</p><a href='{resetUrl}'>Reset Password</a>";
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