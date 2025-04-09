using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Application.UseCases;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.UnitTest.Core.Vidsnap.Application.UseCases
{
    public class BuscarVideoUseCaseTests
    {
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly BuscarVideosUseCase _buscarVideosUseCase;
        private readonly Mock<ICloudFileStorageService> _cloudFileStorageServiceMock;
        private readonly Mock<IOptions<CloudFileStorageSettings>> _cloudFileStorageOptionMock;
        private readonly CloudFileStorageSettings _settings;

        public BuscarVideoUseCaseTests()
        {
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _cloudFileStorageServiceMock = new Mock<ICloudFileStorageService>();
            _cloudFileStorageOptionMock = new Mock<IOptions<CloudFileStorageSettings>>();

            _settings = new CloudFileStorageSettings
            {
                TimeoutDuration = 10
            };

            _cloudFileStorageOptionMock.Setup(opt => opt.Value).Returns(_settings);

            _buscarVideosUseCase = new BuscarVideosUseCase(
                _videoRepositoryMock.Object, 
                _cloudFileStorageServiceMock.Object,
                _cloudFileStorageOptionMock.Object);
        }

        #region ObterVideosDoUsuarioAsync
        
        [Fact]
        public async Task ObterVideosDoUsuarioAsync_QuandoIdUsuarioExistir_DeveRetornarListaDeVideos()
        {
            //arrange
            var idUsuario = Guid.NewGuid();
            List<Video> videos = [
                new Video(idUsuario, "email@email.com.br", "video1", "abc", 1, 1),
                new Video(idUsuario, "email@email.com.br", "video2", "abc", 1, 1)                
            ];

            _videoRepositoryMock.Setup(v => v.ObterTodosDoUsuarioAsync(It.IsAny<Guid>()))
                .ReturnsAsync(videos);

            //act
            var resultado = await _buscarVideosUseCase.ObterVideosDoUsuarioAsync(idUsuario);

            //arrange
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().HaveCount(2);
        }

        [Fact]
        public async Task ObterVideosDoUsuarioAsync_QuandoOcorrerErroInterno_DeveRetornarComException()
        {
            //arrange
            _videoRepositoryMock.Setup(v => v.ObterTodosDoUsuarioAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Erro interno!"));

            //act
            var resultado = await _buscarVideosUseCase.ObterVideosDoUsuarioAsync(Guid.NewGuid());

            //arrange
            resultado.Sucesso.Should().BeFalse();
            resultado.Excecao.Message.Should().Be("Erro interno!");
        }

        #endregion

        #region ObterLinksDeDownloadAsync

        [Fact]
        public async Task ObterLinksDeDownloadAsync_DeveRetornarErro_QuandoVideoNaoEncontrado()
        {
            // Arrange
            var idVideo = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            _videoRepositoryMock.Setup(repo => repo.ObterPorUsuarioAsync(idVideo, idUsuario))
                .ReturnsAsync((Video?)null);

            // Act
            var resultado = await _buscarVideosUseCase.ObterLinksDeDownloadAsync(idVideo, idUsuario);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("não encontrado");
        }

        [Fact]
        public async Task ObterLinksDeDownloadAsync_DeveRetornarErro_QuandoStatusDiferenteDeFinalizado()
        {
            //Arrange
            var video = new Video(
                Guid.NewGuid(), 
                "email@email.com.br", 
                "video1", 
                "abc", 
                1, 
                1
            );

            _videoRepositoryMock.Setup(repo => repo.ObterPorUsuarioAsync(video.Id, video.IdUsuario))
            .ReturnsAsync(video);

            //Act
            var resultado = await _buscarVideosUseCase.ObterLinksDeDownloadAsync(video.Id, video.IdUsuario);

            //Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("não é finalizado com sucesso!");
        }

        [Fact]
        public async Task ObterLinksDeDownloadAsync_DeveRetornarErro_QuandoLinkZipInvalido()
        {
            //Arrange
            var video = new Video(
                Guid.NewGuid(), 
                "email@email.com.br", 
                "video1", 
                "abc", 
                1, 
                1
            );

            video.AtualizarStatus(Status.FinalizadoComSucesso);
            video.IncluirURLs("s3://teste.zip", "s3://teste/teste.jpg");

            _videoRepositoryMock.Setup(repo => repo.ObterPorUsuarioAsync(video.Id, video.IdUsuario))
            .ReturnsAsync(video);

            //Act
            var resultado = await _buscarVideosUseCase.ObterLinksDeDownloadAsync(video.Id, video.IdUsuario);

            //Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Link do zip do video");
        }

        [Fact]
        public async Task ObterLinksDeDownloadAsync_DeveRetornarErro_QuandoLinkImagemInvalido()
        {
            //Arrange
            var video = new Video(
                Guid.NewGuid(),
                "email@email.com.br",
                "video1",
                "abc",
                1,
                1
            );

            video.AtualizarStatus(Status.FinalizadoComSucesso);
            video.IncluirURLs("s3://teste/teste.zip", "s3://teste.jpg");

            _videoRepositoryMock.Setup(repo => repo.ObterPorUsuarioAsync(video.Id, video.IdUsuario))
            .ReturnsAsync(video);

            //Act
            var resultado = await _buscarVideosUseCase.ObterLinksDeDownloadAsync(video.Id, video.IdUsuario);

            //Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("Link da imagem do video ");
        }

        [Fact]
        public async Task ObterLinksDeDownloadAsync_DeveRetornarLinks_QuandoVideoFinalizadoComLinksValidos()
        {
            //Arrange
            var video = new Video(
                Guid.NewGuid(),
                "email@email.com.br",
                "video1",
                "abc",
                1,
                1
            );

            video.AtualizarStatus(Status.FinalizadoComSucesso);
            video.IncluirURLs("s3://bucket/zip.zip", "s3://bucket/image.png");

            _videoRepositoryMock.Setup(repo => repo.ObterPorUsuarioAsync(video.Id, video.IdUsuario))
            .ReturnsAsync(video);

            _cloudFileStorageServiceMock.Setup(service => service.GetDownloadPreSignedURLAsync("bucket", _settings.TimeoutDuration, "zip.zip"))
            .ReturnsAsync("https://s3/bucket/zip.zip");

            _cloudFileStorageServiceMock.Setup(service => service.GetDownloadPreSignedURLAsync("bucket", _settings.TimeoutDuration, "image.png"))
            .ReturnsAsync("https://s3/bucket/image.png");

            //Act
            var resultado = await _buscarVideosUseCase.ObterLinksDeDownloadAsync(video.Id, video.IdUsuario);

            //Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.URLZip.Should().Be("https://s3/bucket/zip.zip");
            resultado.Dados.URLImagem.Should().Be("https://s3/bucket/image.png");
        }

        [Fact]
        public async Task ObterLinksDeDownloadAsync_DeveRetornarErro_QuandoVideoNaoTemLinks()
        {
            //Arrange
            var video = new Video(
                Guid.NewGuid(),
                "email@email.com.br",
                "video1",
                "abc",
                1,
                1
            );

            video.AtualizarStatus(Status.FinalizadoComSucesso);

            _videoRepositoryMock.Setup(repo => repo.ObterPorUsuarioAsync(video.Id, video.IdUsuario))
            .ReturnsAsync(video);

            //Act
            var resultado = await _buscarVideosUseCase.ObterLinksDeDownloadAsync(video.Id, video.IdUsuario);

            //Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Contain("sem zip ou imagem");
        }

        [Fact]
        public async Task ObterLinksDeDownloadAsync_DeveRetornarErro_QuandoExcecaoForLancada()
        {
            // Arrange
            var idVideo = Guid.NewGuid();
            var idUsuario = Guid.NewGuid();

            var exception = new Exception("Erro inesperado");

            _videoRepositoryMock
                .Setup(r => r.ObterPorUsuarioAsync(idVideo, idUsuario))
                .ThrowsAsync(exception);

            // Act
            var resultado = await _buscarVideosUseCase.ObterLinksDeDownloadAsync(idVideo, idUsuario);

            // Assert
            resultado.TeveSucesso().Should().BeFalse();
            resultado.TeveExcecao().Should().BeTrue();
            resultado.Excecao.Should().Be(exception);
            resultado.Mensagem.Should().Be(exception.Message);
        }

        #endregion
    }
}