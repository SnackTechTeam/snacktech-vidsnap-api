using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Application.Extensions.Entities;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Domain.Guards;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.Application.UseCases
{
    public class BuscarVideosUseCase(IVideoRepository videoRepository) : IBuscarVideosUseCase
    {
        private readonly IVideoRepository _videoRepository = videoRepository;

        public async Task<ResultadoOperacao<IReadOnlyList<VideoResponse>>> ObterVideosDoUsuarioAsync(Guid idUsuario)
        {
            try
            {
                CommonGuards.AgainstEmptyGuid(idUsuario, nameof(idUsuario));

                var videos = await _videoRepository.ObterTodosDoUsuarioAsync(idUsuario);

                var videosResponse = videos.Select(v => v.ParaVideoResponse()).ToList();

                return new ResultadoOperacao<IReadOnlyList<VideoResponse>>(videosResponse);
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<IReadOnlyList<VideoResponse>>(ex);
            }
        }
    }
}