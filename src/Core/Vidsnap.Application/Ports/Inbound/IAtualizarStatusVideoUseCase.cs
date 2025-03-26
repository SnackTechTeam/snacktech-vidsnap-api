using Vidsnap.Application.DTOs.Responses;

namespace Vidsnap.Application.Ports.Inbound
{
    public interface IAtualizarStatusVideoUseCase
    {
        Task AtualizarStatusDeProcessamentoAsync(CancellationToken cancellationToken = default);
    }
}
