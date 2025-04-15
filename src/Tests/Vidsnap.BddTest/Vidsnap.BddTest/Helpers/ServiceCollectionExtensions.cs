using Microsoft.Extensions.DependencyInjection;
using Moq;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Domain.DTOs.Queues;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.BddTest.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static void ReplaceMessageQueueServiceWithMock(
            this IServiceCollection services,
            AtualizaStatusVideoRequest mensagemNaFila)
        {
            var messageQueueServiceMock = new Mock<IMessageQueueService<AtualizaStatusVideoRequest>>();

            var mensagem = new QueueMessage<AtualizaStatusVideoRequest>(mensagemNaFila, Guid.NewGuid());

            messageQueueServiceMock.Setup(m => m.ReceberMensagemAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync([mensagem]);

            messageQueueServiceMock.Setup(m => m.DeletarMensagemAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            messageQueueServiceMock.Setup(m => m.MoverParaDlqAsync(It.IsAny<QueueMessage<AtualizaStatusVideoRequest>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            services.AddSingleton(messageQueueServiceMock.Object);
        }
    }
}