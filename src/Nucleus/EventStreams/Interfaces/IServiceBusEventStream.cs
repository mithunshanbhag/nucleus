namespace Nucleus.EventStreams.Interfaces;

/// <remarks>
///     The type parameter constraint of 'class' is only required because of this open GitHub issue:
///     - https://github.com/dotnet/runtime/issues/41749
///     Else we could have just used interface types for the type parameter.
/// </remarks>
public interface IServiceBusEventStream<in TEvent> where TEvent : class
{
    Task PublishAsync(TEvent evt, CancellationToken cancellationToken = default);

    Task PublishAsync(IEnumerable<TEvent> events, CancellationToken cancellationToken = default);
}