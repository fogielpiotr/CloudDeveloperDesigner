namespace Infrastructure.Options
{
    public class ServiceBusOptions
    {
        public const string ServiceBus = "ServiceBus";

        public string ConnectionString { get; set; } = string.Empty;
        public string InputQueueName { get; set; } = string.Empty;
        public string OutputQueueName { get; set; } = string.Empty;
    }
}
