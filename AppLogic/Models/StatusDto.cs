namespace AppLogic.Models;

public record StatusDto(
    WiniStatus Status,
    DateTime Updated,
    string UpdatedBy
);
