namespace Vidsnap.Application.DTOs.Requests
{
    public record class UrlPreAssinadaRequest(
        Guid IdUsuario,
        string NomeArquivo
        );
}
