namespace AppLogic.Interfaces;

public interface IAttachmentService
{
    Task<(bool Success, string Path)> SaveAttachmentAsync(UploadedAttachmentInput file);
}