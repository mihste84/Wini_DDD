namespace Domain.Wini.Models;

public record BookingHeaderModel(
    DateOnly BookingDate,
    string? TextToE1,
    bool IsReversed,
    Ledgers LedgerType
);