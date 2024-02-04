namespace API.Models;

public record BaseResponse<M>
{
    public BaseResponse(M value)
    {
        Value = value;
    }

    public M Value { get; set; }
}