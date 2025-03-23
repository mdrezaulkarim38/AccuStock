using AccuStock.Data;
using AccuStock.Interface;
using AccuStock.Models;
using Microsoft.EntityFrameworkCore;
namespace AccuStock.Services
{
    public class UserService: IUserService
    {
        private readonly AppDbContext _context;
        private readonly BaseService _baseService;

        public UserService(AppDbContext context, BaseService baseService)
        {
            _context = context;
            _baseService = baseService;
        }

        public async Task<bool> CreateUser(User user)
        {
            try {
                var OldUserInfo = await _context.Users.Where(u => u.Email == user.Email).FirstOrDefaultAsync();
                if(OldUserInfo != null)
                {
                    return false;
                }
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                user.SubscriptionId = subscriptionIdClaim;
                user.Password = "1234";
                user.Status = true;
                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception) {
                throw;
            }
        }
        public async Task<bool> UpdateUser(User user)
        {
            try
            {
                var subscriptionIdClaim = _baseService.GetSubscriptionId();
                var existingEmail = await _context.Users.Where(u => u.Id != user.Id && u.Email == user.Email).FirstOrDefaultAsync();
                if(existingEmail != null)
                {
                    return false;
                }
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.SubscriptionId == subscriptionIdClaim && u.Id == user.Id);

                if (existingUser == null)
                {
                    return false;
                }

                existingUser.FullName = user.FullName;
                existingUser.Email = user.Email;
                existingUser.Mobile = user.Mobile;
                existingUser.Address = user.Address;
                existingUser.RoleId = user.RoleId;
                existingUser.BranchId = user.BranchId;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> ToggleUserStatusAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.Status = !user.Status;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.Where(u => u.SubscriptionId == _baseService.GetSubscriptionId() && u.RoleId != 1).ToListAsync();
        }
    }
}
