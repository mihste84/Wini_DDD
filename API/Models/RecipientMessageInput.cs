namespace API.Models;

public record RecipientMessageInput(
    string? Recipient,
    string? Value,
    byte[]? RowVersion
);