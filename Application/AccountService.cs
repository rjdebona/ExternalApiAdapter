using System;
using System.Threading.Tasks;
using DebtCollectionPortal.Domain;
using DebtCollectionPortal.Infrastructure;

namespace DebtCollectionPortal.Application;

public class AccountService : IAccountService
{
    private readonly Domain.IAccountRepository _accountRepository;

    public AccountService(Domain.IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<AccountDto?> GetAccountAsync(AccountId accountId)
    {
    var acct = await _accountRepository.GetByIdAsync(accountId);
        if (acct != null)
        {
            return new AccountDto
            {
                AccountId = (string)acct.Id,
                FirstName = string.Empty,
                LastName = string.Empty,
                FullName = string.Empty,
                Email = string.Empty,
                Phone = string.Empty,
                DateOfBirth = default,
                Address = new AddressDto(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty),
                OutstandingBalance = acct.OutstandingBalance.FormatAsCurrency(),
                IsOverdue = acct.IsOverdue,
                LastPaymentDate = acct.LastPaymentDate,
                NextPaymentDue = acct.NextPaymentDue
            };
        }

    return null;
    }
}