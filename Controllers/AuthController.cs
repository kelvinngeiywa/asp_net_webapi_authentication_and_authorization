using Asp_net_auth.Models;
using Asp_net_auth.Models.DTOs;
using Asp_net_auth.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Asp_net_auth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController (IUser userServices) : ControllerBase
    {
        [Authorize(Roles ="Admin")]
        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            var users = await userServices.GetAllUsersAsync();
            if(users is null)
            {
                return NotFound("Users not found");
            }
            return Ok(users);

        }
        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(Guid id)
        {
            var user = await userServices.GetUserById(id);
            if (user is null)
            {
                return NotFound($"User with Id {id} not found!!!");
            }
            return Ok(user);
        }

        [HttpPost("register")]
        public async Task<ActionResult<string>> RegisterUserAsync(UserDTO registeredUser)
        {
            var user= await userServices.RegisteruserAsync(registeredUser);
            if(user is null)
            {
                return BadRequest("Username exists!!");
            }

            return Ok("User registered successfully!!!");
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginTokenResponseDTO>> LoginUserAsync(LoginUserDTO loginrequest)
        {
            var result = await userServices.LoginUserAsync(loginrequest);

            if(result is null)
            {
                return BadRequest("Invalid username or password");
            }

            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<LoginTokenResponseDTO>> RefreshToken(RefreshTokenRequestDTO refreshTokenRequest)
        {
            var result= await userServices.RefreshTokensAsync(refreshTokenRequest);
            if ( result is null || result.AccessToken is null || result.RefreshToken is null)
            {
                return Unauthorized("Invalid refresh token");
            }

            return Ok(result);
        }




        [Authorize]
        [HttpPut("{id}")]
        public async Task<ActionResult<string>> UpdateUserAsync(Guid id, UserDTO user) {
            var updatedUser = await userServices.UpdateUserAsync(id, user);
            if (user is null)
            {
                return BadRequest("User not found");
            }

            return Ok(updatedUser);
        }

        [Authorize(Roles ="Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<string>> DeleteUser(Guid id)
        {
            var user = await userServices.GetUserById(id);
            if(user is null)
            {
                return NotFound($"User with Id {id} not found");
            }
            var deletedUser = await userServices.DeleteUserAsync(user);
            if(deletedUser is null)
            {
                return BadRequest("Delete failed!!");
            }

            return Ok(deletedUser);
        }
    }
}
