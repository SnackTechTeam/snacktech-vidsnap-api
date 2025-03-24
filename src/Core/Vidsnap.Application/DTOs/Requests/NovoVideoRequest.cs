namespace Vidsnap.Application.DTOs.Requests
{
    public record class NovoVideoRequest(
        Guid IdUsuario,
        string EmailUsuario,
        string NomeVideo, 
        string Extensao, 
        int Tamanho,
        int Duracao
    );   
}