namespace Vidsnap.Application.DTOs.Requests
{
    public record class AtualizaStatusVideoRequest(
        Guid IdVideo,
        string NovoStatus,
        string? UrlZip = null
    );
}
