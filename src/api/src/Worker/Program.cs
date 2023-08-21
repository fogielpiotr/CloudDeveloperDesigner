using Azure.Extensions.AspNetCore.Configuration.Secrets;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Infrastructure;
using Worker.QueueListener;
using Application;
using Application.Common.Interfaces;
using Worker.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        var builtConfig = config.Build();
        var secretClient = new SecretClient(
            new Uri(builtConfig["KeyVault"]),
            new DefaultAzureCredential());
        config.AddAzureKeyVault(secretClient, new KeyVaultSecretManager());
    })
    .ConfigureServices((hostContext, services) =>
    {
        IConfiguration configuration = hostContext.Configuration;
        services.AddHostedService<DeploymentListener>();
        services.AddApplicationInsightsTelemetryWorkerService();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.UseAzureCloudProvider(configuration);
        services.AddApplication(configuration);
    })
    .Build();

await host.RunAsync();
