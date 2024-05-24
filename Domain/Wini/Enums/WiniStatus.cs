namespace Domain.Wini.Enums;

public enum WiniStatus : byte
{
    New,
    Saved,
    Cancelled,
    Sent,
    SendError,
    ToBeSent,
    ToBeAuthorized,
    NotAuthorizedOnTime,

}