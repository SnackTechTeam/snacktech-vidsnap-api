namespace Vidsnap.Application.DTOs.Responses
{
    public record class NovoVideoResponse(
        Guid Id,
        Guid IdUsuario,
        string Nome,
        string Extensao,
        int Tamanho,
        int Duracao,
        DateTime DataInclusao,
        string StatusAtual
    )
    {
        public string UrlPreAssinadaDeUpload { get; set; }
    }
}
