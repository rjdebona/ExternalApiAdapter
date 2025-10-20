using System.Net.Http;
using System.Text;
using System.Text.Json;
using DebtCollectionPortal.Domain;
using DebtCollectionPortal.Application;

namespace DebtCollectionPortal.Infrastructure.ApiRepositories;

public class UserRepositoryApi : IUserRepository
{
    private readonly HttpClient _http;
    private readonly ILogger<UserRepositoryApi> _logger;

    public UserRepositoryApi(HttpClient http, ILogger<UserRepositoryApi> logger)
    {
        _http = http;
        _logger = logger;
    }

    private StringContent JsonContent(object obj) => new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    
    public Task SaveAsync(User user)
    {
        _logger.LogInformation("SaveAsync called on UserRepositoryApi - no-op");
        return Task.CompletedTask;
    }

    public async Task<AuthResult> ValidateAsync(string email, string password)
    {
        try
        {
            var resp = await _http.PostAsync("/api/webuser/validate", JsonContent(new { email, password }));
            var txt = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode)
            {
                var data = JsonSerializer.Deserialize<LoginResponse>(txt, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return new AuthResult(true, data?.token, (int)resp.StatusCode, null);
            }
            _logger.LogWarning("ValidateAsync failed {Status} {Body}", resp.StatusCode, txt);
            return new AuthResult(false, null, (int)resp.StatusCode, txt);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error on ValidateAsync");
            return new AuthResult(false, null, 500, "Network error");
        }
    }

    public async Task<OperationResult> RegisterAsync(RegisterDto dto)
    {
        try
        {
            var body = new { accountId = dto.AccountId, dateOfBirth = dto.DateOfBirth.ToString("yyyy-MM-dd"), postcode = dto.Postcode, email = dto.Email, password = dto.Password };
            var resp = await _http.PostAsync("/api/webuser/register", JsonContent(body));
            var txt = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode) return new OperationResult(true, (int)resp.StatusCode, null);
            _logger.LogWarning("RegisterAsync failed {Status} {Body}", resp.StatusCode, txt);
            return new OperationResult(false, (int)resp.StatusCode, txt);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error on RegisterAsync");
            return new OperationResult(false, 500, "Network error");
        }
    }

    
    private record LoginResponse(string token);
}
