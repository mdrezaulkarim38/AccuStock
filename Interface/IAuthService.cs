using AccuStock.Models.ViewModels.Auth;
using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterViewModel registerViewModel);
        Task<User> LoginAsync(string email, string password);
    }
}
