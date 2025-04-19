using Amazon.S3;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Moq;
using Vidsnap.Api;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Application.UseCases;
using Vidsnap.DataBase.Context;
using Vidsnap.Domain.Entities;
using Vidsnap.Domain.Ports.Outbound;

namespace Vidsnap.BddTest.Support
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IAmazonS3> S3Mock { get; } = new();

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {                
                // Remove o DbContext real
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var contextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(AppDbContext));

                if (contextDescriptor != null)
                    services.Remove(contextDescriptor);

                // Adiciona o DbContext em memória
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });

                var sp = services.BuildServiceProvider();

                //Cria o banco (EnsureCreated)
                using var scope = sp.CreateScope();
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();

                // Substitui ICloudFileStorageService por um mock
                var cloudStorageMock = new Mock<ICloudFileStorageService>();
                
                cloudStorageMock.Setup(s => s.GetDownloadPreSignedURLAsync(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>()))
                    .ReturnsAsync("https://fake-url.com/testfile.zip");

                cloudStorageMock.Setup(s => s.GetUploadPreSignedURLAsync(It.IsAny<string>(), 
                        It.IsAny<int>(), It.IsAny<Guid?>(), It.IsAny<Guid?>(), It.IsAny<string>()))
                    .ReturnsAsync("https://fake-url.com/testfile.mp4");

                services.RemoveAll<ICloudFileStorageService>();
                services.AddSingleton(cloudStorageMock.Object);

                //Adiciona mock de IVideoPublisher
                var videoPublisherMock = new Mock<IVideoPublisher>();

                videoPublisherMock.Setup(s => s.PublicarProcessamentoFinalizadoAsync(It.IsAny<Video>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask);

                services.AddSingleton(videoPublisherMock.Object);

                services.AddScoped<IAtualizarStatusVideoUseCase, AtualizarStatusVideoUseCase>();
            });

            return base.CreateHost(builder);
        }
    }
}
