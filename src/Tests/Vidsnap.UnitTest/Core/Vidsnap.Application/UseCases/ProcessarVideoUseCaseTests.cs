using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Application.UseCases;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.UnitTest.Core.Vidsnap.Application.UseCases
{
    public class ProcessarVideoUseCaseTests
    {
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<ICloudFileStorageService> _cloudFileStorageServiceMock;
        private readonly Mock<IOptions<CloudFileStorageSettings>> _cloudFileStorageOptionMock;
        private readonly ProcessarVideoUseCase _processarVideoUseCase;

        public ProcessarVideoUseCaseTests()
        {
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _cloudFileStorageServiceMock = new Mock<ICloudFileStorageService>();
            _cloudFileStorageOptionMock = new Mock<IOptions<CloudFileStorageSettings>>();
            
            _processarVideoUseCase = new ProcessarVideoUseCase(
                _videoRepositoryMock.Object,
                _cloudFileStorageServiceMock.Object,
                _cloudFileStorageOptionMock.Object);
        }

        #region AtualizarStatusDeProcessamentoAsync

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_QuandoStatusNaoForFinal_DeveAtualizarApenasStatus()
        {
            //arrange
            var atualizaStatusVideoRequest = CriarAtualizarVideoRequestValido("Processando");

            var video = CriarVideoValido();

            _videoRepositoryMock.Setup(v => v.ObterPorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(video);

            _videoRepositoryMock.Setup(v => v.AtualizarStatusProcessamentoAsync(It.IsAny<Video>(), It.IsAny<Status>()))
                .ReturnsAsync(1);            

            //act
            var resultado = await _processarVideoUseCase.AtualizarStatusDeProcessamentoAsync(atualizaStatusVideoRequest.IdVideo, atualizaStatusVideoRequest);

            //assert
            resultado.Sucesso.Should().BeTrue();
            video.URLZipe.Should().BeNull();
            video.StatusAtual.Should().Be(Status.Processando);
            video.VideoStatuses.Should().HaveCount(2);
        }

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_QuandoStatusForFinal_DeveAtualizarStatusEUrlZip()
        {
            //arrange
            var atualizaStatusVideoRequest = CriarAtualizarVideoRequestValido("FinalizadoComSucesso", true);

            var video = CriarVideoValido();

            _videoRepositoryMock.Setup(v => v.ObterPorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(video);

            _videoRepositoryMock.Setup(v => v.AtualizarStatusProcessamentoAsync(It.IsAny<Video>(), It.IsAny<Status>()))
                .ReturnsAsync(1);

            //act
            var resultado = await _processarVideoUseCase.AtualizarStatusDeProcessamentoAsync(atualizaStatusVideoRequest.IdVideo, atualizaStatusVideoRequest);

            //assert
            resultado.Sucesso.Should().BeTrue();
            video.URLZipe.Should().Be(atualizaStatusVideoRequest.UrlZip);
            video.StatusAtual.Should().Be(Status.FinalizadoComSucesso);
            video.VideoStatuses.Should().HaveCount(2);
        }

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_QuandoNaoEncontrarOVideo_DeveRetornarErro()
        {
            //arrange
            var atualizaStatusVideoRequest = CriarAtualizarVideoRequestValido("Processando");

            _videoRepositoryMock.Setup(v => v.ObterPorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Video?)null);

            //act
            var resultado = await _processarVideoUseCase.AtualizarStatusDeProcessamentoAsync(atualizaStatusVideoRequest.IdVideo, atualizaStatusVideoRequest);

            //assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Be("Video não encontrado!");
        }

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_QuandoStatusJaExiste_DeveRetornarErro()
        {
            //arrange
            var idVideo = Guid.NewGuid();
            var atualizaStatusVideoRequest = CriarAtualizarVideoRequestValido("Processando", true);

            var video = CriarVideoValido();

            video.AtualizarStatus(Status.Processando);

            _videoRepositoryMock.Setup(v => v.ObterPorIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(video);

            //act
            var resultado = await _processarVideoUseCase.AtualizarStatusDeProcessamentoAsync(idVideo, atualizaStatusVideoRequest);

            //assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Be($"O vídeo {idVideo} já chegou no status {Status.Processando}");
        }

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_StatusForInvalido_DeveRetornarErro()
        {
            //arrange
            var atualizaStatusVideoRequest = CriarAtualizarVideoRequestValido("StatusInvalido");

            //act
            var resultado = await _processarVideoUseCase.AtualizarStatusDeProcessamentoAsync(atualizaStatusVideoRequest.IdVideo, atualizaStatusVideoRequest);

            //assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Mensagem.Should().Be("Status inválido!");
        }

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_QuandoErroInternoOcorrer_DeveRetornarErroComException()
        {
            //arrange
            var atualizaStatusVideoRequest = CriarAtualizarVideoRequestValido("Processando");

            _videoRepositoryMock.Setup(v => v.ObterPorIdAsync(It.IsAny<Guid>()))
                .ThrowsAsync(new Exception("Erro interno!"));

            //act
            var result = await _processarVideoUseCase.AtualizarStatusDeProcessamentoAsync(atualizaStatusVideoRequest.IdVideo, atualizaStatusVideoRequest);

            //assert
            result.Sucesso.Should().BeFalse();
            result.Excecao.Should().NotBeNull();
        }

        #endregion

        #region Private Methods

        private static AtualizaStatusVideoRequest CriarAtualizarVideoRequestValido(string status, bool withUrl = false)
        {
            return new(Guid.NewGuid(), status, withUrl ? "http://teste" : null);
        }

        private static Video CriarVideoValido()
        {
            return new Video(Guid.NewGuid(),
                "teste@email.com",
                "video",
                "abc",
                1,
                1
            );
        }

        #endregion
    }
}
