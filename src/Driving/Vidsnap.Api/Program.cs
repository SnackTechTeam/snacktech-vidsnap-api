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
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            ConfigureServices(builder);

            var app = builder.Build();

            await ApplyDatabaseMigrationAsync(app);

            ConfigureMiddleware(app);

            await app.RunAsync();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddAdapterDatabaseRepositories();
            builder.Services.AddCoreUseCases();
            builder.Services.AddApplicationValidators();
            builder.Services.AddAdapterAmazonS3Bucket(builder.Configuration);

            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.DescribeAllParametersInCamelCase();
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Vidsnap", Version = "v1" });
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            if (!builder.Environment.IsEnvironment("Testing"))
            {
                string dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "";

                if (string.IsNullOrEmpty(dbConnectionString))
                {
                    throw new InvalidOperationException(
                        "Could not find a connection string named 'DefaultConnection'.");
                }

                builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(dbConnectionString));
            }

            builder.Services.AddHealthChecks()
                    .ConfigureSQLHealthCheck();
        }

        private static async Task ApplyDatabaseMigrationAsync(WebApplication app)
        {
            if (!app.Environment.IsEnvironment("Testing"))
            {
                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await dbContext.Database.MigrateAsync();
            }
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