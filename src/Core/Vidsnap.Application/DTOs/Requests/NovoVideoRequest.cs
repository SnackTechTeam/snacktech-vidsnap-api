namespace Vidsnap.Application.DTOs.Requests
{
    public record class NovoVideoRequest(
        Guid IdUsuario,
        string EmailUsuario,
        string Nome, 
        string Extensao, 
        int Tamanho,
        int Duracao
    );   
}