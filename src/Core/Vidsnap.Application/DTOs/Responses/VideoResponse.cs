namespace Vidsnap.Application.DTOs.Responses
{
    public record class VideoResponse(
        Guid Id,
        Guid IdUsuario,
        string Nome,
        string Extensao,
        int Tamanho,
        int Duracao,
        DateTime DataInclusao,
        string StatusAtual,
        IReadOnlyList<VideoStatusResponse> StatusHistory
    )
    {
        public string? URLZip { get; set; }
        public string? URLImagem { get; set; }
    }    
}