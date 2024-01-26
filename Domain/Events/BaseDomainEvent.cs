namespace Domain.Events;

public abstract class BaseDomainEvent
{
    public readonly Guid Id = Guid.NewGuid();
    public readonly DateTime Created = DateTime.UtcNow;
    public string Type { get; set; }

    protected BaseDomainEvent(string type)
    {
        Type = type;
    }
}