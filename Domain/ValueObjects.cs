namespace DebtCollectionPortal.Domain;

public record Address(string Street, string City, string State, string Postcode, string Country){

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(Street) &&
               !string.IsNullOrEmpty(City) &&
               !string.IsNullOrEmpty(State) &&
               !string.IsNullOrEmpty(Postcode) &&
               !string.IsNullOrEmpty(Country);
    }
}

public record Email
{
    public string Value { get; init; }

    public Email(string value)
    {
        if (string.IsNullOrEmpty(value) || !value.Contains("@"))
            throw new DomainException("Invalid email format");
        Value = value;
    }

    public static implicit operator string(Email email) => email.Value;
    public static implicit operator Email(string value) => new(value);
}

public record AccountId
{
    public string Value { get; init; }

    public AccountId(string value)
    {
        if (string.IsNullOrEmpty(value) || !value.StartsWith("ACC-"))
            throw new DomainException("Invalid AccountId format");
        Value = value;
    }

    public static implicit operator string(AccountId accountId) => accountId.Value;
    public static implicit operator AccountId(string value) => new(value);
}

public record Money
{
    public decimal Value { get; init; }

    public Money(decimal value)
    {
        Value = value;
    }

    public static implicit operator decimal(Money money) => money.Value;
    public static implicit operator Money(decimal value) => new(value);

    public string FormatAsCurrency() => $"£{Value:N2}";
}


