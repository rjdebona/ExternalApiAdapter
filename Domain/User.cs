using System;

namespace DebtCollectionPortal.Domain;

public class User {

    public Guid Id { get; private set; }
    public AccountId AccountId { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }

    private User() { } // EF Constructor

    public static User Create(AccountId accountId, Email email, string password)
    {
        if (accountId == null) throw new DomainException("User must be linked to a valid account");
        if (string.IsNullOrEmpty(password) || password.Length < 6)
            throw new DomainException("Password must be at least 6 characters");

        return new User
        {
            AccountId = accountId,
            Email = email,
            PasswordHash = password,
            CreatedAt = DateTime.UtcNow
        };
    }

    public bool ValidatePassword(string plain)
    {
        if (string.IsNullOrEmpty(plain)) return false;
        return plain == PasswordHash;
    }
}