using Microsoft.EntityFrameworkCore;
using UserManagementSystem.Models;
using BCrypt.Net;

namespace UserManagementSystem.Data
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // Ensure database and directory exist
            var dbPath = Path.GetDirectoryName(context.Database.GetConnectionString()?.Replace("Data Source=", ""));
            if (!string.IsNullOrEmpty(dbPath) && !Directory.Exists(dbPath))
            {
                Directory.CreateDirectory(dbPath);
            }

            await context.Database.EnsureCreatedAsync();

            // Check if users already exist
            if (await context.Users.AnyAsync())
                return;

            var users = new[]
            {
                new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "System Administrator",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
                    Role = UserRole.Admin,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    FullName = "Test User",
                    Email = "user@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("User123!"),
                    Role = UserRole.User,
                    Status = UserStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                }
            };

            context.Users.AddRange(users);
            await context.SaveChangesAsync();
        }
    }
}