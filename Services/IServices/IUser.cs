using Asp_net_auth.Models;
using Asp_net_auth.Models.DTOs;

namespace Asp_net_auth.Services.IServices
{
    public interface IUser
    {
        Task<string> RegisteruserAsync(UserDTO newUserDTO);
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserById(Guid userId);
        Task<LoginTokenResponseDTO> LoginUserAsync(LoginUserDTO loginuser);

        Task<string> UpdateUserAsync(Guid userId,UserDTO userDTO);
        Task<string> DeleteUserAsync(User user);

        Task<LoginTokenResponseDTO> RefreshTokensAsync(RefreshTokenRequestDTO tokenRequestDTO);

    }
}
