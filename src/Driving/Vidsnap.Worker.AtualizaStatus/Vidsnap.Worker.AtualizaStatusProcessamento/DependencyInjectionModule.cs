using Amazon;
using Amazon.Runtime;
using Amazon.SQS;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using Vidsnap.Application.DTOs.Requests;
using Vidsnap.Application.DTOs.Settings;
using Vidsnap.Application.DTOs.Validators;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Application.UseCases;
using Vidsnap.DataBase;
using Vidsnap.DataBase.Context;
using Vidsnap.Domain.Ports.Outbound;
using Vidsnap.SQS.Publishers;
using Vidsnap.SQS.QueueClient;

namespace Vidsnap.Worker.AtualizaStatusProcessamento
{
    public static class DependencyInjectionModule
    {
        public static IServiceCollection AddCoreUseCases(this IServiceCollection services, IConfiguration configuration)
        {
            //Configura DataBase Adapter
            RegisterAdapterDatabase(services, configuration);
            
            //Configura SQS Adapter
            RegisterAdapterSQS(services, configuration);

            RegisterApplicationValidators(services);
            RegisterCoreUseCases(services);

            return services;
        }

        private static void RegisterCoreUseCases(IServiceCollection services)
        {
            services.AddTransient<IAtualizarStatusVideoUseCase, AtualizarStatusVideoUseCase>();            
        }

        private static void RegisterAdapterDatabase(IServiceCollection services, IConfiguration configuration)
        {
            string dbConnectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'DefaultConnection'.");
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(dbConnectionString));

            services.AddAdapterDatabaseRepositories();
        }

        private static void RegisterAdapterSQS(IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<QueuesSettings>(
                configuration.GetSection("AWS:Queues")
            );

            var awsDefaultSettings = configuration.GetSection("AWS").Get<AwsDefaultSettings>() 
                ?? throw new ArgumentNullException(nameof(configuration), "AWS não encontrado.");

            services.AddSingleton<IAmazonSQS>(serviceProvider =>
            {
                var awsCredentials = new SessionAWSCredentials(
                    awsDefaultSettings.Credentials.AccessKey,
                    awsDefaultSettings.Credentials.SecretKey,
                    awsDefaultSettings.Credentials.SessionToken
                );

                return new AmazonSQSClient(awsCredentials, RegionEndpoint.GetBySystemName(awsDefaultSettings.Region));
            });

            services.AddTransient<IMessageQueueService<AtualizaStatusVideoRequest>, SqsMessageQueue<AtualizaStatusVideoRequest>>();
            services.AddTransient<IVideoPublisher, VideoPublisher>();
        }

        private static void RegisterApplicationValidators(IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<AtualizaStatusVideoRequestValidator>();
            // Adiciona o suporte ao FluentValidation para validação automática
            services.AddFluentValidationAutoValidation();
        }
    }
}
