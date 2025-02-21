using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Selu383.SP25.P02.Api.Data;
using Selu383.SP25.P02.Api.Features.Theaters.Roles;

namespace Selu383.SP25.P02.Api.Features.Users
{
    [Route("api/users")]
    [ApiController]
    [Authorize(Roles = "Admin")]  // Admin Controller
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<Role> roleManager;

        public UsersController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> Create([FromBody] CreateUserDto dto)
        {
            // Validate username
            if (string.IsNullOrEmpty(dto.UserName))
            {
                return BadRequest("Username is required");
            }

            // Validate password
            if (string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest("Password is required");
            }

            // Validate roles
            if (dto.Roles == null || dto.Roles.Length == 0)
            {
                return BadRequest("At least one role is required");
            }

            // Validate each role exists
            foreach (var roleName in dto.Roles)
            {
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    return BadRequest($"Role '{roleName}' does not exist");
                }
            }

            // Create user
            var user = new User
            {
                UserName = dto.UserName
            };

            // Try to create the user
            var result = await userManager.CreateAsync(user, dto.Password);
            if (!result.Succeeded)
            {
                // Check for duplicate username
                if (result.Errors.Any(e => e.Code == "DuplicateUserName"))
                {
                    return BadRequest("Username already exists");
                }

                // Handle other errors
                return BadRequest(result.Errors.First().Description);
            }

            // Assign roles
            foreach (var role in dto.Roles)
            {
                await userManager.AddToRoleAsync(user, role);
            }

            // Return the created user
            return Ok(new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Roles = dto.Roles
            });
        }
    }
}