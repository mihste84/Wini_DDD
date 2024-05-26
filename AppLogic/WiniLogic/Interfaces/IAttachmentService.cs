namespace AppLogic.WiniLogic.Interfaces;

public interface IAttachmentService
{
    Task<(bool Success, string Path)> SaveAttachmentAsync(Stream Content, string FileName);
    Task<bool> DeleteAttachmentAsync(string path);
    Task<Stream> GetAttachmentAsync(string path);
}