using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Vidsnap.Api.Configuration;
using Vidsnap.Api.Configuration.HealthChecks;
using Vidsnap.DataBase;
using Vidsnap.DataBase.Context;
using Vidsnap.S3Bucket;

namespace Vidsnap.Api
{
    [ExcludeFromCodeCoverage]
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder.Services, builder.Configuration);

            var app = builder.Build();

            await ApplyDatabaseMigrationAsync(app);

            ConfigureMiddleware(app);

            await app.RunAsync();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAdapterDatabaseRepositories();
            services.AddCoreUseCases();
            services.AddApplicationValidators();
            services.AddAdapterAmazonS3Bucket(configuration);

            services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vidsnap", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            string dbConnectionString = configuration.GetConnectionString("DefaultConnection") ?? "";

            if (string.IsNullOrEmpty(dbConnectionString))
            {
                throw new InvalidOperationException(
                    "Could not find a connection string named 'DefaultConnection'.");
            }

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(dbConnectionString));

            services.AddHealthChecks()
                    .ConfigureSQLHealthCheck();
        }

        private static async Task ApplyDatabaseMigrationAsync(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await dbContext.Database.MigrateAsync();
        }

        private static void ConfigureMiddleware(WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vidsnap API v1");
            });

            app.UseCustomHealthChecks();
            app.UseAuthorization();

            // Redirecionamento da URL raiz para /swagger
            app.MapGet("/", context =>
            {
                context.Response.Redirect("/swagger");
                return Task.CompletedTask;
            });

            app.MapControllers();
        }
    }
}