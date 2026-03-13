using System.Text;
using Nucleus.Misc.ExtensionMethods;

namespace Nucleus.UnitTests.Misc.ExtensionMethods;

public class StringExtensionTests
{
    private const string Input = "Hello, World!";

    #region Positive Cases

    [Fact]
    public void AsByteArray_ShouldConvertStringToByteArray()
    {
        // Arrange
        var expected = Encoding.UTF8.GetBytes(Input);

        // Act
        var actual = Input.AsByteArray();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void AsStream_ShouldConvertStringToStream()
    {
        // Arrange
        var expected = new MemoryStream(Encoding.UTF8.GetBytes(Input));

        // Act
        var actual = Input.AsStream();

        // Assert
        var memoryStream = Assert.IsType<MemoryStream>(actual);
        Assert.Equal(expected.ToArray(), memoryStream.ToArray());
    }

    [Fact]
    public void AsStream_ShouldConvertStringToStreamAndRead()
    {
        // Arrange
        var expected = new MemoryStream(Encoding.UTF8.GetBytes(Input));

        // Act
        var actual = Input.AsStream();

        // Assert
        Assert.Equal(expected.ToArray(), actual.AsByteArray());
    }

    #endregion

    #region Boundary Cases

    [Fact]
    public void AsByteArray_EmptyString_ReturnsEmptyArray()
    {
        // Act
        var actual = string.Empty.AsByteArray();

        // Assert
        Assert.Empty(actual);
    }

    [Fact]
    public void AsStream_EmptyString_ReturnsEmptyStream()
    {
        // Act
        var actual = string.Empty.AsStream();

        // Assert
        var memoryStream = Assert.IsType<MemoryStream>(actual);
        Assert.Empty(memoryStream.ToArray());
    }

    #endregion
}