namespace Domain.Models;

public record BookingValidationResultModel
{
    public string? Message { get; set; }
    public bool IsValid { get; set; } = true;
    public IEnumerable<ValidationError>? Errors { get; set; }
}