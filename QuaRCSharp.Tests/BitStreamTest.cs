using QuaRCSharp.Data;

namespace QuaRCSharp.Tests;

public class BitStreamTest
{
    [Theory]
    [InlineData(new[] { true, false, true, false })]
    [InlineData(new[] { false, true, false, true })]
    public void WriteReadBits(bool[] values)
    {
        BitStream stream = new();

        stream.WriteBits(values);
        stream.Pointer = 0;
        foreach (bool expectedBit in values)
        {
            bool actualBit = stream.ReadBit();
            Assert.Equal(expectedBit, actualBit);
        }

        Assert.Equal(stream.Pointer, values.Length);
        Assert.Equal(stream.Length, values.Length);
        Assert.Throws<EndOfStreamException>(() => stream.ReadBit());
    }

    [Theory]
    [InlineData(true, 1)]
    [InlineData(false, 1)]
    [InlineData(false, 2)]
    [InlineData(true, 2)]
    public void OverwriteReadBits(bool newValue, int pointerPosition)
    {
        BitStream stream = new();

        stream.WriteBits([true, false, true, false]);

        stream.Pointer = pointerPosition;
        stream.WriteBit(newValue);
        stream.Pointer = pointerPosition;
        bool actualBit = stream.ReadBit();

        Assert.Equal(actualBit, newValue);
    }

    [Theory]
    [InlineData(new byte[] { 255, 255 })]
    [InlineData(new byte[] { 0, 255 })]
    [InlineData(new byte[] { 0b_0000_1111, 0b_1111_0000 })]
    public void WriteReadBytes(byte[] values)
    {
        BitStream stream = new();

        stream.WriteBytes(values);
        stream.Pointer = 0;
        foreach(byte expectedValue in values)
        {
            byte actualValue = stream.ReadByte();
            Assert.Equal(expectedValue, actualValue);
        }
        
        Assert.Equal(stream.Pointer, values.Length * 8);
        Assert.Equal(stream.Length, values.Length * 8);
        Assert.Throws<EndOfStreamException>(() => stream.ReadBit());
        Assert.Throws<EndOfStreamException>(() => stream.ReadByte());
    }
    
    [Theory]
    [InlineData(0b_1010_1010, 4, new byte[] { 0b_1111_1010, 0b_1010_1111 }, true)]
    [InlineData(0b_1010_1010, 0, new byte[] { 0b_1010_1010, 0b_1111_1111 }, true)]
    [InlineData(0b_1010_1010, 9, new byte[] { 0b_1111_1111, 0b_1101_0101 }, false)]
    public void OverwriteReadBytes(byte newValue, int pointerPosition, byte[] expectedValues, bool expectedLastBit)
    {
        BitStream stream = new();
        stream.WriteBytes([0b_1111_1111, 0b_1111_1111]);
        stream.WriteBit(true);
        stream.Pointer = pointerPosition;
        stream.WriteByte(newValue);
        
        stream.Pointer = 0;
        foreach (byte expectedValue in expectedValues)
        {
            byte actualValue = stream.ReadByte();
            Assert.Equal(expectedValue, actualValue);
        }
        Assert.Throws<EndOfStreamException>(() => stream.ReadByte());
        
        bool lastBit = stream.ReadBit();
        Assert.Equal(lastBit, expectedLastBit);
        Assert.True(stream.Pointer == 17, "Pointer mismatch");
        Assert.True(stream.Length == 17, "Length mismatch");
        Assert.Throws<EndOfStreamException>(() => stream.ReadByte());
    }

    [Theory]
    [InlineData(15, 2, true)]
    [InlineData(15, 3, true)]
    [InlineData(15, 4, false)]
    [InlineData(15, 5, false)]
    public void WriteIntException(int value, int numberOfBits, bool shouldThrowException)
    {
        BitStream stream = new();
        
        if (shouldThrowException)
        { Assert.Throws<ArgumentException>(() => stream.WriteInt(value, numberOfBits)); }
        else
        { stream.WriteInt(value, numberOfBits); }
    }
    
    [Theory]
    [InlineData(255, 8, new byte[] { 255 })]
    [InlineData(255, 16, new byte[] { 0, 255 })]
    public void WriteReadInt(int value, int numberOfBits, byte[] values)
    {
        BitStream stream = new();
        
        stream.WriteInt(value, numberOfBits);
        stream.Pointer = 0;
        foreach (byte expectedValue in values)
        {
            byte actualValue = stream.ReadByte();
            Assert.Equal(expectedValue, actualValue);
        }
        
        Assert.Equal(stream.Pointer, values.Length * 8);
        Assert.Equal(stream.Length, values.Length * 8);
        Assert.Throws<EndOfStreamException>(() => stream.ReadBit());
        Assert.Throws<EndOfStreamException>(() => stream.ReadByte());
    }
    
    [Theory]
    [InlineData(0b_1010_1010, 8, 0, new byte[] { 0b_1010_1010, 0b_1111_1111 }, true)]
    [InlineData(0b_1010_1010, 8, 4, new byte[] { 0b_1111_1010, 0b_1010_1111 }, true)]
    [InlineData(0b_1010_1010, 8, 3, new byte[] { 0b_1111_0101, 0b_0101_1111 }, true)]
    [InlineData(0b_1010_1010, 10, 0, new byte[] { 0b_0010_1010, 0b_1011_1111 }, true)]
    [InlineData(0b_1010_1010, 8, 9, new byte[] { 0b_1111_1111, 0b_1101_0101 }, false)]
    public void OverwriteReadInt(int newValue, int numberOfBits, int pointerPosition, byte[] values, bool expectedLastBit)
    {
        BitStream stream = new();
        stream.WriteInt(0b_1111_1111_1111_1111, 16);
        stream.WriteBit(true);
        
        stream.Pointer = pointerPosition;
        stream.WriteInt(newValue, numberOfBits);

        stream.Pointer = 0;
        foreach (byte expectedValue in values)
        {
            byte actualValue = stream.ReadByte();
            Assert.Equal(expectedValue, actualValue);
        }
        Assert.Throws<EndOfStreamException>(() => stream.ReadByte());
        
        bool lastBit = stream.ReadBit();
        Assert.Equal(lastBit, expectedLastBit);
        Assert.True(stream.Pointer == 17, "Pointer mismatch");
        Assert.True(stream.Length == 17, "Length mismatch");
        Assert.Throws<EndOfStreamException>(() => stream.ReadBit());
        Assert.Throws<EndOfStreamException>(() => stream.ReadByte());
    }
}