using Microsoft.Extensions.Options;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Application.Extensions.Entities;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Guards;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.Application.UseCases
{
    public class BuscarVideosUseCase(IVideoRepository videoRepository,
        ICloudFileStorageService cloudFileStorageService,
        IOptions<CloudFileStorageSettings> cloudFileStorageOption) : IBuscarVideosUseCase
    {
        private readonly IVideoRepository _videoRepository = videoRepository;
        private readonly ICloudFileStorageService _cloudFileStorageService = cloudFileStorageService;
        private readonly CloudFileStorageSettings _cloudFileStorageSettings = cloudFileStorageOption.Value;        

        public async Task<ResultadoOperacao<IReadOnlyList<VideoResponse>>> ObterVideosDoUsuarioAsync(Guid idUsuario)
        {
            try
            {
                CommonGuards.AgainstEmptyGuid(idUsuario, nameof(idUsuario));

                var videos = await _videoRepository.ObterTodosDoUsuarioAsync(idUsuario);

                var videosResponse = videos.Select(v => 
                { 
                    VideoResponse videoReponse = v.ParaVideoResponse();                                        

                    return videoReponse;
                }).ToList();

                return new ResultadoOperacao<IReadOnlyList<VideoResponse>>(videosResponse);
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<IReadOnlyList<VideoResponse>>(ex);
            }
        }

        public async Task<ResultadoOperacao<LinksDeDownloadResponse>> ObterLinksDeDownloadAsync(Guid idVideo, Guid idUsuario)
        {
            try
            {
                CommonGuards.AgainstEmptyGuid(idVideo, nameof(idVideo));
                CommonGuards.AgainstEmptyGuid(idUsuario, nameof(idUsuario));

                var video = await _videoRepository.ObterPorUsuarioAsync(idVideo, idUsuario);

                if (video is null)
                {
                    return new ResultadoOperacao<LinksDeDownloadResponse>($"Video {idVideo} não encontrado!", true);
                }

                if (video.StatusAtual != Status.FinalizadoComSucesso)
                {
                    return new ResultadoOperacao<LinksDeDownloadResponse>($"Status do video {idVideo} não é finalizado com sucesso!", true);
                }

                if (!string.IsNullOrEmpty(video.URLZip) && !string.IsNullOrEmpty(video.URLImagem))
                {
                    var uriZipParts = video.URLZip.Replace("s3://", "").Split('/', 2);
                    
                    if (uriZipParts.Length < 2)
                        return new ResultadoOperacao<LinksDeDownloadResponse>($"Link do zip do video {idVideo} inválido!", true);

                    string zipBucketName = uriZipParts[0];
                    string zipObjectKey = uriZipParts[1];

                    var uriImageParts = video.URLImagem.Replace("s3://", "").Split('/', 2);

                    if (uriImageParts.Length < 2)
                        return new ResultadoOperacao<LinksDeDownloadResponse>($"Link da imagem do video {idVideo} inválido!", true);

                    string imageBucketName = uriImageParts[0];
                    string imageObjectKey = uriImageParts[1];

                    string zipPreSignedUrl = await _cloudFileStorageService.GetDownloadPreSignedURLAsync(
                        zipBucketName, _cloudFileStorageSettings.TimeoutDuration, zipObjectKey);

                    string imagePreSignedUrl = await _cloudFileStorageService.GetDownloadPreSignedURLAsync(
                        imageBucketName, _cloudFileStorageSettings.TimeoutDuration, imageObjectKey);

                    var linksDeDownloadResponse = new LinksDeDownloadResponse(idVideo, 
                        zipPreSignedUrl, 
                        imagePreSignedUrl,
                        DateTime.Now.AddMinutes(_cloudFileStorageSettings.TimeoutDuration)
                    );

                    return new ResultadoOperacao<LinksDeDownloadResponse>(linksDeDownloadResponse);
                }

                return new ResultadoOperacao<LinksDeDownloadResponse>($"Video {idVideo} sem zip ou imagem gerados!", true);
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<LinksDeDownloadResponse>(ex);
            }
        }
    }
}