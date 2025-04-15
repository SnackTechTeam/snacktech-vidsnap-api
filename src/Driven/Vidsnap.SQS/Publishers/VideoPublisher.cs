using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Vidsnap.Application.DTOs.Broker;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.SQS.Publishers
{
    public class VideoPublisher(IAmazonSQS sqsClient, IOptions<QueuesSettings> queuesSettings) : IVideoPublisher
    {
        private readonly IAmazonSQS _sqsClient = sqsClient;
        private readonly QueuesSettings _queuesSettings = queuesSettings.Value;

        public async Task PublicarProcessamentoFinalizadoAsync(Video video, CancellationToken cancellationToken = default)
        {
            var processamentoMessage = new ProcessamentoVideoFinalizadoMessage(
                video.EmailUsuario,
                video.NomeVideo,
                video.StatusAtual.ToString()
            );

            var request = new SendMessageRequest
            {
                QueueUrl = _queuesSettings.QueueEnviaNotificacaoURL,
                MessageBody = JsonSerializer.Serialize(processamentoMessage)
            };

            await _sqsClient.SendMessageAsync(request, cancellationToken);
        }
    }
}
