namespace Domain.Wini.Entities;

public class Commissioner : User
{
    public Commissioner(string userId) : base(userId, true, "Commissioner") { }
}
