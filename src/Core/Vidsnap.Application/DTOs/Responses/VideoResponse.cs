namespace Vidsnap.Application.DTOs.Responses
{
    public record class VideoResponse(
        Guid Id,
        Guid IdUsuario,
        string Nome,
        string Extensao,
        int Tamanho,
        int Duracao,
        string? URLZip,
        string? URLImagem,
        DateTime DataInclusao,
        string StatusAtual,
        IReadOnlyList<VideoStatusResponse> StatusHistory
    );    
}