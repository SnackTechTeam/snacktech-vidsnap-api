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
                var presignedUrl = await ObterUrlPreAssinadaAsync(urlPreAssinadaRequest.IdUsuario, urlPreAssinadaRequest.NomeArquivo);

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

                var presignedUrl = await ObterUrlPreAssinadaAsync(novoVideoRequest.IdUsuario, novoVideoRequest.Nome);

                var novoVideoResponse = video.ParaNovoVideoResponse();
                novoVideoResponse.UrlPreAssinadaDeUpload = presignedUrl;

                return new ResultadoOperacao<NovoVideoResponse>(novoVideoResponse);
            } 
            catch (Exception ex)
            {
                return new ResultadoOperacao<NovoVideoResponse>(ex);
            }
        }

        public async Task<ResultadoOperacao> AtualizarStatusDeProcessamentoAsync(Guid idVideo, AtualizaStatusVideoRequest atualizaStatusVideoRequest)
        {
            try
            {
                if (Enum.TryParse(atualizaStatusVideoRequest.NovoStatus, true, out Status status))
                {
                    var video = await _videoRepository.ObterPorIdAsync(idVideo);

                    if (video is null)
                    {
                        return new ResultadoOperacao("Video não encontrado!");
                    }

                    if (video.VideoStatuses.Any(vs => vs.Status == status))
                    {
                        return new ResultadoOperacao($"O vídeo {idVideo} já chegou no status {status}");
                    }

                    video.AtualizarStatus(status);

                    //Somente inclui a url do zip se ela for não nula e o status for FinalizadoComSucesso
                    if (!string.IsNullOrEmpty(atualizaStatusVideoRequest.UrlZip)
                        && status == Status.FinalizadoComSucesso)
                    {
                        video.IncluirURLZipe(atualizaStatusVideoRequest.UrlZip);
                    }

                    await _videoRepository.AtualizarStatusProcessamentoAsync(video, status);

                    return new ResultadoOperacao();
                }
                else
                {
                    return new ResultadoOperacao("Status inválido!");
                }
            }
            catch (Exception ex) 
            {
                return new ResultadoOperacao(ex);
            }
        }

        #region Private Methods

        private async Task<string> ObterUrlPreAssinadaAsync(Guid idUsuario, string nomeArquivo)
        {
            var presignedUrl = await _cloudFileStorageService.GetPreSignedURLAsync(
                    _cloudFileStorageSettings.ContainerName,
                    _cloudFileStorageSettings.TimeoutDuration,
                    idUsuario,
                    nomeArquivo
                );

            return presignedUrl;
        }

        #endregion
    }
}