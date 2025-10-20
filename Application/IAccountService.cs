using DebtCollectionPortal.Domain;

namespace DebtCollectionPortal.Application;

public interface IAccountService
{
    Task<AccountDto?> GetAccountAsync(AccountId accountId);
    
}
