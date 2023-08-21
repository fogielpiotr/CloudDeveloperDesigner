using Application.Deployments.Command.ExecuteDeployment;
using Azure.Messaging.ServiceBus;
using Domain.Events;
using Infrastructure.Options;
using MediatR;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Worker.QueueListener
{
    internal sealed class DeploymentListener : BackgroundService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusProcessor _processor;
        private readonly ILogger<DeploymentListener> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TelemetryClient _telemetryClient;

        public DeploymentListener(
            ServiceBusClient serviceBusClient,
            IOptions<ServiceBusOptions> serviceBusOptions,
            ILogger<DeploymentListener> logger,
            TelemetryClient telemetryClient, IServiceScopeFactory scopeFactory)
        {
            _serviceBusClient = serviceBusClient;
            _serviceBusClient = serviceBusClient;
            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
                ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete
            };

            _processor = _serviceBusClient.CreateProcessor(serviceBusOptions.Value.InputQueueName, options);
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcesErrorAsync;
            _logger = logger;
            _telemetryClient = telemetryClient;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _processor.StartProcessingAsync(stoppingToken);
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            await _processor.StopProcessingAsync(stoppingToken);
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            var deploymentEvent = JsonConvert.DeserializeObject<DeploymentQueued>(args.Message.Body.ToString());
            using var telemetry = _telemetryClient.StartOperation<RequestTelemetry>(deploymentEvent.DeploymentId.ToString());
            using var scope = _scopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetService<IMediator>();
            var messageBroker = scope.ServiceProvider.GetService<IMessageBroker>();
            _logger.LogWarning("Start Processing DeploymentId {DeploymentId}", deploymentEvent.DeploymentId);
     
            var command = new ExecuteDeploymentCommand() { Id = deploymentEvent.DeploymentId };
            await mediator.Send(command, args.CancellationToken);

            _logger.LogWarning("Finish Processing DeploymentId {DeploymentId}", deploymentEvent.DeploymentId);
        }

        private Task ProcesErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "ExceptionMessage: {exceptionMessage}", args.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
