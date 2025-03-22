namespace Vidsnap.Application.DTOs.Responses
{
    public record class VideoStatusResponse(
        string Status,
        DateTime DataInclusao
    );    
}
