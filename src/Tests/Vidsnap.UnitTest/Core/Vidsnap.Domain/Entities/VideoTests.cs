using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Enums;

namespace Vidsnap.UnitTest.Core.Vidsnap.Domain.Entities
{
    public class VideoTests
    {
        [Fact]
        public void Construtor_DeveIniciarPropriedades()
        {
            // Arrange
            var idUsuario = Guid.NewGuid();
            var emailUsuario = "test@example.com";
            var nomeVideo = "video.mp4";
            var extensao = "mp4";
            var tamanho = 1000;
            var duracao = 300;

            // Act
            var video = new Video(idUsuario, emailUsuario, nomeVideo, extensao, tamanho, duracao);

            // Assert
            Assert.Equal(idUsuario, video.IdUsuario);
            Assert.Equal(emailUsuario, video.EmailUsuario);
            Assert.Equal(nomeVideo, video.NomeVideo);
            Assert.Equal(extensao, video.Extensao);
            Assert.Equal(tamanho, video.Tamanho);
            Assert.Equal(duracao, video.Duracao);
            Assert.Equal(Status.Recebido, video.StatusAtual);
            Assert.NotNull(video.VideoStatuses);
            Assert.Single(video.VideoStatuses);
        }

        [Fact]
        public void Construtor_DeveLancarExceptionParaParametrosInvalidos()
        {
            // Arrange
            var invalidGuid = Guid.Empty;
            var invalidEmail = "invalid-email";
            var emptyString = "";
            var negativeValue = -1;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new Video(invalidGuid, "test@example.com", "video.mp4", "mp4", 1000, 300));
            Assert.Throws<ArgumentException>(() => new Video(Guid.NewGuid(), invalidEmail, "video.mp4", "mp4", 1000, 300));
            Assert.Throws<ArgumentException>(() => new Video(Guid.NewGuid(), "test@example.com", emptyString, "mp4", 1000, 300));
            Assert.Throws<ArgumentException>(() => new Video(Guid.NewGuid(), "test@example.com", "video.mp4", emptyString, 1000, 300));
            Assert.Throws<ArgumentException>(() => new Video(Guid.NewGuid(), "test@example.com", "video.mp4", "mp4", negativeValue, 300));
            Assert.Throws<ArgumentException>(() => new Video(Guid.NewGuid(), "test@example.com", "video.mp4", "mp4", 1000, negativeValue));
        }

        [Fact]
        public void AtualizarStatus_DeveAdicionarNovoStatus()
        {
            // Arrange
            var video = new Video(Guid.NewGuid(), "test@example.com", "video.mp4", "mp4", 1000, 300);
            var novoStatus = Status.Processando;

            // Act
            video.AtualizarStatus(novoStatus);

            // Assert
            Assert.Equal(novoStatus, video.StatusAtual);
            Assert.Equal(2, video.VideoStatuses.Count);
            Assert.Contains(video.VideoStatuses, vs => vs.Status == novoStatus);
        }

        [Fact]
        public void AtualizarStatus_DeveLancarExceptionParaParametrosInvalidos()
        {
            // Arrange
            var video = new Video(Guid.NewGuid(), "test@example.com", "video.mp4", "mp4", 1000, 300);
            var invalidStatus = (Status)999;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => video.AtualizarStatus(invalidStatus));
        }

        [Fact]
        public void IncluirURLs_DeveDefinirUrlsQuandoStatusEFinalizadoComSucesso()
        {
            // Arrange
            var video = new Video(Guid.NewGuid(), "test@example.com", "video.mp4", "mp4", 1000, 300);
            video.AtualizarStatus(Status.FinalizadoComSucesso);
            var urlZip = "http://example.com/video.zip";
            var urlImagem = "http://example.com/thumb.jpg";

            // Act
            video.IncluirURLs(urlZip, urlImagem);

            // Assert
            Assert.Equal(urlZip, video.URLZip);
            Assert.Equal(urlImagem, video.URLImagem);
        }

        [Fact]
        public void IncluirURLs_DeveLancarExceptionSeUrlsJaExistem()
        {
            // Arrange
            var video = new Video(Guid.NewGuid(), "test@example.com", "video.mp4", "mp4", 1000, 300);
            video.AtualizarStatus(Status.FinalizadoComSucesso);
            video.IncluirURLs("http://example.com/video.zip", "http://example.com/thumb.jpg");

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => video.IncluirURLs("http://new.com/video.zip", "http://new.com/thumb.jpg"));
            Assert.Contains("URLs já foram definidas", ex.Message);
        }

        [Fact]
        public void IncluirURLs_DeveLancarExceptionSeStatusDiferenteDeFinalizadoComSucesso()
        {
            // Arrange
            var video = new Video(Guid.NewGuid(), "test@example.com", "video.mp4", "mp4", 1000, 300);
            var urlZip = "http://example.com/video.zip";
            var urlImagem = "http://example.com/thumb.jpg";

            // Act & Assert
            var ex = Assert.Throws<InvalidOperationException>(() => video.IncluirURLs(urlZip, urlImagem));
            Assert.Contains("apenas podem ser definidas quando StatusAtual for igual a FinalizadoComSucesso", ex.Message);
        }
    }
}
