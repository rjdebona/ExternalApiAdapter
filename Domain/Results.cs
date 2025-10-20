namespace DebtCollectionPortal.Domain;

public record OperationResult(bool Ok, int Status, string? Error);
public record AuthResult(bool Ok, string? ExternalToken, int Status, string? Error);
