namespace AppLogic.Models;

public record BookingSearchResult(
    int BookingId,
    DateOnly BookingDate,
    WiniStatus Status,
    string Commissioner,
    int NumberOfAttachments,
    string? Comments,
    DateTime Created
);