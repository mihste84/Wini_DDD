namespace AppLogic.WiniLogic.Interfaces;

public interface ICommentRepository
{
    Task<SqlResult?> DeleteCommentAsync(int commentId, int bookingId, string updatedBy);
    Task<SqlResult?> InsertCommentAsync(Comment comment, string createdBy);
    Task<SqlResult?> UpdateCommentAsync(Comment comment, int commentId, string updatedBy);
}
