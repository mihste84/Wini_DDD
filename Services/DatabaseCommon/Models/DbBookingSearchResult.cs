namespace Services.DatabaseCommon.Models;

public record DbBookingSearchResult
{
    public int? Id { get; init; }
    public int? Status { get; init; }
    public DateTime? BookingDate { get; init; }
    public string? TextToE1 { get; init; }
    public int? AttachmentsCount { get; init; }
    public string? Comments { get; init; }
    public DateTime? Created { get; init; }
    public string? CreatedBy { get; init; }
}