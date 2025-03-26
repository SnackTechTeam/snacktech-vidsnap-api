namespace Vidsnap.Application.DTOs.Requests
{
    public record class AtualizaStatusVideoRequest(
        Guid IdVideo,
        string Status,
        string? UrlZip = null,
        string? UrlImagem = null
    );
}
