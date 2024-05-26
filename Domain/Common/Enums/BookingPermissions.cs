namespace Domain.Common.Enums;

[Flags]
public enum BookingPermissions : byte
{
    None = 0,
    BookingRead = 1,
    BookingWrite = 2,
    AccountingUser = 4,
    ControlActuary = 8,
    SpecificActuary = 16,
    LhTxUser = 32,
    Admin = 64
}