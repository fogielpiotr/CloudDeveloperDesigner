using Azure.Identity;
using Azure.ResourceManager;
using Azure.Storage;
using Infrastructure.Messaging;
using Infrastructure.Persistance.Blob;
using Infrastructure.Persistance.Cosmos;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Azure;
using Azure.ResourceManager.KeyVault;
using Infrastructure.ResourceManagement;
using Microsoft.Identity.Client;
using Microsoft.Graph;
using System.Net.Http.Headers;
using Domain.Interfaces;
using Domain.Deployments;
using Microsoft.Azure.Cosmos;
using Infrastructure.ResourceCreators;
using Infrastructure.Options;
using MediatR;
using System.Reflection;
using Domain.Events;
using Azure.Infrastructure.IdentityManagement;

namespace Infrastructure
{
    public static class DependecyInjection
    {
        public static IServiceCollection UseAzureCloudProvider(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<StorageAccountOptions>(configuration.GetSection(StorageAccountOptions.StorageAccount));
            services.Configure<AzureDevOpsOptions>(configuration.GetSection(AzureDevOpsOptions.AzureDevOps));
            services.Configure<ServiceBusOptions>(configuration.GetSection(ServiceBusOptions.ServiceBus));
            services.Configure<CosmosDbContextOptions>(configuration.GetSection(CosmosDbContextOptions.CosmosDbContext));
            services.Configure<AzureAdOptions>(configuration.GetSection(AzureAdOptions.AzureAd));
            services.AddHttpClient();

            var subscriptionId = configuration.GetSection("SubscriptionId").Get<string>();
            services.AddAzureClients(builder =>
            {
                var azureAdConf = configuration.GetSection(AzureAdOptions.AzureAd).Get<AzureAdOptions>();
                var clientCredentials = new ClientSecretCredential(azureAdConf.TenantId, azureAdConf.ClientId, azureAdConf.ClientSecret);

                var serviceBusOptions = configuration.GetSection(ServiceBusOptions.ServiceBus).Get<ServiceBusOptions>();
                var storageAccountOptions = configuration.GetSection(StorageAccountOptions.StorageAccount).Get<StorageAccountOptions>();

                builder.AddClient<ArmClient, ArmClientOptions>(x => new ArmClient(clientCredentials, subscriptionId));

                builder.AddClient<KeyVaultManagementClient, KeyVaultManagementClientOptions>(x =>
                    new KeyVaultManagementClient(subscriptionId, clientCredentials));

                builder.AddServiceBusClient(serviceBusOptions.ConnectionString);
                builder.AddBlobServiceClient(storageAccountOptions.ConnectionString);
            });


            services.AddSingleton(sp => {

                var azureAdConf = configuration.GetSection(AzureAdOptions.AzureAd).Get<AzureAdOptions>();
                var app = ConfidentialClientApplicationBuilder.Create(azureAdConf.ClientId)
                                          .WithClientSecret(azureAdConf.ClientSecret)
                                          .WithTenantId(azureAdConf.TenantId)
                                          .Build();

                GraphServiceClient graphClient = new(new DelegateAuthenticationProvider(async (requestMessage) =>
                {
                    var scopes = new string[] { "https://graph.microsoft.com/.default" };

                    var authResult = await app.AcquireTokenForClient(scopes).ExecuteAsync();

                    requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
                }));

                return graphClient;
            });

            var projectDbContextOptions = configuration.GetSection(CosmosDbContextOptions.CosmosDbContext).Get<CosmosDbContextOptions>();

            services.AddDbContext<CosmosDbContext>(x => x.UseCosmos(
                projectDbContextOptions.ConnectionString,
                projectDbContextOptions.DatabaseName));

            services.AddDbContextFactory<CosmosDbContext>(x=> x.UseCosmos(
                projectDbContextOptions.ConnectionString,
                projectDbContextOptions.DatabaseName), lifetime: ServiceLifetime.Transient);

            services.AddSingleton(new CosmosClient(projectDbContextOptions.ConnectionString, new CosmosClientOptions()));

            services.AddScoped<IResourceManagerFactory, ResourceManagerFactory>();
            services.AddScoped<IResourceManager, ServiceBusManager>();
            services.AddScoped<IResourceManager, StorageAccountManager>();
            services.AddScoped<IResourceManager, AppConfigurationManager>();
            services.AddScoped<IResourceManager, CosmosDbManager>();
            services.AddTransient<IResourceManager, SignalRManager>();
            services.AddHttpClient<SignalRManager>();
            services.AddTransient<IResourceManager, ApplicationInsightsManager>();
            services.AddHttpClient<ApplicationInsightsManager>();
            services.AddScoped<IMessageBroker, ServiceBusMessageBroker>();
            services.AddScoped<IBlobManager, BlobManager>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<ISecretsCreator, KeyVaultSecretsCreator>();
            services.AddScoped<IApplicationIdentityCreator, AzureAdApplicationCreator>();
            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ICloudResourceCreator, AzureResourceCreator>();
            services.AddScoped<IRemoteCodeRepositoryCreator, AzureDevOpsRepositoryCreator>();
            services.AddScoped<IBuildCreator, AzureDevOpsBuildCreator>();
            services.AddScoped<IResourceRepository, ResourceRepository>();
            services.AddScoped<IApplicationRepository, ApplicationRepository>();
            services.AddScoped<IDeploymentRepository, DeploymentRepository>();
            services.AddScoped<IDeploymentLogsRepository, DeploymentLogsRepository>();
            services.AddScoped<IConfigurationRepository, ConfigurationRepository>();
            services.AddScoped<IIdentityManagement, IdentityManagement>();

            services.AddMediatR(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}