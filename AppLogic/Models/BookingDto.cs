namespace AppLogic.Models;

public record BookingDto(
    short Status,
    DateTime BookingDate,
    string? TextToE1,
    bool Reversed,
    short LedgerType,
    string UpdatedBy,
    DateTime Updated,
    byte[] RowVersion,
    IEnumerable<BookingRowDto> Rows
);