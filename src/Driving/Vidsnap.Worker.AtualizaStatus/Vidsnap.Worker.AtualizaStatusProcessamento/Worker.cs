using System.Reflection;
using Vidsnap.Application.Ports.Inbound;

namespace Vidsnap.Worker.AtualizaStatusProcessamento
{
    public class Worker(IServiceProvider serviceProvider, IHostApplicationLifetime lifetime, ILogger<Worker> logger) : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider = serviceProvider;
        private readonly IHostApplicationLifetime _lifetime = lifetime;
        private readonly ILogger<Worker> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();
                    var atualizaStatusUseCase = scope.ServiceProvider.GetService<IAtualizarStatusVideoUseCase>();

                    if (atualizaStatusUseCase is not null)
                    {
                        await atualizaStatusUseCase.AtualizarStatusDeProcessamentoAsync(stoppingToken);
                        await Task.Delay(1000, stoppingToken);                        
                    }
                    else
                    {
                        _lifetime.StopApplication();
                    }
                }
                catch (Exception ex) 
                {
                    _logger.LogError(ex, "Erro ao executar o Worker {Assembly}", Assembly.GetExecutingAssembly().GetName().Name!);
                    _lifetime.StopApplication();
                }
            }
        }
    }
}
