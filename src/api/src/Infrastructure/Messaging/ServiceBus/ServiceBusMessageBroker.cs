using Azure.Messaging.ServiceBus;
using Newtonsoft.Json;
using Microsoft.Extensions.Options;
using Infrastructure.Options;
using Domain.Events;

namespace Infrastructure.Messaging
{
    public class ServiceBusMessageBroker : IMessageBroker
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly string _queueName;

        public ServiceBusMessageBroker(ServiceBusClient serviceBusClient, IOptions<ServiceBusOptions> serviceBusOptions)
        {
            _serviceBusClient = serviceBusClient;
            _queueName = serviceBusOptions.Value.OutputQueueName;

        }
        public async Task SendMessage<T>(T message, CancellationToken ct)
        {
            await using var sender = _serviceBusClient.CreateSender(_queueName);
            using var messageBatch = await sender.CreateMessageBatchAsync(ct);
            var objAsText = JsonConvert.SerializeObject(message);
            if (!messageBatch.TryAddMessage(new ServiceBusMessage(objAsText)))
            {
                throw new Exception("Problem with adding message");
            }

            await sender.SendMessagesAsync(messageBatch, ct);
        }
    }
}
