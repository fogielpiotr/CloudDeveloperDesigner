using Api.Messaging;
using Azure.Messaging.ServiceBus;
using Domain.Events;
using Infrastructure.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Api.QueueListener
{
    internal sealed class DeploymentStatusListener : BackgroundService
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ServiceBusProcessor _processor;
        private readonly IRealTimeMessageBroker _realTimeMessageBroker;
        private readonly ILogger<DeploymentStatusListener> _logger;

        public DeploymentStatusListener(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions, IRealTimeMessageBroker realTimeMessageBroker, ILogger<DeploymentStatusListener> logger)
        {
            _serviceBusClient = serviceBusClient;
            _serviceBusClient = serviceBusClient;
            var options = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false,
            };

            _processor = _serviceBusClient.CreateProcessor(serviceBusOptions.Value.InputQueueName, options);
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ProcesErrorAsync;
            _realTimeMessageBroker = realTimeMessageBroker;
            _logger = logger;
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
            var status = JsonConvert.DeserializeObject<DeploymentEvent>(args.Message.Body.ToString());
            _logger.LogWarning("Process Message {message}", args.Message.Body.ToString());
            await _realTimeMessageBroker.SendMessage(status.DeploymentId.ToString(), status);
            await args.CompleteMessageAsync(args.Message);
        }

        private Task ProcesErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "ExceptionMessage: {exceptionMessage}", args.Exception.Message);
            return Task.CompletedTask;
        }
    }
}
