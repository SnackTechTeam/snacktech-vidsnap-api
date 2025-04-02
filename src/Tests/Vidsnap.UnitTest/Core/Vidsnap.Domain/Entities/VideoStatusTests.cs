using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Enums;

namespace Vidsnap.UnitTest.Core.Vidsnap.Domain.Entities
{
    public class VideoStatusTests
    {
        [Fact]
        public void Construtor_DeveInicializarPropriedadesCorretamente()
        {
            // Arrange
            var status = Status.Recebido;
            var idVideo = Guid.NewGuid();

            // Act
            var videoStatus = new VideoStatus(status, idVideo);

            // Assert
            Assert.Equal(status, videoStatus.Status);
            Assert.Equal(idVideo, videoStatus.IdVideo);
            Assert.Null(videoStatus.Video);
            Assert.True(videoStatus.DataInclusao <= DateTime.Now);
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_ParaStatusInvalido()
        {
            // Arrange
            var statusInvalido = (Status)999;
            var idVideo = Guid.NewGuid();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new VideoStatus(statusInvalido, idVideo));
        }

        [Fact]
        public void Construtor_DeveLancarExcecao_ParaIdVideoInvalido()
        {
            // Arrange
            var status = Status.Recebido;
            var idVideoInvalido = Guid.Empty;

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new VideoStatus(status, idVideoInvalido));
        }
    }
}
