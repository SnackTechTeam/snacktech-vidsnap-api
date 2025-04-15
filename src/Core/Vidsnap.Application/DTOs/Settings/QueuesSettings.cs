namespace Vidsnap.Application.DTOs.Settings
{
    public class QueuesSettings
    {
        public int MaxNumberOfMessages { get; set; }
        public string QueueAtualizaStatusURL { get; set; } = null!;
        public string DlqQueueAtualizaStatusURL { get; set; } = null!;
        public string QueueEnviaNotificacaoURL { get; set; } = null!;
    }
}
