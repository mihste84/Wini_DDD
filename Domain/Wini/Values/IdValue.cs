namespace Domain.Wini.Values;

public record IdValue<T> where T : notnull
{
    public readonly T Value;

    public IdValue(T value)
    {
        Value = value;
    }
}