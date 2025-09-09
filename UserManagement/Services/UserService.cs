using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Data;
using UserManagementSystem.Models;
using UserManagementSystem.ViewModels;
using BCrypt.Net;

namespace UserManagementSystem.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AuthenticateAsync(string email, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email && u.Status == UserStatus.Active);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.OrderBy(u => u.FullName).ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> CreateUserAsync(CreateUserViewModel model, string createdBy)
        {
            if (await EmailExistsAsync(model.Email))
                return false;

            var user = new User
            {
                FullName = model.FullName,
                Email = model.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                Role = model.Role,
                Status = model.Status,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateUserAsync(EditUserViewModel model, string modifiedBy)
        {
            var user = await _context.Users.FindAsync(model.Id);
            if (user == null)
                return false;

            if (await EmailExistsAsync(model.Email, model.Id))
                return false;

            user.FullName = model.FullName;
            user.Email = model.Email;
            user.Role = model.Role;
            user.Status = model.Status;
            user.ModifiedAt = DateTime.UtcNow;
            user.ModifiedBy = modifiedBy;

            if (!string.IsNullOrEmpty(model.Password))
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteUserAsync(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> EmailExistsAsync(string email, Guid? excludeId = null)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email && (excludeId == null || u.Id != excludeId));
        }
    }
}