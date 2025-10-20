using DebtCollectionPortal.Domain;

namespace DebtCollectionPortal.Application;

public interface IClientService
{
    // Operações agora são baseadas em AccountId (a API externa é account-centric)
    Task<ClientDto?> GetClientAsync(DebtCollectionPortal.Domain.AccountId accountId);
    Task<bool> UpdateAddressAsync(DebtCollectionPortal.Domain.AccountId accountId, AddressDto dto);
    Task<bool> UpdateEmailAsync(DebtCollectionPortal.Domain.AccountId accountId, string newEmail, string password);
}
