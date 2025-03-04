using AccuStock.DTOS.AuthDto;
using AccuStock.Models;

namespace AccuStock.Interface
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterDto registerDto);
        Task<User> LoginAsync(string email, string password);
    }
}
