namespace Services.DatabaseDapper.Extensions;
public static class NullableExtensions
{
    public static M GetValue<M>(this M? value, string propName) where M : struct
    => value ?? throw new DatabaseValueException("Value could not be retrieved from " + propName);

    public static M GetValue<M>(this M? value, string propName) where M : class
    => value ?? throw new DatabaseValueException("Value could not be retrieved from " + propName);
}