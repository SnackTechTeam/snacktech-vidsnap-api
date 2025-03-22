namespace Vidsnap.Domain.Ports.Outbound
{
    public interface ICloudFileStorageService
    {
        /// <summary>
        /// Generate a presigned URL that can be used to access the file named
        /// in the fileName parameter for the amount of time specified in the
        /// duration parameter.
        /// </summary>        
        /// <param name="storageName">The name of the storage container containing the
        /// object for which to create the presigned URL.</param>
        /// <param name="fileName">The name of the object to access with the
        /// presigned URL.</param>
        /// <param name="timeoutDuration">The length of time for which the presigned
        /// URL will be valid.</param>
        /// <returns>A string representing the generated presigned URL.</returns>
        Task<string> GetPreSignedURLAsync(string storageName, int timeoutDuration, Guid idUsuario, string fileName);
    }
}
