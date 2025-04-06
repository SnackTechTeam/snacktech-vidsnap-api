using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics.CodeAnalysis;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Domain.Ports.Outbound;
using Vidsnap.S3Bucket.Services;

namespace Vidsnap.S3Bucket
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionModule
    {
        public static IServiceCollection AddAdapterAmazonS3Bucket(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CloudFileStorageSettings>(
                configuration.GetSection("AWS:CloudFileStorage")
            );

            var awsDefaultSettings = configuration.GetSection("AWS").Get<AwsDefaultSettings>()
                ?? throw new ArgumentNullException(nameof(configuration), "AWS configuration not found.");

            services.AddSingleton<IAmazonS3>(serviceProvider =>
            {
                var awsCredentials = new SessionAWSCredentials(
                    awsDefaultSettings.Credentials.AccessKey,
                    awsDefaultSettings.Credentials.SecretKey,
                    awsDefaultSettings.Credentials.SessionToken
                );

                return new AmazonS3Client(awsCredentials, RegionEndpoint.GetBySystemName(awsDefaultSettings.Region));
            });

            services.AddTransient<ICloudFileStorageService, S3Service>();

            return services;
        }
    }
}
