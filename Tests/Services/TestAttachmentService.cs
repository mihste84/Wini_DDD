using AppLogic.WiniLogic.Interfaces;

namespace Tests.MockServices;

public class TestAttachmentService : IAttachmentService
{
    public Task<bool> DeleteAttachmentAsync(string path)
    => Task.FromResult(true);


    public Task<Stream> GetAttachmentAsync(string path)
    => Task.FromResult<Stream>(new MemoryStream());

    public Task<(bool Success, string Path)> SaveAttachmentAsync(Stream Content, string FileName)
    => Task.FromResult((true, "path"));
}