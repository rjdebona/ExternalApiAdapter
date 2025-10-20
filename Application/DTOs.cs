namespace DebtCollectionPortal.Application;

public record RegisterDto(string AccountId, DateTime DateOfBirth, string Postcode, string Password, string Email);
public record LoginDto(string Email, string Password);
public record AddressDto(string AddressLine1, string AddressLine2, string City, string Postcode, string Country);
public record EmailUpdateDto(string Email, string Password);

public class UserResult
{
    public bool Success { get; init; }
    public string? Token { get; init; }
    public string? AccountId { get; init; }
    public string? Message { get; init; }
    public int? ExternalStatus { get; init; }
    public string? ExternalError { get; init; }

    public UserResult(bool success, string? token, string? accountId, string? message, int? externalStatus = null, string? externalError = null)
    {
        Success = success;
        Token = token;
        AccountId = accountId;
        Message = message;
        ExternalStatus = externalStatus;
        ExternalError = externalError;
    }

    public static UserResult CreateSuccess(string token, string accountId) => new(true, token, accountId, null);
    public static UserResult CreateFailure(string message, int? externalStatus = null, string? externalError = null) => new(false, null, null, message, externalStatus, externalError);
}

public record AccountDto
{
    public string AccountId { get; init; } = null!;
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public string FullName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Phone { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public AddressDto Address { get; init; } = null!;
    public string OutstandingBalance { get; init; } = null!;
    public bool IsOverdue { get; init; }
    public DateTime? LastPaymentDate { get; init; }
    public DateTime? NextPaymentDue { get; init; }
}

public record ClientDto
{
    public string FirstName { get; init; } = null!;
    public string LastName { get; init; } = null!;
    public DateTime DateOfBirth { get; init; }
    public AddressDto Address { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Phone { get; init; } = null!;
}