using FluentAssertions;
using Moq;
using Vidsnap.Application.UseCases;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.UnitTest.Core.Vidsnap.Application.UseCases
{
    public class BuscarVideoUseCaseTests
    {
        private readonly Mock<IVideoRepository> _videoRepositoryMock;
        private readonly BuscarVideosUseCase _buscarVideoUseCase;

        public BuscarVideoUseCaseTests()
        {
            _videoRepositoryMock = new Mock<IVideoRepository>();
            _buscarVideoUseCase = new BuscarVideosUseCase(_videoRepositoryMock.Object);
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
            var resultado = await _buscarVideoUseCase.ObterVideosDoUsuarioAsync(idUsuario);

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
            var resultado = await _buscarVideoUseCase.ObterVideosDoUsuarioAsync(Guid.NewGuid());

            //arrange
            resultado.Sucesso.Should().BeFalse();
            resultado.Excecao.Message.Should().Be("Erro interno!");
        }

        #endregion
    }
}
