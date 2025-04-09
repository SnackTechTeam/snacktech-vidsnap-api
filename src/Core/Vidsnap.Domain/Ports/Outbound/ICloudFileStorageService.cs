namespace Vidsnap.Domain.Ports.Outbound
{
    public interface ICloudFileStorageService
    {
        /// <summary>
        /// Gera uma URL pré-assinada para upload com tempo de expiração de um vídeo
        /// </summary>
        /// <param name="storageName">Nome do container de arquivos</param>
        /// <param name="timeoutDuration">Minutos que a URL será válida</param>
        /// <param name="idUsuario">Identificador do usuário que fará o upload</param>
        /// <param name="idVideo">Identificador do vídeo cadastrado para upload</param>
        /// <param name="fileName">Nome do arquivo com sua extensão. Exemplo: video.mp4</param>
        /// <returns>URL pré-assinada gerada com os parâmetros passados</returns>
        Task<string> GetUploadPreSignedURLAsync(string storageName, 
            int timeoutDuration, 
            Guid? idUsuario, 
            Guid? idVideo, 
            string fileName);

        /// <summary>
        /// Gera uma URL pré-assinada para download com tempo de expiração de um arquivo
        /// </summary>
        /// <param name="storageName">Nome do container de arquivos</param>
        /// <param name="timeoutDuration">Minutos que a URL será válida</param>
        /// <param name="idUsuario">Identificador do usuário que fará o download</param>
        /// <param name="idVideo">Identificador do vídeo cadastrado para download</param>
        /// <param name="filePath">Diretório completo do arquivo com seu nome e extensão. Exemplo: videos/video.mp4</param>
        /// <returns>URL pré-assinada gerada com os parâmetros passados</returns>
        Task<string> GetDownloadPreSignedURLAsync(string storageName,
            int timeoutDuration,
            string filePath);
    }
}
