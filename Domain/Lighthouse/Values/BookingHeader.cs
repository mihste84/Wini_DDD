
namespace Domain.Lighthouse.Values;

public record BookingHeader
{
    public BookingDate BookingDate { get; }
    public bool IsReversed { get; }
    public InsuranceType InsuranceType { get; }

    public BookingHeader()
    {
        BookingDate = new BookingDate(DateTime.UtcNow);
        IsReversed = false;
        InsuranceType = new InsuranceType(Insurance.Claims);
    }

    public BookingHeader(DateOnly bookingDate, bool isReversed, Insurance insurance)
    {
        BookingDate = new BookingDate(bookingDate);
        IsReversed = isReversed;
        InsuranceType = new InsuranceType(insurance);
    }

    public BookingHeader(BookingDate bookingDate, bool isReversed, InsuranceType insuranceType)
    {
        BookingDate = bookingDate;
        IsReversed = isReversed;
        InsuranceType = insuranceType;
    }
}