using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DebtCollectionPortal.Domain;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using DebtCollectionPortal.Infrastructure;

namespace DebtCollectionPortal.Application;

public class UserService : IUserService
{
    private readonly IConfiguration _configuration;
    private readonly Domain.IAccountRepository _accountRepository;
    private readonly Domain.IUserRepository _userRepository;
    private readonly Domain.IClientRepository _clientRepository;

    public UserService(IConfiguration configuration, Domain.IAccountRepository accountRepository, Domain.IUserRepository userRepository, Domain.IClientRepository clientRepository)
    {
        _configuration = configuration;
        _accountRepository = accountRepository;
        _userRepository = userRepository;
        _clientRepository = clientRepository;
    }

    public async Task<UserResult> RegisterAsync(RegisterDto dto)
    {
        try
        {
            
            var requestedAccountId = (AccountId)dto.AccountId;
            var account = await _accountRepository.GetByIdAsync(requestedAccountId);
            if (account == null) return UserResult.CreateFailure("Account not found");

            
            var reg = await _userRepository.RegisterAsync(dto);
            if (!reg.Ok) return UserResult.CreateFailure(reg.Error ?? "Registration failed", reg.Status, reg.Error);

            
            var user = User.Create(account.Id, new Email(dto.Email), dto.Password);
            await _userRepository.SaveAsync(user);
            var token = GenerateJwtToken(user);
            return UserResult.CreateSuccess(token, user.AccountId.Value);
        }
        catch (DomainException dex) { return UserResult.CreateFailure(dex.Message); }
    catch (Exception ex) { return UserResult.CreateFailure("Internal error", 500, ex.Message); }
    }

    public async Task<UserResult> LoginAsync(LoginDto dto)
    {
        try
        {
            var auth = await _userRepository.ValidateAsync(dto.Email, dto.Password);
            if (!auth.Ok || string.IsNullOrEmpty(auth.ExternalToken))
            {
                return UserResult.CreateFailure(auth.Error ?? "Invalid email or password", auth.Status, auth.Error);
            }

            
            return UserResult.CreateSuccess(auth.ExternalToken ?? string.Empty, "");
        }
        catch (DomainException dex)
        {
            return UserResult.CreateFailure(dex.Message);
        }
        catch (Exception ex)
        {
            // Return internal error with exception message for debugging
            return UserResult.CreateFailure("Internal error", 500, ex.Message);
        }
    }

    private string GenerateJwtToken(User user)
    {
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? "your-jwt-key-must-be-at-least-32-characters-long");
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim("accountId", user.AccountId.Value),
                new Claim("email", user.Email.Value)
            }),
            Expires = DateTime.UtcNow.AddHours(24),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    
}