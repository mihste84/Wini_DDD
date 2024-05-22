namespace AppLogic.WiniLogic.Exceptions;

public class AttachmentServiceException : Exception
{
    public UploadedAttachmentInput? Input { get; }
    public AttachmentServiceException(UploadedAttachmentInput input)
    {
        Input = input;
    }

    public AttachmentServiceException()
    {
    }

    public AttachmentServiceException(string? message) : base(message)
    {
    }

    public AttachmentServiceException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}