using FluentAssertions;
using Microsoft.Extensions.Options;
using Moq;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Application.UseCases;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.UnitTest.Core.Vidsnap.Application.UseCases
{
    public class ProcessarVideoUseCaseTests
    {
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly Mock<ICloudFileStorageService> _cloudFileStorageServiceMock;
        private readonly Mock<IOptions<CloudFileStorageSettings>> _cloudFileStorageOptionMock;
        private readonly ProcessarVideoUseCase _processarVideoUseCase;
        private readonly CloudFileStorageSettings _cloudFileStorageSettings;

        public ProcessarVideoUseCaseTests()
        {
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _cloudFileStorageServiceMock = new Mock<ICloudFileStorageService>();
            _cloudFileStorageOptionMock = new Mock<IOptions<CloudFileStorageSettings>>();

            _cloudFileStorageSettings = new CloudFileStorageSettings
            {
                ContainerName = "test-container",
                TimeoutDuration = 1
            };

            _cloudFileStorageOptionMock.Setup(opt => opt.Value).Returns(_cloudFileStorageSettings);

            _processarVideoUseCase = new ProcessarVideoUseCase(
                _videoRepositoryMock.Object,
                _cloudFileStorageServiceMock.Object,
                _cloudFileStorageOptionMock.Object);
        }

        #region EnviarVideoParaProcessamentoAsync
        [Fact]
        public async Task EnviarVideoParaProcessamentoAsync_DeveRetornarNovoVideoResponse_QuandoExecutadoComSucesso()
        {
            // Arrange
            var request = new NovoVideoRequest(
                Guid.NewGuid(), "teste@teste.com", "video-teste", "mp4", 1000, 120
            );

            var presignedUrl = "https://storage.com/upload-url";

            _videoRepositoryMock.Setup(repo => repo.CriarAsync(It.IsAny<Video>()))
                .Returns(Task.CompletedTask);

            _cloudFileStorageServiceMock.Setup(service => service.GetUploadPreSignedURLAsync(
                    It.IsAny<string>(),
                    It.IsAny<int>(),
                    It.IsAny<Guid>(),
                    It.IsAny<Guid?>(),
                    It.IsAny<string>()))
                .ReturnsAsync(presignedUrl);

            // Act
            var resultado = await _processarVideoUseCase.EnviarVideoParaProcessamentoAsync(request);

            // Assert
            resultado.Sucesso.Should().BeTrue();
            resultado.Dados.Should().NotBeNull();
            resultado.Dados.UrlPreAssinadaDeUpload.Should().Be(presignedUrl);
        }

        [Fact]
        public async Task EnviarVideoParaProcessamentoAsync_DeveRetornarErro_QuandoExcecaoForLancada()
        {
            // Arrange
            var request = new NovoVideoRequest(
                Guid.NewGuid(), "teste@teste.com", "video-teste", "mp4", 1000, 120
            );

            _videoRepositoryMock.Setup(repo => repo.CriarAsync(It.IsAny<Video>()))
                .ThrowsAsync(new Exception("Erro ao salvar vídeo"));

            // Act
            var resultado = await _processarVideoUseCase.EnviarVideoParaProcessamentoAsync(request);

            // Assert
            resultado.Sucesso.Should().BeFalse();
            resultado.Excecao.Should().NotBeNull();
            resultado.Excecao.Message.Should().Be("Erro ao salvar vídeo");
        }
        #endregion
    }
}
