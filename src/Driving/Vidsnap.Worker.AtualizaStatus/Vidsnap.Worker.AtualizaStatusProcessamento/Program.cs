using Vidsnap.Worker.AtualizaStatusProcessamento;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCoreUseCases(builder.Configuration);

// Configuração do Logging Padrão do .NET
builder.Logging.ClearProviders();  // Remove provedores padrão
builder.Logging.AddConsole();      // Adiciona logging no console
builder.Logging.AddDebug();        // Adiciona logging no Debug Output

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
