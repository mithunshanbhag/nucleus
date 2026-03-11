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
        actual.Should().Equal(expected);
    }

    [Fact]
    public void AsStream_ShouldConvertStringToStream()
    {
        // Arrange
        var expected = new MemoryStream(Encoding.UTF8.GetBytes(Input));

        // Act
        var actual = Input.AsStream();

        // Assert
        actual.Should().BeOfType<MemoryStream>()
            .Which.ToArray()
            .Should().Equal(expected.ToArray());
    }

    [Fact]
    public void AsStream_ShouldConvertStringToStreamAndRead()
    {
        // Arrange
        var expected = new MemoryStream(Encoding.UTF8.GetBytes(Input));

        // Act
        var actual = Input.AsStream();

        // Assert
        actual.AsByteArray().Should().Equal(expected.ToArray());
    }

    #endregion

    #region Boundary Cases

    [Fact]
    public void AsByteArray_EmptyString_ReturnsEmptyArray()
    {
        // Act
        var actual = string.Empty.AsByteArray();

        // Assert
        actual.Should().BeEmpty();
    }

    [Fact]
    public void AsStream_EmptyString_ReturnsEmptyStream()
    {
        // Act
        var actual = string.Empty.AsStream();

        // Assert
        actual.Should().BeOfType<MemoryStream>()
            .Which.ToArray()
            .Should().BeEmpty();
    }

    #endregion
}