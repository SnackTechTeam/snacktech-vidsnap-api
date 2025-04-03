using Amazon.S3;
using Amazon.S3.Model;
using FluentAssertions;
using Moq;
using Vidsnap.S3Bucket.Services;

namespace Vidsnap.UnitTest.Driven.Vidsnap.S3Bucket.Services
{
    public class S3ServiceTests
    {
        private readonly Mock<IAmazonS3> _s3ClientMock;
        private readonly S3Service _s3Service;

        public S3ServiceTests()
        {
            _s3ClientMock = new Mock<IAmazonS3>();
            _s3Service = new S3Service(_s3ClientMock.Object);
        }

        [Fact]
        public async Task GetUploadPreSignedURLAsync_DeveRetornarURL_QuandoExecutadoComSucesso()
        {
            // Arrange
            var storageName = "test-bucket";
            var timeoutDuration = 15;
            var idUsuario = Guid.NewGuid();
            var idVideo = Guid.NewGuid();
            var fileName = "video.mp4";
            var expectedUrl = "https://s3.amazonaws.com/test-bucket/video.mp4";

            _s3ClientMock.Setup(s3 => s3.GetPreSignedURLAsync(It.IsAny<GetPreSignedUrlRequest>()))
                .ReturnsAsync(expectedUrl);

            // Act
            var result = await _s3Service.GetUploadPreSignedURLAsync(storageName, timeoutDuration, idUsuario, idVideo, fileName);

            // Assert
            result.Should().Be(expectedUrl);
            _s3ClientMock.Verify(s3 => s3.GetPreSignedURLAsync(It.Is<GetPreSignedUrlRequest>(r =>
                r.BucketName == storageName &&
                r.Verb == HttpVerb.PUT &&
                r.Key == $"{idUsuario}/{idVideo}/{fileName}" &&
                r.Expires > DateTime.UtcNow)), Times.Once);
        }

        [Fact]
        public async Task GetDownloadPreSignedURLAsync_DeveRetornarURL_QuandoExecutadoComSucesso()
        {
            // Arrange
            var storageName = "test-bucket";
            var timeoutDuration = 15;
            var idUsuario = Guid.NewGuid();
            var idVideo = Guid.NewGuid();
            var fileName = "video.mp4";
            var expectedUrl = "https://s3.amazonaws.com/test-bucket/video.mp4";

            _s3ClientMock.Setup(s3 => s3.GetPreSignedURLAsync(It.IsAny<GetPreSignedUrlRequest>()))
                .ReturnsAsync(expectedUrl);

            // Act
            var result = await _s3Service.GetDownloadPreSignedURLAsync(storageName, timeoutDuration, idUsuario, idVideo, fileName);

            // Assert
            result.Should().Be(expectedUrl);
            _s3ClientMock.Verify(s3 => s3.GetPreSignedURLAsync(It.Is<GetPreSignedUrlRequest>(r =>
                r.BucketName == storageName &&
                r.Verb == HttpVerb.PUT &&
                r.Key == $"{idUsuario}/{idVideo}/{fileName}" &&
                r.Expires > DateTime.UtcNow)), Times.Once);
        }
    }
}
