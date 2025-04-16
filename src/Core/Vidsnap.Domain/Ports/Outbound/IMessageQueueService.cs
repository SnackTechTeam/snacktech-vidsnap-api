using Vidsnap.Domain.DTOs.Queues;

namespace Vidsnap.Domain.Ports.Outbound;

public interface IMessageQueueService<T>
{
    Task<List<QueueMessage<T>>> ReceberMensagemAsync(CancellationToken cancellationToken = default);
    Task DeletarMensagemAsync(object messageIdentifier, CancellationToken cancellationToken = default);
    Task MoverParaDlqAsync(QueueMessage<T> queueMessage, CancellationToken cancellationToken = default);
}