namespace DebtCollectionPortal.Domain;

public class Client{
    
    public AccountId AccountId { get; private set; } = null!;
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public DateTime DateOfBirth { get; private set; }
    public Address Address { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string Phone { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private Client() { }
    public Client(AccountId accountId, string firstName, string lastName, DateTime dateOfBirth, Address address, Email email, string phone)
    {
        AccountId = accountId ?? throw new System.ArgumentNullException(nameof(accountId));
        FirstName = firstName;
        LastName = lastName;
        DateOfBirth = dateOfBirth;
        Address = address;
        Email = email;
        Phone = phone;
        CreatedAt = DateTime.UtcNow;
    }

    public void UpdateAddress(Address newAddress)
    {
        if (!newAddress.IsValid()) throw new DomainException("Invalid address information");
        Address = newAddress;
    }

    public void UpdateEmail(Email newEmail)
    {
        Email = newEmail ?? throw new DomainException("Invalid email");
    }
    
}