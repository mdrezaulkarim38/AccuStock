using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.DTOS.AuthDto;
using AccuStock.Interface;
using AccuStock.Models;

namespace AccuStock.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext  _context;

        public AuthService(AppDbContext context)
        {
            _context = context;
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

        public async Task<User> RegisterAsync(RegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Email == registerDto.Email))
            {
                throw new Exception("Email already exists");
            }

            int subscriptionId = await GetDefaultSubscriptionId();
            // Create new user
            var user = new User
            {
                FullName = registerDto.FullName,
                Email = registerDto.Email,
                Password = registerDto.Password,
                Mobile = registerDto.ContactNumber,
                RoleId = registerDto.RoleId ?? 1, // Default to 1 if not provided
                Status = true, // Active by default
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                SubscriptionId = subscriptionId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return user;
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

