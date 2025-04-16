using Amazon.Runtime.Internal.Util;
using Amazon.SQS;
using Amazon.SQS.Model;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Vidsnap.Application.DTOs.Broker;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.SQS.Publishers
{
    public class VideoPublisher(IAmazonSQS sqsClient, IOptions<QueuesSettings> queuesSettings, Microsoft.Extensions.Logging.ILogger<VideoPublisher> logger = null) : IVideoPublisher
    {
        private readonly IAmazonSQS _sqsClient = sqsClient;
        private readonly QueuesSettings _queuesSettings = queuesSettings.Value;
        private readonly Microsoft.Extensions.Logging.ILogger<VideoPublisher> _logger = logger;

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

            if(_logger is not null){
                _logger.LogWarning("Enviando mensagem para a fila SQS: {Mensagem}", JsonSerializer.Serialize(processamentoMessage));
                _logger.LogWarning("QueueUrl: {QueueUrl}", _queuesSettings.QueueEnviaNotificacaoURL);
            }

            await _sqsClient.SendMessageAsync(request, cancellationToken);

            if (_logger is not null) _logger.LogWarning("Mensagem enviada com sucesso para a fila SQS: {Mensagem}", JsonSerializer.Serialize(processamentoMessage));
        }
    }
}
