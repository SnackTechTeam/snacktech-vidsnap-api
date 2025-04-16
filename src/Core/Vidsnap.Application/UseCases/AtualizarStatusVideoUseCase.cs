using FluentValidation;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.Application.UseCases
{
    public class AtualizarStatusVideoUseCase(
        IMessageQueueService<AtualizaStatusVideoRequest> messageQueueService,
        IVideoPublisher videoPublisher,
        IVideoRepository videoRepository,
        IValidator<AtualizaStatusVideoRequest> validator,
        ILogger<AtualizarStatusVideoUseCase> logger
        ) : IAtualizarStatusVideoUseCase
    {        
        private readonly IMessageQueueService<AtualizaStatusVideoRequest> _messageQueueService = messageQueueService;
        private readonly IVideoPublisher _videoPublisher = videoPublisher;
        private readonly IVideoRepository _videoRepository = videoRepository;
        private readonly IValidator<AtualizaStatusVideoRequest> _validator = validator;
        private readonly ILogger<AtualizarStatusVideoUseCase> _logger = logger;

        public async Task AtualizarStatusDeProcessamentoAsync(CancellationToken cancellationToken = default)
        {
            var queueMessages = await _messageQueueService.ReceberMensagemAsync(cancellationToken);

            foreach (var queueMessage in queueMessages)
            {
                try
                {
                    var validationResult = await _validator.ValidateAsync(queueMessage.MessageBody, cancellationToken);

                    if (validationResult.IsValid)
                    {
                        var video = await _videoRepository.ObterPorIdAsync(queueMessage.MessageBody.IdVideo);
                        var status = Enum.Parse<Status>(queueMessage.MessageBody.Status, false);

                        if (video is null || status <= video.StatusAtual)
                        {
                            //Move para a DLQ quando o vídeo não é encontrado no banco de dados
                            //ou quando o status enviado é menor ou igual ao status atual do vídeo
                            _logger.LogWarning("Enviando para DLQ pois o vídeo não existe ou o status enviado é menor ou igual ao status atual: {Mensagem}",
                                JsonSerializer.Serialize(queueMessage));
                            await _messageQueueService.MoverParaDlqAsync(queueMessage, cancellationToken);
                            continue;
                        }

                        video.AtualizarStatus(status);

                        //Somente inclui a url do zip se ela for não nula e o status for FinalizadoComSucesso
                        if (!string.IsNullOrEmpty(queueMessage.MessageBody.UrlZip)
                            && !string.IsNullOrEmpty(queueMessage.MessageBody.UrlImagem)
                            && status == Status.FinalizadoComSucesso)
                        {
                            video.IncluirURLs(queueMessage.MessageBody.UrlZip, queueMessage.MessageBody.UrlImagem);
                        }

                        await _videoRepository.AtualizarStatusProcessamentoAsync(video, status);

                        await _messageQueueService.DeletarMensagemAsync(queueMessage.MessageIdentifier, cancellationToken);

                        if (status == Status.FinalizadoComSucesso || status == Status.FinalizadoComErro)
                        {
                            await _videoPublisher.PublicarProcessamentoFinalizadoAsync(video, cancellationToken);
                        }

                        _logger.LogInformation("Mensagem processada com sucesso: {Mensagem}", JsonSerializer.Serialize(queueMessage));
                    }
                    else
                    {
                        //Move para a DLQ quando o status não é reconhecido
                        _logger.LogWarning("Enviando para DLQ porque a mensagem é inválida: {Mensagem} \nErros: {Erros}",
                                JsonSerializer.Serialize(queueMessage), JsonSerializer.Serialize(validationResult.Errors));
                        await _messageQueueService.MoverParaDlqAsync(queueMessage, cancellationToken);
                    }
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar a seguinte mensagem: {Message}", JsonSerializer.Serialize(queueMessage));
                }
            }           
        }
    }
}