using Amazon.Runtime.Internal.Util;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Domain.DTOs.Queues;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.SQS.QueueClient;

public class SqsMessageQueue<T>(IAmazonSQS sqsClient, IOptions<QueuesSettings> queuesSettings, Microsoft.Extensions.Logging.ILogger<SqsMessageQueue<T>>? logger = null) : IMessageQueueService<T>
{
    private readonly IAmazonSQS _sqsClient = sqsClient;
    private readonly QueuesSettings _queuesSettings = queuesSettings.Value;
    private readonly Microsoft.Extensions.Logging.ILogger? _logger = logger;

    public async Task<List<QueueMessage<T>>> ReceberMensagemAsync(CancellationToken cancellationToken = default)
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = _queuesSettings.QueueAtualizaStatusURL,
            MaxNumberOfMessages = _queuesSettings.MaxNumberOfMessages,
            WaitTimeSeconds = 5
        };

        var response = await _sqsClient.ReceiveMessageAsync(request, cancellationToken);

        List<QueueMessage<T>> result = [];

        foreach (var message in response.Messages)
        {
            try
            {
                var objectMessage = JsonSerializer.Deserialize<T>(message.Body);

                if (objectMessage is not null)
                {
                    result.Add(new(objectMessage, message.ReceiptHandle));
                }
            }
            catch
            {
                await EnviarParaDlq(message.Body, message.ReceiptHandle, cancellationToken);
            }
        }

        return result;
    }

    public async Task DeletarMensagemAsync(object messageIdentifier, CancellationToken cancellationToken = default)
    {
        if (messageIdentifier is not string receiptHandle)
            throw new ArgumentException("Invalid message identifier for SQS", nameof(messageIdentifier));

        var request = new DeleteMessageRequest
        {
            QueueUrl = _queuesSettings.QueueAtualizaStatusURL,
            ReceiptHandle = receiptHandle
        };

        await _sqsClient.DeleteMessageAsync(request, cancellationToken);
    }

    public async Task MoverParaDlqAsync(QueueMessage<T> queueMessage, CancellationToken cancellationToken = default)
    {
        if (queueMessage.MessageIdentifier is not string receiptHandle)
            throw new ArgumentException("Invalid message identifier for SQS", nameof(queueMessage));

        await EnviarParaDlq(JsonSerializer.Serialize(queueMessage.MessageBody), receiptHandle, cancellationToken);
    }

    private async Task EnviarParaDlq(string messageBody, string messageIdentifier, CancellationToken cancellationToken = default)
    {
        // Reenviar a mensagem para a DLQ
        var sendRequest = new SendMessageRequest
        {
            QueueUrl = _queuesSettings.DlqQueueAtualizaStatusURL, // Agora a mensagem vai para a DLQ
            MessageGroupId = "Erro",
            MessageDeduplicationId = messageIdentifier.GetHashCode().ToString(),
            MessageBody = messageBody
        };

        if(_logger is not null) _logger.LogWarning("Enviando mensagem para DLQ: {MessageBody}", messageBody);
        if(_logger is not null) _logger.LogWarning("MessageDeduplicationId: {MessageIdentifier}", messageIdentifier.GetHashCode().ToString());

        await _sqsClient.SendMessageAsync(sendRequest, cancellationToken);

        if(_logger is not null) _logger.LogInformation("Mensagem enviada para DLQ: {DLQ}", _queuesSettings.DlqQueueAtualizaStatusURL);

        // Deletar a mensagem original para evitar reprocessamento
        await DeletarMensagemAsync(messageIdentifier, cancellationToken);

        if(_logger is not null) _logger.LogInformation("Mensagem deletada da fila original: {QueueUrl}", _queuesSettings.QueueAtualizaStatusURL);
    }
}