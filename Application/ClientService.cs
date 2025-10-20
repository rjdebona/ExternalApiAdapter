using System;
using System.Threading.Tasks;
using DebtCollectionPortal.Domain;

namespace DebtCollectionPortal.Application;

public class ClientService : IClientService
{
    private readonly Domain.IClientRepository _clientRepository;
    private readonly Domain.IAccountRepository _accountRepository;

    public ClientService(Domain.IClientRepository clientRepository, Domain.IAccountRepository accountRepository)
    {
        _clientRepository = clientRepository;
        _accountRepository = accountRepository;
    }

    public async Task<ClientDto?> GetClientAsync(DebtCollectionPortal.Domain.AccountId accountId)
    {
    var client = await _clientRepository.GetByAccountIdAsync(accountId);
        if (client == null) return null;

        return new ClientDto
        {
            FirstName = client.FirstName,
            LastName = client.LastName,
            DateOfBirth = client.DateOfBirth,
            Address = new AddressDto(client.Address.Street, client.Address.State, client.Address.City, client.Address.Postcode, client.Address.Country),
            Email = client.Email.Value,
            Phone = client.Phone
        };
    }

    public async Task<bool> UpdateAddressAsync(DebtCollectionPortal.Domain.AccountId accountId, AddressDto dto)
    {
        var res = await _clientRepository.UpdateAddressAsync(accountId, dto);
        return res.Ok;
    }

    public async Task<bool> UpdateEmailAsync(DebtCollectionPortal.Domain.AccountId accountId, string newEmail, string password)
    {
        var res = await _clientRepository.UpdateEmailAsync(accountId, newEmail, password);
        return res.Ok;
    }
}