using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Data;

public static class ByteChain
{
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