using Microsoft.EntityFrameworkCore;
using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using AccuStock.Models.ViewModels.Auth;

namespace AccuStock.Services;
public class AuthService : IAuthService
{
    private readonly AppDbContext  _context;
    private readonly BaseService _baseService;

    public AuthService(AppDbContext context, BaseService baseService)
    {
        _context = context;
        _baseService = baseService;
    }

    public async Task<User> LoginAsync(string email, string password)
    {
        var user = await _context.Users
            .Include(u => u.Subscription)
            .FirstOrDefaultAsync(u => u.Email == email && u.Status);

        if (user == null)
        {
            throw new Exception("Invalid email or password");
        }

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

        var subscriptionId = await GetDefaultSubscriptionId();
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
        try
        {
            var userId = _baseService.GetUserId();
            if (userId == 0)
            {
                throw new Exception("User not found.");
            }
            var user = await _context.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null || user.Password != currentPassword)
            {
                throw new Exception("Current password is incorrect");
            }

            user.Password = newPassword;
            user.UpdatedAt = DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private async Task<int> GetDefaultSubscriptionId()
    {

         var subscription = new Subscription
            {
                IsActive = true                  
            };
            _context.Subscriptions.Add(subscription);
            await _context.SaveChangesAsync();
        
        return subscription.Id;
    }
}