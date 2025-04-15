namespace Vidsnap.Application.DTOs.Broker
{
    public record class ProcessamentoVideoFinalizadoMessage(string Email, string NomeVideo, string Status);
}
