using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using Moq;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.UseCases;
using Vidsnap.Domain.DTOs.Queues;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.UnitTest.Core.Vidsnap.Application.UseCases
{
    public class AtualizarStatusVideoUseCaseTests
    {
        private readonly Mock<IMessageQueueService<AtualizaStatusVideoRequest>> _messageQueueServiceMock = new();
        private readonly Mock<IVideoPublisher> _videoPublisherMock = new();
        private readonly Mock<IVideoRepository> _videoRepositoryMock = new();
        private readonly Mock<IValidator<AtualizaStatusVideoRequest>> _validatorMock = new();
        private readonly Mock<ILogger<AtualizarStatusVideoUseCase>> _loggerMock = new();
        private readonly AtualizarStatusVideoUseCase _useCase;

        public AtualizarStatusVideoUseCaseTests()
        {
            _useCase = new AtualizarStatusVideoUseCase(
                _messageQueueServiceMock.Object,
                _videoPublisherMock.Object,
                _videoRepositoryMock.Object,
                _validatorMock.Object,
                _loggerMock.Object
            );
        }

        #region AtualizarStatusDeProcessamentoAsync

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_DeveAtualizarStatusDoVideo_QuandoValido()
        {
            // Arrange
            var request = CriaRequestValido();
            var queueMessage = new QueueMessage<AtualizaStatusVideoRequest>(request, Guid.NewGuid());
            var video = new Video(request.IdVideo, "test@example.com", "video.mp4", "mp4", 1000, 300);
            var validationResult = new ValidationResult();

            _messageQueueServiceMock.Setup(m => m.ReceberMensagemAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([queueMessage]);

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            _videoRepositoryMock.Setup(r => r.ObterPorIdAsync(request.IdVideo))
                .ReturnsAsync(video);

            _videoRepositoryMock.Setup(r => r.AtualizarStatusProcessamentoAsync(video, Status.FinalizadoComSucesso))
                .ReturnsAsync(1);

            _messageQueueServiceMock.Setup(m => m.DeletarMensagemAsync(queueMessage.MessageIdentifier, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _videoPublisherMock.Setup(p => p.PublicarProcessamentoFinalizadoAsync(video, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _useCase.AtualizarStatusDeProcessamentoAsync();

            // Assert
            Assert.Equal(Status.FinalizadoComSucesso, video.StatusAtual);
            _videoRepositoryMock.Verify(r => r.AtualizarStatusProcessamentoAsync(video, Status.FinalizadoComSucesso), Times.Once);
            _messageQueueServiceMock.Verify(m => m.DeletarMensagemAsync(queueMessage.MessageIdentifier, It.IsAny<CancellationToken>()), Times.Once);
            _videoPublisherMock.Verify(p => p.PublicarProcessamentoFinalizadoAsync(video, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_DeveMoverMensagemParaDLQQuandoVideoNaoExisteOuStatusRepetido()
        {
            // Arrange
            var request = CriaRequestValido();
            var queueMessage = new QueueMessage<AtualizaStatusVideoRequest>(request, Guid.NewGuid());

            _messageQueueServiceMock.Setup(m => m.ReceberMensagemAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<QueueMessage<AtualizaStatusVideoRequest>> { queueMessage });

            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            _videoRepositoryMock.Setup(r => r.ObterPorIdAsync(request.IdVideo))
                .ReturnsAsync((Video?)null);

            // Act
            await _useCase.AtualizarStatusDeProcessamentoAsync();

            // Assert
            _messageQueueServiceMock.Verify(m => m.MoverParaDlqAsync(queueMessage, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_DeveMoverMensagemParaDLQQuandoValidacaoFalha()
        {
            // Arrange
            var request = new AtualizaStatusVideoRequest(Guid.NewGuid(), "Invalido");
            var queueMessage = new QueueMessage<AtualizaStatusVideoRequest>(request, Guid.NewGuid());
            var validationResult = new ValidationResult(
                new List<ValidationFailure> 
                { 
                    new("Status", 
                    "Status inválido") 
                }
            );

            _messageQueueServiceMock.Setup(m => m.ReceberMensagemAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<QueueMessage<AtualizaStatusVideoRequest>> { queueMessage });
            
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(validationResult);

            // Act
            await _useCase.AtualizarStatusDeProcessamentoAsync();

            // Assert
            _messageQueueServiceMock.Verify(m => m.MoverParaDlqAsync(queueMessage, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AtualizarStatusDeProcessamentoAsync_DeveLogarErroQuandoExceptionOcorre()
        {
            // Arrange
            var request = CriaRequestValido();
            var queueMessage = new QueueMessage<AtualizaStatusVideoRequest>(request, Guid.NewGuid());

            _messageQueueServiceMock.Setup(m => m.ReceberMensagemAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<QueueMessage<AtualizaStatusVideoRequest>> { queueMessage });
            
            _validatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Erro simulado"));

            // Act
            await _useCase.AtualizarStatusDeProcessamentoAsync();

            // Assert
            _loggerMock.Verify(l => l.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), It.IsAny<Exception>(), (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Once);
        }

        #endregion

        private static AtualizaStatusVideoRequest CriaRequestValido()
        {
            return new(Guid.NewGuid(), "FinalizadoComSucesso", "s3://video.zip", "s3://video.jpg");
        }
    }
}
