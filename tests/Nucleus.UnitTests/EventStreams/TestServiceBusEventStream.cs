namespace Nucleus.UnitTests.EventStreams;

public class TestServiceBusEventStream(ServiceBusClient serviceBusClient)
    : ServiceBusEventStreamBase<TestMessage>(serviceBusClient, "testTopic"), ITestServiceBusEventStream;