using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Data;

/// <summary>
/// Static class for creating byte chains (arrays of byte arrays) from bit streams
/// </summary>
public static class ByteChain
{
    /// <summary>
    /// Converts bit stream to the byte chain
    /// </summary>
    /// <param name="stream">Bit stream of data</param>
    /// <param name="version">Version of QR-Code</param>
    /// <param name="correction">Level of error correction</param>
    /// <returns></returns>
    public static byte[][] CreateByteChainFromBitStream(BitStream stream, QRCodeVersion version, CorrectionLevel correction)
    {
        stream.Pointer = 0;
        
        byte[][] byteChain = CreateEmptyByteChain(stream.Length / 8, version, correction);
        foreach (byte[] chain in byteChain)
        {
            for (int i = 0; i < chain.Length; ++i)
            { chain[i] = stream.ReadByte(); }
        }

        return byteChain;
    }
    
    private static byte[][] CreateEmptyByteChain(int byteLength, QRCodeVersion version, CorrectionLevel correction)
    {
        int numberOfBlocks = QRCodeData.GetNumberOfCorrectionBlocks(version, correction);

        byte[][] chain = new byte[numberOfBlocks][];
        var divisionResult = Math.DivRem(byteLength, numberOfBlocks);
        if (divisionResult.Remainder is 0)
        {
            for (int i = 0; i < numberOfBlocks; ++i)
            { chain[i] = new byte[divisionResult.Quotient]; }

            return chain;
        }

        for (int i = 0; i < numberOfBlocks; ++i)
        { chain[i] = (i < numberOfBlocks - divisionResult.Remainder) ? new byte[divisionResult.Quotient] : new byte[divisionResult.Quotient + 1]; }
        
        return chain;
    }
}