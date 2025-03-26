namespace Vidsnap.Domain.DTOs.Queues
{
    public class QueueMessage<T>(T messageBody, object messageIdentifier)
    {
        public T MessageBody { get; } = messageBody;
        public object MessageIdentifier { get; } = messageIdentifier;
    }
}
