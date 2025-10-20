using System.Net.Http;
using System.Text;
using System.Text.Json;
using DebtCollectionPortal.Domain;
using DebtCollectionPortal.Application;

namespace DebtCollectionPortal.Infrastructure.ApiRepositories;

public class AccountRepositoryApi : IAccountRepository
{
    private readonly HttpClient _http;
    private readonly ILogger<AccountRepositoryApi> _logger;

    public AccountRepositoryApi(HttpClient http, ILogger<AccountRepositoryApi> logger)
    {
        _http = http;
        _logger = logger;
    }

    private StringContent JsonContent(object obj) => new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");

    public async Task<Account?> GetByIdAsync(AccountId accountId)
    {
        try
        {
            var resp = await _http.GetAsync($"/api/account/{accountId}");
            var txt = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode) return null;
            var data = JsonSerializer.Deserialize<AccountExternal>(txt, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (data == null) return null;
            return new Account(accountId, (decimal)data.OutstandingBalance, data.LastPaymentDate, data.NextPaymentDue);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error in GetByIdAsync");
            return null;
        }
    }

    public async Task<Account?> GetByDetailsAsync(AccountId accountId, DateTime dateOfBirth, string postcode)
    {
        return await GetByIdAsync(accountId);
    }

    public Task SaveAsync(Account account)
    {
        _logger.LogInformation("SaveAsync called on AccountRepositoryApi - no-op");
        return Task.CompletedTask;
    }

    
    public record AccountExternal(string AccountId, decimal OutstandingBalance, string Status, bool IsOverdue, DateTime? LastPaymentDate, DateTime? NextPaymentDue, ClientExternal Debtor);
    public record ClientExternal(string FirstName, string LastName, string Email, AddressExternal Address);
    public record AddressExternal(string Street, string City, string State, string Postcode, string Country);
}
