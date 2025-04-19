using Microsoft.Extensions.DependencyInjection;
using Vidsnap.Application.Ports.Inbound;

namespace Vidsnap.BddTest.Helpers
{
    public static class WorkerTestHelper
    {
        public static async Task ExecutarWorkerAtualizacaoStatusAsync(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
        {
            using var scope = serviceProvider.CreateScope();
            var useCase = scope.ServiceProvider.GetRequiredService<IAtualizarStatusVideoUseCase>();
            await useCase.AtualizarStatusDeProcessamentoAsync(cancellationToken);
        }
    }
}