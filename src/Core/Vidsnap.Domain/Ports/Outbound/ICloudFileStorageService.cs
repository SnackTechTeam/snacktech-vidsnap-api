namespace Vidsnap.Domain.Ports.Outbound
{
    public interface ICloudFileStorageService
    {
        Task<string> GetPreSignedURLAsync(string storageName, int timeoutDuration, Guid? idUsuario, Guid? idVideo, string fileName);
    }
}
