
namespace API.Services;

public class TestAttachmentService : IAttachmentService
{
    public Task<(bool Success, string Path)> SaveAttachmentAsync(UploadedAttachmentInput file)
    => Task.FromResult((true, "path"));
}