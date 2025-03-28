using Vidsnap.Worker.AtualizaStatusProcessamento;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddCoreUseCases(builder.Configuration);

// Configura��o do Logging Padr�o do .NET
builder.Logging.ClearProviders();  // Remove provedores padr�o
builder.Logging.AddConsole();      // Adiciona logging no console
builder.Logging.AddDebug();        // Adiciona logging no Debug Output

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
