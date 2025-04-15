using Amazon.SQS.Model;
using Amazon.SQS;
using Microsoft.Extensions.Options;
using Moq;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Domain.DTOs.Queues;
using Vidsnap.SQS.QueueClient;
using System.Text.Json;
using FluentAssertions;

namespace Vidsnap.UnitTest.Driven.Vidsnap.SQS.QueueClient
{
    public class SqsMessageQueueTests
    {
        private readonly Mock<IAmazonSQS> _sqsClientMock;
        private readonly Mock<IOptions<QueuesSettings>> _queuesSettingsMock;
        private readonly SqsMessageQueue<TestMessage> _messageQueue;
        private readonly QueuesSettings _queuesSettings;

        public SqsMessageQueueTests()
        {
            _sqsClientMock = new Mock<IAmazonSQS>();
            _queuesSettings = new QueuesSettings
            {
                QueueAtualizaStatusURL = "https://sqs.us-east-1.amazonaws.com/123456789012/my-queue",
                DlqQueueAtualizaStatusURL = "https://sqs.us-east-1.amazonaws.com/123456789012/my-dlq",
                MaxNumberOfMessages = 5
            };
            _queuesSettingsMock = new Mock<IOptions<QueuesSettings>>();
            _queuesSettingsMock.Setup(x => x.Value).Returns(_queuesSettings);

            _messageQueue = new SqsMessageQueue<TestMessage>(_sqsClientMock.Object, _queuesSettingsMock.Object);
        }

        [Fact]
        public async Task ReceberMensagemAsync_DeveRetornarMensagensDeserializadas()
        {
            var testBody = new TestMessage { Content = "Mensagem de teste" };
            var message = new Message
            {
                Body = JsonSerializer.Serialize(testBody),
                ReceiptHandle = "receipt-handle"
            };

            _sqsClientMock.Setup(s => s.ReceiveMessageAsync(It.IsAny<ReceiveMessageRequest>(), default))
                .ReturnsAsync(new ReceiveMessageResponse { Messages = new List<Message> { message } });

            var result = await _messageQueue.ReceberMensagemAsync();

            result.Should().HaveCount(1);
            result[0].MessageBody.Content.Should().Be("Mensagem de teste");
            result[0].MessageIdentifier.Should().Be("receipt-handle");
        }

        [Fact]
        public async Task DeletarMensagemAsync_DeveChamarDeleteMessageAsync()
        {
            const string receiptHandle = "receipt-handle";

            _sqsClientMock.Setup(s => s.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), default))
                .ReturnsAsync(new DeleteMessageResponse());

            await _messageQueue.DeletarMensagemAsync(receiptHandle);

            _sqsClientMock.Verify(s => s.DeleteMessageAsync(
                It.Is<DeleteMessageRequest>(req => req.QueueUrl == _queuesSettings.QueueAtualizaStatusURL && req.ReceiptHandle == receiptHandle), default),
                Times.Once);
        }

        [Fact]
        public async Task MoverParaDlqAsync_DeveEnviarParaDlqEDeletarOriginal()
        {
            var testMessage = new TestMessage { Content = "Erro" };
            var queueMessage = new QueueMessage<TestMessage>(testMessage, "receipt-handle");

            _sqsClientMock.Setup(s => s.SendMessageAsync(It.IsAny<SendMessageRequest>(), default))
                .ReturnsAsync(new SendMessageResponse());

            _sqsClientMock.Setup(s => s.DeleteMessageAsync(It.IsAny<DeleteMessageRequest>(), default))
                .ReturnsAsync(new DeleteMessageResponse());

            await _messageQueue.MoverParaDlqAsync(queueMessage);

            _sqsClientMock.Verify(s => s.SendMessageAsync(
                It.Is<SendMessageRequest>(r => r.QueueUrl == _queuesSettings.DlqQueueAtualizaStatusURL), default), Times.Once);

            _sqsClientMock.Verify(s => s.DeleteMessageAsync(
                It.Is<DeleteMessageRequest>(r => r.ReceiptHandle == "receipt-handle"), default), Times.Once);
        }

        [Fact]
        public async Task DeletarMensagemAsync_DeveLancarExcecao_QuandoIdentificadorForInvalido()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _messageQueue.DeletarMensagemAsync(123));
        }

        [Fact]
        public async Task MoverParaDlqAsync_DeveLancarExcecao_QuandoIdentificadorForInvalido()
        {
            var mensagemInvalida = new QueueMessage<TestMessage>(new TestMessage { Content = "DLQ" }, 123);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                _messageQueue.MoverParaDlqAsync(mensagemInvalida));
        }

        private class TestMessage
        {
            public string Content { get; set; } = string.Empty;
        }
    }
}
