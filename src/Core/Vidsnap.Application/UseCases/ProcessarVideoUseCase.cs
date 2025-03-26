using Microsoft.Extensions.Options;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Application.Extensions.Entities;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.Application.UseCases
{
    public class ProcessarVideoUseCase(IVideoRepository videoRepository, 
        ICloudFileStorageService cloudFileStorageService, 
        IOptions<CloudFileStorageSettings> cloudFileStorageOption) : IProcessarVideoUseCase
    {        
        private readonly IVideoRepository _videoRepository = videoRepository;
        private readonly ICloudFileStorageService _cloudFileStorageService = cloudFileStorageService;
        private readonly CloudFileStorageSettings _cloudFileStorageSettings = cloudFileStorageOption.Value;

        public async Task<ResultadoOperacao<string>> GerarUrlPreAssinadaParaUpload(UrlPreAssinadaRequest urlPreAssinadaRequest)
        {
            try
            {
                var presignedUrl = await ObterUrlPreAssinadaAsync(urlPreAssinadaRequest.IdUsuario, null, urlPreAssinadaRequest.NomeArquivo);

                return new ResultadoOperacao<string>(presignedUrl);
            }
            catch (Exception ex)
            {
                return new ResultadoOperacao<string>(ex);
            }
        }

        public async Task<ResultadoOperacao<NovoVideoResponse>> EnviarVideoParaProcessamentoAsync(NovoVideoRequest novoVideoRequest)
        {
            try
            {
                var video = novoVideoRequest.ParaVideo();
                await _videoRepository.CriarAsync(video);

                var presignedUrl = await ObterUrlPreAssinadaAsync(novoVideoRequest.IdUsuario, video.Id, novoVideoRequest.NomeVideo);

                var novoVideoResponse = video.ParaNovoVideoResponse();
                novoVideoResponse.UrlPreAssinadaDeUpload = presignedUrl;

                return new ResultadoOperacao<NovoVideoResponse>(novoVideoResponse);
            } 
            catch (Exception ex)
            {
                return new ResultadoOperacao<NovoVideoResponse>(ex);
            }
        }        

        #region Private Methods

        private async Task<string> ObterUrlPreAssinadaAsync(Guid idUsuario, Guid? idVideo, string nomeArquivo)
        {
            var presignedUrl = await _cloudFileStorageService.GetPreSignedURLAsync(
                    _cloudFileStorageSettings.ContainerName,
                    _cloudFileStorageSettings.TimeoutDuration,
                    idUsuario,
                    idVideo,
                    nomeArquivo
                );

            return presignedUrl;
        }

        #endregion
    }
}