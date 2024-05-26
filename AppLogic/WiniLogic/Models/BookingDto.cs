namespace AppLogic.WiniLogic.Models;

public record BookingDto(
    int BookingId,
    WiniStatus Status,
    string BookingDate,
    string? TextToE1,
    bool Reversed,
    Ledgers LedgerType,
    string Commissioner,
    string UpdatedBy,
    DateTime Updated,
    byte[] RowVersion,
    IEnumerable<BookingRowDto> Rows,
    IEnumerable<StatusDto> StatusHistory,
    IEnumerable<CommentDto> Comments,
    IEnumerable<RecipientMessageDto> Messages,
    IEnumerable<AttachmentDto> Attachments,
    int[]? DeletedRowNumbers,
    int MaxRowNumber
);