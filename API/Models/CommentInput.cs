namespace API.Models;

public record CommentInput(
    DateTime? Created,
    string? Value,
    byte[]? RowVersion
);