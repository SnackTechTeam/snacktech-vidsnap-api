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
        private readonly Mock<ILogger<IProcessarVideoUseCase>> _loggerMock; // Logger type should match controller's logger
        private readonly VideosController _controller;

        public VideosControllerTests()
        {
            _processarVideoUseCaseMock = new Mock<IProcessarVideoUseCase>();
            _buscarVideosUseCaseMock = new Mock<IBuscarVideosUseCase>();
            // Note: The logger type in the controller is ILogger<IProcessarVideoUseCase>.
            // If CustomBaseController uses a different logger type, adjust this mock accordingly.
            // For now, assuming it uses the same type or a base type compatible with ILogger<IProcessarVideoUseCase>.
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
            var idUsuario = Guid.NewGuid();
            var emailUsuario = "teste@teste.com";
            var requestBody = new NovoVideoBodyRequest("video.mp4", "mp4", 1000, 120);

            // This is the object the use case expects
            var expectedUseCaseRequest = new NovoVideoRequest(idUsuario, emailUsuario, requestBody.NomeVideo, requestBody.Extensao, requestBody.Tamanho, requestBody.Duracao);

            var response = new NovoVideoResponse(Guid.NewGuid(), idUsuario, requestBody.NomeVideo, requestBody.Extensao, requestBody.Tamanho, requestBody.Duracao, DateTime.UtcNow, "Recebido");
            var resultadoOperacao = new ResultadoOperacao<NovoVideoResponse>(response);

            _processarVideoUseCaseMock.Setup(x => x.EnviarVideoParaProcessamentoAsync(It.Is<NovoVideoRequest>(req =>
                    req.IdUsuario == idUsuario &&
                    req.EmailUsuario == emailUsuario &&
                    req.NomeVideo == requestBody.NomeVideo &&
                    req.Extensao == requestBody.Extensao &&
                    req.Tamanho == requestBody.Tamanho &&
                    req.Duracao == requestBody.Duracao)))
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await _controller.Post(idUsuario, emailUsuario, requestBody) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(response);
            _processarVideoUseCaseMock.Verify(x => x.EnviarVideoParaProcessamentoAsync(It.IsAny<NovoVideoRequest>()), Times.Once);
        }

        [Fact]
        public async Task Post_DeveRetornarBadRequest_QuandoProcessamentoFalhar()
        {
            // Arrange
            var idUsuario = Guid.NewGuid();
            var emailUsuario = "teste@teste.com";
            var requestBody = new NovoVideoBodyRequest("video.mp4", "mp4", 1000, 120);

            var expectedUseCaseRequest = new NovoVideoRequest(idUsuario, emailUsuario, requestBody.NomeVideo, requestBody.Extensao, requestBody.Tamanho, requestBody.Duracao);

            var resultadoOperacao = new ResultadoOperacao<NovoVideoResponse>("Falha ao processar vídeo.", true);

            _processarVideoUseCaseMock.Setup(x => x.EnviarVideoParaProcessamentoAsync(It.Is<NovoVideoRequest>(req =>
                    req.IdUsuario == idUsuario &&
                    req.EmailUsuario == emailUsuario &&
                    req.NomeVideo == requestBody.NomeVideo))) // Simplified match for brevity
                .ReturnsAsync(resultadoOperacao);

            // Act
            var resultado = await _controller.Post(idUsuario, emailUsuario, requestBody) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.BadRequest);
            var errorResponse = resultado.Value as ErrorResponse;
            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().Be("Falha ao processar vídeo.");
            _processarVideoUseCaseMock.Verify(x => x.EnviarVideoParaProcessamentoAsync(It.IsAny<NovoVideoRequest>()), Times.Once);
        }

        [Fact]
        public async Task Post_DeveRetornarInternalServerError_QuandoExcecaoForLancada()
        {
            // Arrange
            var idUsuario = Guid.NewGuid();
            var emailUsuario = "teste@teste.com";
            var requestBody = new NovoVideoBodyRequest("video.mp4", "mp4", 1000, 120);

            var expectedUseCaseRequest = new NovoVideoRequest(idUsuario, emailUsuario, requestBody.NomeVideo, requestBody.Extensao, requestBody.Tamanho, requestBody.Duracao);

            _processarVideoUseCaseMock.Setup(x => x.EnviarVideoParaProcessamentoAsync(It.Is<NovoVideoRequest>(req =>
                    req.IdUsuario == idUsuario &&
                    req.EmailUsuario == emailUsuario &&
                    req.NomeVideo == requestBody.NomeVideo))) // Simplified match for brevity
                .ThrowsAsync(new Exception("Erro interno"));

            // Act
            var resultado = await _controller.Post(idUsuario, emailUsuario, requestBody) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.InternalServerError);
            var errorResponse = resultado.Value as ErrorResponse;
            errorResponse.Should().NotBeNull();
            errorResponse!.Message.Should().Be("Erro interno");
            _processarVideoUseCaseMock.Verify(x => x.EnviarVideoParaProcessamentoAsync(It.IsAny<NovoVideoRequest>()), Times.Once);
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
            // Call the controller method with the idUsuario (simulating header binding)
            var resultado = await _controller.ObterVideosPorUsuario(idUsuario) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(videos);
            _buscarVideosUseCaseMock.Verify(x => x.ObterVideosDoUsuarioAsync(idUsuario), Times.Once);
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
            _buscarVideosUseCaseMock.Verify(x => x.ObterVideosDoUsuarioAsync(idUsuario), Times.Once);
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
            _buscarVideosUseCaseMock.Verify(x => x.ObterVideosDoUsuarioAsync(idUsuario), Times.Once);
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
            // Call the controller method with idUsuario (header) and idVideo (route)
            var resultado = await _controller.ObterLinksDeDownloadDoVideo(idUsuario, idVideo) as ObjectResult;

            // Assert
            resultado.Should().NotBeNull();
            resultado!.StatusCode.Should().Be((int)HttpStatusCode.OK);
            resultado.Value.Should().BeEquivalentTo(linksDeDownloadResponse);
            _buscarVideosUseCaseMock.Verify(x => x.ObterLinksDeDownloadAsync(idVideo, idUsuario), Times.Once);
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
            _buscarVideosUseCaseMock.Verify(x => x.ObterLinksDeDownloadAsync(idVideo, idUsuario), Times.Once);
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
            _buscarVideosUseCaseMock.Verify(x => x.ObterLinksDeDownloadAsync(idVideo, idUsuario), Times.Once);
        }

        #endregion
    }
}
