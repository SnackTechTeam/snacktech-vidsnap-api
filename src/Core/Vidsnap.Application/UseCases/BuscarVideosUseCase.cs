using Microsoft.Extensions.Options;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Application.Extensions.Entities;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Domain.Guards;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.Application.UseCases
{
    public class BuscarVideosUseCase(IVideoRepository videoRepository, 
        IOptions<CloudFileStorageSettings> cloudFileStorageOption) : IBuscarVideosUseCase
    {
        private readonly IVideoRepository _videoRepository = videoRepository;
        private readonly CloudFileStorageSettings _cloudFileStorage = cloudFileStorageOption.Value;

        public async Task<ResultadoOperacao<IReadOnlyList<VideoResponse>>> ObterVideosDoUsuarioAsync(Guid idUsuario)
        {
            try
            {
                CommonGuards.AgainstEmptyGuid(idUsuario, nameof(idUsuario));

                var videos = await _videoRepository.ObterTodosDoUsuarioAsync(idUsuario);

                var videosResponse = videos.Select(v => 
                { 
                    VideoResponse videoReponse = v.ParaVideoResponse();

                    if (!string.IsNullOrEmpty(videoReponse.URLZip) && !string.IsNullOrEmpty(videoReponse.URLImagem))
                    {
                        videoReponse.URLZip = $"https://{_cloudFileStorage.ContainerName}.s3.amazonaws.com{videoReponse.URLZip}";
                        videoReponse.URLImagem = $"https://{_cloudFileStorage.ContainerName}.s3.amazonaws.com{videoReponse.URLImagem}";
                    }                    

                    return videoReponse;
                }).ToList();

                return new ResultadoOperacao<IReadOnlyList<VideoResponse>>(videosResponse);
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<IReadOnlyList<VideoResponse>>(ex);
            }
        }
    }
}