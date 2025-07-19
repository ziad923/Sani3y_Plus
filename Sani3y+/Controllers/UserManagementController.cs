using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sani3y_.Repositry.Interfaces;

namespace Sani3y_.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class UserManagementController : ControllerBase
    {
        private readonly IUserRepo _userRepo;

        public UserManagementController(IUserRepo userRepository)
        {
            _userRepo = userRepository;
        }
        [HttpGet("all-users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userRepo.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpGet("all-craftsmen")]
        public async Task<IActionResult> GetAllCraftsmen()
        {
            var craftsmen = await _userRepo.GetAllCraftsmenAsync();
            return Ok(craftsmen);
        }

        [HttpDelete("delete-user/{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var success = await _userRepo.DeleteUserAsync(id);
            return success ? Ok(new { message = "User deleted successfully." }) : NotFound(new { message = "User not found." });
        }
    }
}
