﻿using Amazon.S3;
using Amazon.S3.Model;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.S3Bucket.Services
{
    public class S3Service(IAmazonS3 s3Client) : ICloudFileStorageService
    {        
        private readonly IAmazonS3 _s3Client = s3Client;        

        public async Task<string> GetUploadPreSignedURLAsync(
            string storageName, 
            int timeoutDuration, 
            Guid? idUsuario, 
            Guid? idVideo, 
            string fileName
        )
        {
            var pastaBase = idUsuario.HasValue ? $"{idUsuario.Value}/" : string.Empty;
            var subPasta = idVideo.HasValue ? $"{idVideo.Value}/" : string.Empty;

            var request = new GetPreSignedUrlRequest()
            {
                BucketName = storageName,
                Key = $"{pastaBase}{subPasta}{fileName}",
                Verb = HttpVerb.PUT,
                Expires = DateTime.UtcNow.AddMinutes(timeoutDuration),
                ContentType = "application/octet-stream"
            };

            var urlString = await _s3Client.GetPreSignedURLAsync(request);

            return urlString;            
        }

        public async Task<string> GetDownloadPreSignedURLAsync(
            string storageName, 
            int timeoutDuration,
            string filePath
        )
        {
            var request = new GetPreSignedUrlRequest()
            {
                BucketName = storageName,
                Key = filePath,
                Verb = HttpVerb.GET,
                Expires = DateTime.UtcNow.AddMinutes(timeoutDuration)
            };

            var urlString = await _s3Client.GetPreSignedURLAsync(request);

            return urlString;
        }        
    }
}
