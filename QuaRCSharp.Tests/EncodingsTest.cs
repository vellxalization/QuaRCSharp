using System.Text;
using QuaRCSharp.Data;
using QuaRCSharp.Data.Encoding;

namespace QuaRCSharp.Tests;

public class EncodingsTest
{
    [Theory]
    [InlineData("", 0, true)]
    [InlineData(" ", 6, false)]
    [InlineData("A", 6, false)]
    [InlineData("AA", 11, false)]
    [InlineData("AAA", 17, false)]
    [InlineData("A$", 11, false)]
    [InlineData("A,", 11, true)]
    public void AlphanumericEncoding(string input, int expectedLength, bool shouldThrowException)
    {
        var alphanumericEncoding = new AlphanumericEncoder();
        if (shouldThrowException)
        { Assert.Throws<ArgumentException>(() => alphanumericEncoding.Encode(input)); }
        else
        {
            BitStream encoded = alphanumericEncoding.Encode(input);
            Assert.Equal(expectedLength, encoded.Length);
            Assert.Equal(expectedLength, encoded.Pointer);
        }
    }
    
    [Theory]
    [InlineData("", 0, true)]
    [InlineData(" ", 0, true)]
    [InlineData("11A", 0, true)]
    [InlineData("1111", 14, false)]
    [InlineData("111", 10, false)]
    [InlineData("11", 7, false)]
    [InlineData("1", 4, false)]
    public void NumericEncoding(string input, int expectedLength, bool shouldThrowException)
    {
        var numericEncoder = new NumericEncoder();
        if (shouldThrowException)
        { Assert.Throws<ArgumentException>(() => numericEncoder.Encode(input)); }
        else
        {
            BitStream encoded = numericEncoder.Encode(input);
            Assert.Equal(expectedLength, encoded.Length);
            Assert.Equal(expectedLength, encoded.Pointer);
        }
    }

    [Theory]
    [InlineData("", 0, true)]
    [InlineData(" ", 8, false)]
    [InlineData("11A", 24, false)]
    [InlineData("1111", 32, false)]
    [InlineData("111", 24, false)]
    [InlineData("11", 16, false)]
    [InlineData("1", 8, false)]
    public void ByteEncoding(string input, int expectedLength, bool shouldThrowException)
    {
        var encoder = new ByteEncoder();
        if (shouldThrowException)
        { Assert.Throws<ArgumentException>(() => encoder.Encode(input)); }
        else
        {
            BitStream encoded = encoder.Encode(input);
            Assert.Equal(expectedLength, encoded.Pointer);
            Assert.Equal(expectedLength, encoded.Length);
        }
    }
}