using Asp_net_auth.Data;
using Asp_net_auth.Models;
using Asp_net_auth.Models.DTOs;
using Asp_net_auth.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Asp_net_auth.Services
{
    public class UserServices(AppDbContext dbContext, IConfiguration configuration) : IUser
    {
        public async Task<string> DeleteUserAsync(User user)
        {
            dbContext.user.Remove(user);
            await dbContext.SaveChangesAsync();
            return "User deleted successfully!!!";
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await dbContext.user.ToListAsync();
        }

        public async Task<User> GetUserById(Guid userId)
        {
            var user= await dbContext.user.FindAsync(userId);
            if(user is null)
            {
                return null;
            }
            return user;
        }

        public async Task<LoginTokenResponseDTO> LoginUserAsync(LoginUserDTO loginuser)
        {
            var user = await dbContext.user.FirstOrDefaultAsync(user => user.Username == loginuser.Username);
            if (user is null)
            {
                return null;
            }

            var confirmPassword = new PasswordHasher<User>().VerifyHashedPassword(user, user.Password, loginuser.Password);

            if (confirmPassword == PasswordVerificationResult.Failed)
            {
                return null;
            }

            //var token =CreateToken(user);
            //LoginTokenResponseDTO response = await LoginTokenresponse(user);
            return await LoginTokenresponse(user);
        }

        private async Task<LoginTokenResponseDTO> LoginTokenresponse(User user)
        {
            return new LoginTokenResponseDTO
            {
                AccessToken = CreateToken(user),
                RefreshToken = await GenerateAndSaveRefreshTokenAsync(user)
            };
        }

        public async Task<LoginTokenResponseDTO> RefreshTokensAsync(RefreshTokenRequestDTO tokenRequestDTO)
        {
            var user = await ValidateRefreshTokenAsync(tokenRequestDTO.Id, tokenRequestDTO.RefreshToken);
            if (user is null)
            {
                return null;
            }

            return await LoginTokenresponse(user);
        }

        public async Task<string> RegisteruserAsync(UserDTO newUserDTO)
        {
            if(await dbContext.user.AnyAsync(user=>user.Username == newUserDTO.Username))
            {
                return null;
            }
            var newUser = new User();
            var hashedPassword = new PasswordHasher<User>().HashPassword(newUser, newUserDTO.Password);

            newUser.Username = newUserDTO.Username;
            newUser.Password = hashedPassword;
            newUser.Role= newUserDTO.Role;

            dbContext.user.Add(newUser);
            await dbContext.SaveChangesAsync();
            return "";
        }

        public async Task<string> UpdateUserAsync(Guid userId,UserDTO userDTO)
        {
            var user= await dbContext.user.FindAsync(userId);
            if (user is null)
            {
                return null;
            }
            var updatedhashedPassword= new PasswordHasher<User>().HashPassword(user, userDTO.Password);
            user.Username=userDTO.Username;
            user.Password=updatedhashedPassword;
            user.Role=userDTO.Role;

            

            await dbContext.SaveChangesAsync();
            return "User updated successfully!!";
            
        }



        private async Task<User?> ValidateRefreshTokenAsync(Guid userId, string refreshToken)
        {
            var user = await dbContext.user.FindAsync(userId);
            if(user is null || user.RefreshToken != refreshToken || user.RefreshTokenLifeTime <= DateTime.UtcNow)
            {
                return null;
            }
            return user;
        }

        private string GenerateToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task<string> GenerateAndSaveRefreshTokenAsync(User user)
        {
            var refreshToken = GenerateToken();
            user.RefreshToken = refreshToken;
            user.RefreshTokenLifeTime = DateTime.UtcNow.AddDays(5);
            await dbContext.SaveChangesAsync();
            return refreshToken;
        }

        private string CreateToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name,user.Username),
                new Claim(ClaimTypes.NameIdentifier,user.Id.ToString()),
                new Claim(ClaimTypes.Role,user.Role)
            };

            //SymmetricSecurityKey needs a package called System.IdentityModel.Tokens.jwt
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetValue<string>("Jwt:Token")!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

            var tokenDescriptor = new JwtSecurityToken
                (
                   issuer: configuration.GetValue<string>("Jwt:Issuer"),
                   audience:configuration.GetValue<string>("jwt:Audience"),
                   claims:claims,
                   expires:DateTime.UtcNow.AddHours(5),
                   signingCredentials:creds

                );

            return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        }


    }
}
