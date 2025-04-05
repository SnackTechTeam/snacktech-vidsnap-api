namespace Vidsnap.Application.DTOs.Responses
{
    public record class LinksDeDownloadResponse(Guid IdVideo, string URLZip, string URLImagem, DateTime DataExpiracao);
}