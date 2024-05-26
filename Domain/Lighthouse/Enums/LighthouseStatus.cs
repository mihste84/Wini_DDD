namespace Domain.Lighthouse.Enums;

public enum LighthouseStatus : byte
{
    New,
    Saved,
    Sent,
    SendError,
    ToBeSent,
}