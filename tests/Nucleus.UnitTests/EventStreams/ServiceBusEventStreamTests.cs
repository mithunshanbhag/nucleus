using System.Text;
using System.Text.Json;

namespace Nucleus.UnitTests.EventStreams;

public class ServiceBusEventStreamTests
{
    private readonly Mock<ServiceBusClient> _serviceBusClientMock = new();
    private readonly Mock<ServiceBusSender> _serviceBusSenderMock = new();
    private readonly ITestServiceBusEventStream _sut;

    public ServiceBusEventStreamTests()
    {
        _serviceBusClientMock
            .Setup(client => client.CreateSender(It.IsAny<string>()))
            .Returns(_serviceBusSenderMock.Object);

        _sut = new TestServiceBusEventStream(_serviceBusClientMock.Object);
    }

    private static bool MatchesMessage(ServiceBusMessage sbMessage, TestMessage testMessage)
    {
        var sbMsgAsByteArray = sbMessage.Body.ToArray();

        var sbMsgAsString = Encoding.UTF8.GetString(sbMsgAsByteArray);

        var sbMsgDeserialized = JsonSerializer.Deserialize<TestMessage>(sbMsgAsString);

        return sbMsgDeserialized?.Content == testMessage?.Content;
    }

    private static bool MatchMessages(IReadOnlyList<ServiceBusMessage> sbMessages, IReadOnlyList<TestMessage> testMessages)
    {
        if (sbMessages.Count != testMessages.Count) return false;

        for (var i = 0; i < sbMessages.Count; i++)
            if (!MatchesMessage(sbMessages[i], testMessages[i]))
                return false;

        return true;
    }

    #region Positive Cases

    [Fact]
    public async Task ServiceBusEventStream_ValidMessage_Succeeds()
    {
        // Arrange
        var testMessage = new TestMessage { Content = "testContent" };
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.PublishAsync(testMessage, cancellationToken);

        // Assert
        _serviceBusSenderMock.Verify(
            sender => sender.SendMessageAsync(It.Is<ServiceBusMessage>(sbMsg => MatchesMessage(sbMsg, testMessage)), cancellationToken),
            Times.Once);
    }

    [Fact]
    public async Task ServiceBusEventStream_MultipleValidMessages_Succeeds()
    {
        // Arrange
        var testMessages = new List<TestMessage>
        {
            new() { Content = "testContent1" }, new() { Content = "testContent2" }, new() { Content = "testContent3" }
        };
        var cancellationToken = CancellationToken.None;

        // Act
        await _sut.PublishAsync(testMessages, cancellationToken);

        // Assert
        _serviceBusSenderMock.Verify(
            sender => sender.SendMessagesAsync(
                It.Is<IEnumerable<ServiceBusMessage>>(sbMsgs => MatchMessages(sbMsgs.ToList(), testMessages.ToList())), cancellationToken),
            Times.Once);
    }

    #endregion
}