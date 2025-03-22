using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Enums;

namespace Vidsnap.Domain.Ports.Outbound
{
    public interface IVideoRepository
    {
        Task CriarAsync(Video video);
        Task<Video?> ObterPorIdAsync(Guid idVideo);
        Task<IReadOnlyList<Video>> ObterTodosDoUsuarioAsync(Guid idUsuario);
        Task<int> AtualizarStatusProcessamentoAsync(Video video, Status newStatus);
    }
}