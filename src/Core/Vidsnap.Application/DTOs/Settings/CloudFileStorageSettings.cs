namespace Vidsnap.Application.DTOs.Settings
{
    public class CloudFileStorageSettings
    {
        public string ContainerName { get; set; } = string.Empty;
        public int TimeoutDuration { get; set; }
    }
}