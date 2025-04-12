namespace Vidsnap.Application.DTOs.Requests
{
    public record class NovoVideoBodyRequest(
        string NomeVideo, 
        string Extensao, 
        int Tamanho,
        int Duracao
    );
}