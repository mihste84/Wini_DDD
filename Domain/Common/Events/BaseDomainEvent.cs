namespace Domain.Common.Events;

public abstract class BaseDomainEvent(string type)
{
    public readonly Guid Id = Guid.NewGuid();
    public readonly DateTime Created = DateTime.UtcNow;
    public string Type { get; set; } = type;
}