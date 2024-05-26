namespace Domain.Common.Entities;

public class Commissioner(string userId) : User(userId, true, "Commissioner")
{
}
