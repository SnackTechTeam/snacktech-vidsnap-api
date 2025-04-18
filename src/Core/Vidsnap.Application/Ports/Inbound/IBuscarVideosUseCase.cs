﻿using Vidsnap.Application.DTOs.Responses;

namespace Vidsnap.Application.Ports.Inbound
{
    public interface IBuscarVideosUseCase
    {
        Task<ResultadoOperacao<IReadOnlyList<VideoResponse>>> ObterVideosDoUsuarioAsync(Guid idUsuario);
        Task<ResultadoOperacao<LinksDeDownloadResponse>> ObterLinksDeDownloadAsync(Guid idVideo, Guid idUsuario);
    }
}
