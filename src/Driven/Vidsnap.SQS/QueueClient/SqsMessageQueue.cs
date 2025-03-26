using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Domain.DTOs.Queues;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.SQS.QueueClient;

public class SqsMessageQueue<T>(IAmazonSQS sqsClient, IOptions<QueuesSettings> queuesSettings) : IMessageQueueService<T>
{
    private readonly IAmazonSQS _sqsClient = sqsClient;
    private readonly QueuesSettings _queuesSettings = queuesSettings.Value;

    public async Task<List<QueueMessage<T>>> ReceberMensagemAsync(CancellationToken cancellationToken = default)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = _queuesSettings.QueueUrl,
            MaxNumberOfMessages = _queuesSettings.MaxNumberOfMessages,
            WaitTimeSeconds = 5
        };

        var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);

        List<QueueMessage<T>> result = [];

        foreach (var message in response.Messages)
        {            
            var objectMessage = JsonSerializer.Deserialize<T>(message.Body);

            if (objectMessage is not null)
            {
                result.Add(new(objectMessage, message.ReceiptHandle));
            }
        }

        return result;
    }

    public async Task EnviarMensagemAsync(T messageBody, CancellationToken cancellationToken = default)
    {
        var request = new SendMessageRequest
        {
            QueueUrl = _queuesSettings.QueueUrl,
            MessageBody = JsonSerializer.Serialize(messageBody)
        };

        await _sqsClient.SendMessageAsync(request, cancellationToken);
    }

    public async Task DeletarMensagemAsync(object messageIdentifier, CancellationToken cancellationToken = default)
    {
        if (messageIdentifier is not string receiptHandle)
            throw new ArgumentException("Invalid message identifier for SQS", nameof(messageIdentifier));

        var request = new DeleteMessageRequest
        {
            QueueUrl = _queuesSettings.QueueUrl,
            ReceiptHandle = receiptHandle
        };

        await _sqsClient.DeleteMessageAsync(request, cancellationToken);
    }

    public async Task MoverParaDlqAsync(QueueMessage<T> queueMessage, CancellationToken cancellationToken = default)
    {
        if (queueMessage.MessageIdentifier is not string receiptHandle)
            throw new ArgumentException("Invalid message identifier for SQS", nameof(queueMessage));

        // Reenviar a mensagem para a DLQ
        var sendRequest = new SendMessageRequest
        {
            QueueUrl = _queuesSettings.DlqQueueURL, // Agora a mensagem vai para a DLQ
            MessageBody = JsonSerializer.Serialize(queueMessage.MessageBody)
        };

        await _sqsClient.SendMessageAsync(sendRequest, cancellationToken);

        // Deletar a mensagem original para evitar reprocessamento
        await DeletarMensagemAsync(receiptHandle, cancellationToken);
    }
}