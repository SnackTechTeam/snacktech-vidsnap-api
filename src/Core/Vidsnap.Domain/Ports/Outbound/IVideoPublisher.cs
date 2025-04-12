using System.Threading;
using Vidsnap.Domain.Entities;

namespace Vidsnap.Domain.Ports.Outbound
{
    public interface IVideoPublisher
    {
        Task PublicarProcessamentoFinalizadoAsync(Video video, CancellationToken cancellationToken = default);
    }
}
