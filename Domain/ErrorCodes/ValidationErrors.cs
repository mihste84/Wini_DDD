namespace Domain.ErrorCodes;

public static class ValidationErrorCodes
{
    public const int Required = 200;
    public const int TextTooLong = 201;
    public const int TextTooShort = 202;
    public const int ValueTooHigh = 203;
    public const int ValueTooLow = 204;
    public const int OutOfRange = 205;
    public const int IncorrectValue = 206;
    public const int IncorrectFormat = 207;
}