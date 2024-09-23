using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Data.Encoding;

public class ByteEncoder : IEncodingMethod
{
    public BitStream Encode(string input)
    {
        var dataStream = new BitStream();
        
        foreach (byte @byte in System.Text.Encoding.UTF8.GetBytes(input))
        { dataStream.WriteByte(@byte); }
        
        return dataStream;
    }

    public BitStream GenerateHeader(QRCodeVersion version, string input)
    {
        var stream = new BitStream();
        stream.WriteBits([false, true, false, false]);
        
        stream.WriteInt(System.Text.Encoding.UTF8.GetByteCount(input), CalculateFieldLength(version));
        return stream;
    }

    private int CalculateFieldLength(int version) => version < 10 ? 8 : 16;
}