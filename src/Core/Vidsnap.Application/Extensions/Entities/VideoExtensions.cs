using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Domain.Entities;

namespace Vidsnap.Application.Extensions.Entities
{
    public static class VideoExtensions
    {
        public static Video ParaVideo(this NovoVideoRequest novoVideoRequest)
        {
            return new(novoVideoRequest.IdUsuario,
                novoVideoRequest.EmailUsuario,
                novoVideoRequest.NomeVideo, 
                novoVideoRequest.Extensao, 
                novoVideoRequest.Tamanho, 
                novoVideoRequest.Duracao
            );
        }

        public static NovoVideoResponse ParaNovoVideoResponse(this Video video)
        {
            return new(video.Id,
                video.IdUsuario,
                video.NomeVideo,
                video.Extensao,
                video.Tamanho,
                video.Duracao,
                video.DataInclusao,
                video.StatusAtual.ToString()
            );
        }

        public static VideoResponse ParaVideoResponse(this Video video)
        {
            var videoStatuses = video.VideoStatuses
                .Select(vs => vs.ParaVideoStatusResponse())
                .ToList();

            return new(video.Id,
                video.IdUsuario,
                video.NomeVideo,
                video.Extensao,
                video.Tamanho,
                video.Duracao,
                video.URLZip,
                video.DataInclusao,
                video.StatusAtual.ToString(),
                videoStatuses
            );
        }

        private static VideoStatusResponse ParaVideoStatusResponse(this VideoStatus videoStatus)
        {
            return new(videoStatus.Status.ToString(), videoStatus.DataInclusao);
        }
    }
}
