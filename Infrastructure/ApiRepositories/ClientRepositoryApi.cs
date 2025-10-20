using System.Net.Http;
using DebtCollectionPortal.Domain;
using DebtCollectionPortal.Application;
using System.Text.Json;

namespace DebtCollectionPortal.Infrastructure.ApiRepositories;

public class ClientRepositoryApi : IClientRepository
{
    private readonly HttpClient _http;
    private readonly ILogger<ClientRepositoryApi> _logger;

    public ClientRepositoryApi(HttpClient http, ILogger<ClientRepositoryApi> logger)
    {
        _http = http;
        _logger = logger;
    }

    public async Task<Client?> GetByAccountIdAsync(AccountId accountId)
    {
        try
        {
            var resp = await _http.GetAsync($"/api/account/{accountId}");
            var txt = await resp.Content.ReadAsStringAsync();
            if (!resp.IsSuccessStatusCode) return null;
            var data = JsonSerializer.Deserialize<AccountExternalDto>(txt, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (data == null || data.Debtor == null) return null;

            var debtor = data.Debtor;
            var address = new Address(debtor.Address.Street, debtor.Address.City, debtor.Address.State, debtor.Address.Postcode, debtor.Address.Country);
            var email = new Email(debtor.Email);
            DateTime dob = DateTime.MinValue; // external debtor may not provide DOB
            return new Client(accountId, debtor.FirstName, debtor.LastName, dob, address, email, debtor.Phone);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error in GetByAccountIdAsync");
            return null;
        }
    }

    public Task<Client?> GetByDetailsAsync(AccountId accountId, DateTime dateOfBirth, string postcode)
    {
        return GetByAccountIdAsync(accountId);
    }

    
    private record AccountExternalDto(string AccountId, Guid ClientId, decimal OutstandingBalance, string Status, bool IsOverdue, DateTime? LastPaymentDate, DateTime? NextPaymentDue, ClientExternalDto Debtor);
    private record ClientExternalDto(string FirstName, string LastName, string Email, AddressExternalDto Address, string Phone);
    private record AddressExternalDto(string Street, string City, string State, string Postcode, string Country);

    public Task SaveAsync(Client client)
    {
        _logger.LogInformation("SaveAsync called on ClientRepositoryApi - no-op");
        return Task.CompletedTask;
    }

    private StringContent JsonContent(object obj) => new StringContent(JsonSerializer.Serialize(obj), System.Text.Encoding.UTF8, "application/json");

    public async Task<DebtCollectionPortal.Domain.OperationResult> UpdateAddressAsync(DebtCollectionPortal.Domain.AccountId accountId, DebtCollectionPortal.Application.AddressDto dto)
    {
        try
        {
            var body = new { addressLine1 = dto.AddressLine1, addressLine2 = dto.AddressLine2, city = dto.City, postcode = dto.Postcode, country = dto.Country };
            var resp = await _http.PutAsync($"/api/account/{accountId}/address", JsonContent(body));
            var txt = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode) return new DebtCollectionPortal.Domain.OperationResult(true, (int)resp.StatusCode, null);
            _logger.LogWarning("Client.UpdateAddressAsync failed {Status} {Body}", resp.StatusCode, txt);
            return new DebtCollectionPortal.Domain.OperationResult(false, (int)resp.StatusCode, txt);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error in Client.UpdateAddressAsync");
            return new DebtCollectionPortal.Domain.OperationResult(false, 500, "Network error");
        }
    }

    public async Task<DebtCollectionPortal.Domain.OperationResult> UpdateEmailAsync(DebtCollectionPortal.Domain.AccountId accountId, string email, string password)
    {
        try
        {
            var body = new { email, password };
            var resp = await _http.PutAsync($"/api/account/{accountId}/email", JsonContent(body));
            var txt = await resp.Content.ReadAsStringAsync();
            if (resp.IsSuccessStatusCode) return new DebtCollectionPortal.Domain.OperationResult(true, (int)resp.StatusCode, null);
            _logger.LogWarning("Client.UpdateEmailAsync failed {Status} {Body}", resp.StatusCode, txt);
            return new DebtCollectionPortal.Domain.OperationResult(false, (int)resp.StatusCode, txt);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Network error in Client.UpdateEmailAsync");
            return new DebtCollectionPortal.Domain.OperationResult(false, 500, "Network error");
        }
    }
}
