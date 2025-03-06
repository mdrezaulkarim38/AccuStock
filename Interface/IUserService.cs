using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IUserService 
    {
        
        Task<bool> CreateUser(User user);
        Task<bool> UpdateUser(User user);

    }
}
