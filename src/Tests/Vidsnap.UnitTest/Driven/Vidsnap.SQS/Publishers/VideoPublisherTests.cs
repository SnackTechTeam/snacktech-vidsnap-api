using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using Moq;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Domain.Entities;
using Vidsnap.SQS.Publishers;

namespace Vidsnap.UnitTest.Driven.Vidsnap.SQS.Publishers
{
    public class VideoPublisherTests
    {
        private readonly Mock<IAmazonSQS> _sqsClientMock;
        private readonly Mock<IOptions<QueuesSettings>> _queuesSettingsMock;
        private readonly VideoPublisher _videoPublisher;
        private readonly QueuesSettings _queuesSettings;

        public VideoPublisherTests()
        {
            _sqsClientMock = new Mock<IAmazonSQS>();
            _queuesSettings = new QueuesSettings
            {
                QueueEnviaNotificacaoURL = "https://sqs.us-east-1.amazonaws.com/123456789012/my-queue",
                MaxNumberOfMessages = 5
            };
            _queuesSettingsMock = new Mock<IOptions<QueuesSettings>>();
            _queuesSettingsMock.Setup(x => x.Value).Returns(_queuesSettings);

            _videoPublisher = new VideoPublisher(_sqsClientMock.Object, _queuesSettingsMock.Object);
        }

        [Fact]
        public async Task EnviarMensagemAsync_DeveChamarSendMessageAsync()
        {
            var video = new Video(Guid.NewGuid(), "email@email.com", "teste.mp4", "mp4", 1, 1);

            _sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), default))
                .ReturnsAsync(new SendMessageResponse());

            await _videoPublisher.PublicarProcessamentoFinalizadoAsync(video);

            _sqsClientMock.Verify(s => s.SendMessageAsync(
                It.Is<SendMessageRequest>(req => req.QueueUrl == _queuesSettings.QueueEnviaNotificacaoURL && req.MessageBody.Contains("email@email.com")), default),
                Times.Once);
        }
    }
}
