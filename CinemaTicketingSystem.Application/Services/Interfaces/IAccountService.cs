using CinemaTicketingSystem.Application.Common.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTicketingSystem.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> ValidateReCaptchaAsync(string response);
        Task<string> LoginAsync(LoginDto model);
        Task<string> RegisterAsync(RegisterUserDto model);
        Task LogoutAsync();
        Task<List<UserWithRolesDto>> SearchUsersAsync(string query);
        Task<UpdateUserDto> GetUserDetailsAsync(string userEmail);
        Task<string> UpdateUserAsync(UpdateUserDto model);
        Task<MyAccountDto> GetMyAccountAsync(string username);
        Task<string> ForgotPasswordAsync(ForgotPasswordDto model);
        Task<string> ResetPasswordAsync(ResetPasswordDto model);
    }
}
