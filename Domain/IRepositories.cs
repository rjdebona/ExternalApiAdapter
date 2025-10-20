namespace DebtCollectionPortal.Domain;
public interface IAccountRepository
{
    Task<Account?> GetByIdAsync(AccountId accountId);
    Task<Account?> GetByDetailsAsync(AccountId accountId, DateTime dateOfBirth, string postcode);
    Task SaveAsync(Account account);

}

public interface IUserRepository
{
    
    Task SaveAsync(User user);
    Task<DebtCollectionPortal.Domain.AuthResult> ValidateAsync(string email, string password);
    Task<DebtCollectionPortal.Domain.OperationResult> RegisterAsync(DebtCollectionPortal.Application.RegisterDto dto);
}

public interface IClientRepository
{
    
    Task<Client?> GetByAccountIdAsync(AccountId accountId);
    Task<Client?> GetByDetailsAsync(AccountId accountId, DateTime dateOfBirth, string postcode);
    Task SaveAsync(Client client);
    Task<DebtCollectionPortal.Domain.OperationResult> UpdateAddressAsync(AccountId accountId, DebtCollectionPortal.Application.AddressDto dto);
    Task<DebtCollectionPortal.Domain.OperationResult> UpdateEmailAsync(AccountId accountId, string email, string password);
}

 