using FluentValidation;
using FluentValidation.AspNetCore;
using System.Diagnostics.CodeAnalysis;
using Vidsnap.Application.DTOs.Validators;
using Vidsnap.Application.Ports.Inbound;
using Vidsnap.Application.UseCases;

namespace Vidsnap.Api.Configuration
{
    [ExcludeFromCodeCoverage]
    public static class DependencyInjectionModule
    {
        public static IServiceCollection AddCoreUseCases(this IServiceCollection services)
        {
            services.AddTransient<IProcessarVideoUseCase, ProcessarVideoUseCase>();
            services.AddTransient<IBuscarVideosUseCase, BuscarVideosUseCase>();            

            return services;
        }

        public static IServiceCollection AddApplicationValidators(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<NovoVideoRequestValidator>();
            // Adiciona o suporte ao FluentValidation para validação automática
            services.AddFluentValidationAutoValidation();

            return services;
        }
    }
}