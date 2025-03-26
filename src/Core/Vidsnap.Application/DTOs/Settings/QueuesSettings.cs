namespace Vidsnap.Application.DTOs.Settings
{
    public class QueuesSettings
    {
        public int MaxNumberOfMessages { get; set; }
        public string QueueUrl { get; set; } = null!;
        public string DlqQueueURL { get; set; } = null!;
    }
}
