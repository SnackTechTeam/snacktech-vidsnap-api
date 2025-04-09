using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Net;
using Vidsnap.Api.Controllers;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Responses;
using Vidsnap.Application.Ports.Inbound;

namespace Vidsnap.UnitTest.Driving.Vidsnap.API.Controllers
{
    public class VideosControllerTests
    {
        private readonly Mock<IProcessarVideoUseCase> _processarVideoUseCaseMock;
        private readonly Mock<IBuscarVideosUseCase> _buscarVideosUseCaseMock;
        private readonly Mock<ILogger<IProcessarVideoUseCase>> _loggerMock;
        private readonly VideosController _controller;

        public VideosControllerTests()
        {
            _processarVideoUseCaseMock = new Mock<IProcessarVideoUseCase>();
            _buscarVideosUseCaseMock = new Mock<IBuscarVideosUseCase>();
            _loggerMock = new Mock<ILogger<IProcessarVideoUseCase>>();

            _controller = new VideosController(
                _loggerMock.Object,
                _processarVideoUseCaseMock.Object,
                _buscarVideosUseCaseMock.Object
            );
        }

        #region Post

        [Fact]
        public async Task Post_DeveRetornarOk_QuandoVideoForProcessadoComSucesso()
        {
            // Arrange
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", "video.mp4", "mp4", 1000, 120);
            var response = new NovoVideoResponse(Guid.NewGuid(), request.IdUsuario, request.NomeVideo, request.Extensao, request.Tamanho, request.Duracao, DateTime.UtcNow, "Recebido");

            var resultadoOperacao = new ResultadoOperacao<NovoVideoResponse>(response);

            _processarVideoUseCaseMock.Setup(x => x.EnviarVideoParaProcessamentoAsync(request))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await _controller.Post(request) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(response);
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequest_QuandoProcessamentoFalhar()
        {
            // Arrange
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", "video.mp4", "mp4", 1000, 120);
            var resultadoOperacao = new ResultadoOperacao<NovoVideoResponse>("Falha ao processar vídeo.", true);

            _processarVideoUseCaseMock.Setup(x => x.EnviarVideoParaProcessamentoAsync(request))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await _controller.Post(request) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var errorResponse = resultado.Value as ErrorResponse;
            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().Be("Falha ao processar vídeo.");
        }

        [Fact]
        public async Task Post_DeveRetornarInternalServerError_QuandoExcecaoForLancada()
        {
            // Arrange
            var request = new NovoVideoRequest(Guid.NewGuid(), "teste@teste.com", "video.mp4", "mp4", 1000, 120);
            _processarVideoUseCaseMock.Setup(x => x.EnviarVideoParaProcessamentoAsync(request))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var resultado = await _controller.Post(request) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var errorResponse = resultado.Value as ErrorResponse;
            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().Be("Erro interno");
        }

        #endregion

        #region ObterVideosPorUsuario

        [Fact]
        public async Task ObterVideosPorUsuario_DeveRetornarOk_QuandoVideosForemEncontrados()
        {
            // Arrange
            var idUsuario = Guid.NewGuid();
            var videos = new List<VideoResponse>
            {
                new(
                    Guid.NewGuid(), idUsuario, "video1.mp4", "mp4", 1000, 120, DateTime.UtcNow, "Finalizado",
                    [
                        new("Recebido", DateTime.UtcNow.AddMinutes(-10)),
                        new("Finalizado", DateTime.UtcNow)
                    ]
                ),
                new(
                    Guid.NewGuid(), idUsuario, "video2.mp4", "mp4", 2000, 240, DateTime.UtcNow, "Processando",
                    [
                        new("Recebido", DateTime.UtcNow.AddMinutes(-5)),
                        new("Processando", DateTime.UtcNow)
                    ]
                )
            };

            var resultadoOperacao = new ResultadoOperacao<IReadOnlyList<VideoResponse>>(videos);

            _buscarVideosUseCaseMock.Setup(x => x.ObterVideosDoUsuarioAsync(idUsuario))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await _controller.ObterVideosPorUsuario(idUsuario) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(videos);
        }

        [Fact]
        public async Task ObterVideosPorUsuario_DeveRetornarBadRequest_QuandoOperacaoFalhar()
        {
            // Arrange
            var idUsuario = Guid.NewGuid();
            var resultadoOperacao = new ResultadoOperacao<IReadOnlyList<VideoResponse>>("Falha ao obter vídeos.", true);

            _buscarVideosUseCaseMock.Setup(x => x.ObterVideosDoUsuarioAsync(idUsuario))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await _controller.ObterVideosPorUsuario(idUsuario) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var errorResponse = resultado.Value as ErrorResponse;
            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().Be("Falha ao obter vídeos.");
        }

        [Fact]
        public async Task ObterVideosPorUsuario_DeveRetornarInternalServerError_QuandoExcecaoForLancada()
        {
            // Arrange
            var idUsuario = Guid.NewGuid();
            _buscarVideosUseCaseMock.Setup(x => x.ObterVideosDoUsuarioAsync(idUsuario))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var resultado = await _controller.ObterVideosPorUsuario(idUsuario) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var errorResponse = resultado.Value as ErrorResponse;
            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().Be("Erro interno");
        }

        #endregion

        #region ObterLinksDeDownloadDoVideo

        [Fact]
        public async Task ObterLinksDeDownloadDoVideo_DeveRetornarOk_QuandoVideoForEncontrado()
        {
            // Arrange
            var idVideo = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();
            var linksDeDownloadResponse = new LinksDeDownloadResponse(
                idVideo,
                "https://bucket/zip.zip",
                "https://bucket/imagem.png",
                DateTime.Now);
            

            var resultadoOperacao = new ResultadoOperacao<LinksDeDownloadResponse>(linksDeDownloadResponse);

            _buscarVideosUseCaseMock.Setup(x => x.ObterLinksDeDownloadAsync(idVideo, idUsuario))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await _controller.ObterLinksDeDownloadDoVideo(idUsuario, idVideo) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(linksDeDownloadResponse);
        }

        [Fact]
        public async Task ObterLinksDeDownloadDoVideo_DeveRetornarBadRequest_QuandoOperacaoFalhar()
        {
            // Arrange
            var idVideo = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();
            var resultadoOperacao = new ResultadoOperacao<LinksDeDownloadResponse>("Falha ao obter vídeo.", true);

            _buscarVideosUseCaseMock.Setup(x => x.ObterLinksDeDownloadAsync(idVideo, idUsuario))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await _controller.ObterLinksDeDownloadDoVideo(idUsuario, idVideo) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var errorResponse = resultado.Value as ErrorResponse;
            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().Be("Falha ao obter vídeo.");
        }

        [Fact]
        public async Task ObterLinksDeDownloadDoVideo_DeveRetornarInternalServerError_QuandoExcecaoForLancada()
        {
            // Arrange
            var idVideo = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();
            _buscarVideosUseCaseMock.Setup(x => x.ObterLinksDeDownloadAsync(idVideo, idUsuario))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var resultado = await _controller.ObterLinksDeDownloadDoVideo(idUsuario, idVideo) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var errorResponse = resultado.Value as ErrorResponse;
            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().Be("Erro interno");
        }

        #endregion
    }
}
