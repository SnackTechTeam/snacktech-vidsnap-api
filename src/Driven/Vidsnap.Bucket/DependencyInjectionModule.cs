using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Domain.Ports.Outbound;
using Vidsnap.S3Bucket.Services;

namespace Vidsnap.S3Bucket
{
    public static class DependencyInjectionModule
    {
        public static IServiceCollection AddAdapterAmazonS3Bucket(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudFileStorageSettings>(
                configuration.GetSection("AWS:CloudFileStorage")
            );

            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
            services.AddSingleton<IAmazonS3, AmazonS3Client>();

            services.AddTransient<ICloudFileStorageService, S3Service>();

            return services;
        }
    }
}
