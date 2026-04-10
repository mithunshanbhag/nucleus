namespace Nucleus.EventStreams.Implementations;

public abstract class ServiceBusEventStreamBase<TEvent>(ServiceBusClient serviceBusClient, string queueOrTopicName) : IServiceBusEventStream<TEvent>
    where TEvent : class
{
    private readonly ServiceBusSender _serviceBusSender = serviceBusClient.CreateSender(queueOrTopicName);

    public async Task PublishAsync(TEvent evt, CancellationToken cancellationToken = default)
    {
        var serializedEvent = JsonSerializer.Serialize(evt);

        var serviceBusMessage = new ServiceBusMessage(serializedEvent);

        serviceBusMessage.ApplicationProperties.Add(ServiceBusMessageCustomPropertyKeys.EventType, evt.GetType().Name);

        await _serviceBusSender.SendMessageAsync(serviceBusMessage, cancellationToken);
    }

    public async Task PublishAsync(IEnumerable<TEvent> events, CancellationToken cancellationToken = default)
    {
        var serviceBusMessages = new List<ServiceBusMessage>();

        foreach (var evt in events)
        {
            var serializedEvent = JsonSerializer.Serialize(evt);

            var serviceBusMessage = new ServiceBusMessage(serializedEvent);

            serviceBusMessage.ApplicationProperties.Add(ServiceBusMessageCustomPropertyKeys.EventType,
                evt.GetType().Name);

            serviceBusMessages.Add(serviceBusMessage);
        }

        await _serviceBusSender.SendMessagesAsync(serviceBusMessages, cancellationToken);
    }
}