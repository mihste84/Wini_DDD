namespace Domain.Wini.Enums;

[Flags]
public enum WiniStatus
{
    Saved,
    Cancelled,
    Sent,
    SendError,
    ToBeSent,
    ToBeAuthorized,
    NotAuthorizedOnTime
}