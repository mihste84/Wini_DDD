namespace Domain.Wini.Models;

public record BookingHeaderModel(
    DateTime BookingDate,
    string TextToE1,
    bool IsReversed,
    Ledgers LedgerType
);