using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Models.ViewModels.Auth;
using System.Security.Claims;

namespace AccuStock.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext  _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(AppDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            // Find user by email
            var user = await _context.Users
                .Include(u => u.Subscription)
                .FirstOrDefaultAsync(u => u.Email == email && u.Status); // Only active users

            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }

            // Verify password
            if (password != user.Password)
            {
                throw new Exception("Invalid email or password");
            }
            return user;
        }

        public async Task<User> RegisterAsync(RegisterViewModel registerViewModel)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerViewModel.Email))
            {
                throw new Exception("Email already exists");
            }

            int subscriptionId = await GetDefaultSubscriptionId();
            // Create new user
            var user = new User
            {
                FullName = registerViewModel.FullName,
                Email = registerViewModel.Email,
                Password = registerViewModel.Password,
                Mobile = registerViewModel.ContactNumber,
                RoleId = registerViewModel.RoleId ?? 1, // Default to 1 if not provided
                Status = true, // Active by default
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                SubscriptionId = subscriptionId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
        }
        public async Task<bool> ResetPasswordAsync(string currentPassword, string newPassword)
        {
            var userId = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                throw new Exception("User not found.");
            }
            var user = await _context.Users.Where(u => u.Id == int.Parse(userId)).FirstOrDefaultAsync();
            if (user == null || user.Password != currentPassword)
            {
                throw new Exception("Current password is incorrect");
            }

            user.Password = newPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }

        private async Task<int> GetDefaultSubscriptionId()
        {

             var subscription = new Subscription
                {
                    IsActive = true                  
                };
                _context.Subscriptions.Add(subscription);
                await _context.SaveChangesAsync(); // This persists the new subscription and assigns an ID
            
            return subscription.Id;
        }
    }
}

