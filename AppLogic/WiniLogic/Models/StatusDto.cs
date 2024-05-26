namespace AppLogic.WiniLogic.Models;

public record StatusDto(
    WiniStatus Status,
    DateTime Updated,
    string UpdatedBy
);
