using UserManagementSystem.Models;
using UserManagementSystem.ViewModels;

namespace UserManagementSystem.Services
{
    public interface IUserService
    {
        Task<User?> AuthenticateAsync(string email, string password);
        Task<IEnumerable<User>> GetAllUsersAsync();
        Task<User?> GetUserByIdAsync(Guid id);
        Task<User?> GetUserByEmailAsync(string email);
        Task<bool> CreateUserAsync(CreateUserViewModel model, string createdBy);
        Task<bool> UpdateUserAsync(EditUserViewModel model, string modifiedBy);
        Task<bool> DeleteUserAsync(Guid id);
        Task<bool> EmailExistsAsync(string email, Guid? excludeId = null);
    }
}