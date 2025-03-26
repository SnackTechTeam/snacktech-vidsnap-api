using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Domain.Enums;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.Application.UseCases
{
    public class AtualizarStatusVideoUseCase(
        IMessageQueueService<AtualizaStatusVideoRequest> messageQueueService,
        IVideoRepository videoRepository
        ) : IAtualizarStatusVideoUseCase
    {        
        private readonly IMessageQueueService<AtualizaStatusVideoRequest> _messageQueueService = messageQueueService;
        private readonly IVideoRepository _videoRepository = videoRepository;

        public async Task AtualizarStatusDeProcessamentoAsync(CancellationToken cancellationToken = default)
        {
            var queueMessages = await _messageQueueService.ReceberMensagemAsync(cancellationToken);

            foreach (var queueMessage in queueMessages)
            {
                if (Enum.TryParse(queueMessage.MessageBody.Status, true, out Status status))
                {
                    var video = await _videoRepository.ObterPorIdAsync(queueMessage.MessageBody.IdVideo);

                    if (video is null || video.VideoStatuses.Any(vs => vs.Status == status))
                    {
                        //Mode para a DLQ quando o vídeo não é encontrado no banco de dados
                        //ou quando o vídeo já tem o status que está tentando atualizar
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
                }
                else
                {
                    //Move para a DLQ quando o status não é reconhecido
                    await _messageQueueService.MoverParaDlqAsync(queueMessage, cancellationToken);                    
                }
            }           
        }
    }
}
