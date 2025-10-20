namespace DebtCollectionPortal.Domain;

public class Account{

    public AccountId Id { get; private set; } = null!;
    public Money OutstandingBalance { get; private set; } = new(0);
    public DateTime? LastPaymentDate { get; private set; }
    public DateTime? NextPaymentDue { get; private set; }
    
    private Account() { } // EF Constructor

    public Account(AccountId id, Money outstandingBalance,
                  DateTime? lastPaymentDate = null, DateTime? nextPaymentDue = null)
    {
        Id = id;
        OutstandingBalance = outstandingBalance;
        LastPaymentDate = lastPaymentDate;
        NextPaymentDue = nextPaymentDue;
    }

    public bool IsOverdue => NextPaymentDue.HasValue && NextPaymentDue < DateTime.Now;

}