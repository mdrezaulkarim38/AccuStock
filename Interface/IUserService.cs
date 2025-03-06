using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IUserService 
    {
        Task<List<User>> GetAllUsers();
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);
        Task<bool> ToggleUserStatusAsync(int userId);
    }
}
