using DebtCollectionPortal.Domain;

namespace DebtCollectionPortal.Application;

public interface IUserService
{
    Task<UserResult> RegisterAsync(RegisterDto dto);
    Task<UserResult> LoginAsync(LoginDto dto);
}
