using QuaRCSharp.Data;
using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Tests;

public class CorrectionByteTest
{
    [Theory]
    [InlineData(new byte[] {}, new byte[] {0, 0, 0, 0, 0, 0, 0})]
    [InlineData(new byte[] { 0 }, new byte[] {0, 0, 0, 0, 0, 0, 0})]
    [InlineData(new byte[] { 1, 2, 3, 4, 5, 6 }, new byte[] { 16, 29, 73, 187, 244, 214, 218 })]
    [InlineData(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, new byte[] { 186, 221, 200, 172, 182, 137, 52 })]
    public void GenerateCorrectionBytes(byte[] message, params byte[][] expectedBytes)
    {
        var generator = new CorrectionByteGenerator();
        var correctionBytes = generator.CreateErrorCorrectionBytesForByteChain([message], new QRCodeVersion(1), CorrectionLevel.L)[0];
        for (int i = 0; i < correctionBytes.Length; ++i)
        { Assert.Equal(expectedBytes[0][i], correctionBytes[i]); }
    }
}