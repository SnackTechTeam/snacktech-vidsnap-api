﻿using Amazon.S3;
using Amazon.S3.Model;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.S3Bucket.Services
{
    public class S3Service(IAmazonS3 s3Client) : ICloudFileStorageService
    {        
        private readonly IAmazonS3 _s3Client = s3Client;

        public async Task<string> GetPreSignedURLAsync(string storageName, int timeoutDuration, Guid idUsuario, string fileName)
        {
            string urlString = string.Empty;

            try
            {
                var request = new GetPreSignedUrlRequest()
                {
                    BucketName = storageName,
                    Key = $"{idUsuario}/{fileName}",
                    //Key = fileName,
                    Verb = HttpVerb.PUT,
                    Expires = DateTime.UtcNow.AddHours(timeoutDuration),
                    ContentType = "application/octet-stream"
                };
                
                urlString = await _s3Client.GetPreSignedURLAsync(request);
            }
            catch (AmazonS3Exception ex)
            {
                Console.WriteLine($"Error:'{ex.Message}'");
            }

            return urlString;
        }
    }
}
