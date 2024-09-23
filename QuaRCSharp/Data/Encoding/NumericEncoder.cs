using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Data.Encoding;

/// <summary>
/// Data encoder that can encode number from 0-9.
/// NOTE: some of the QR-Code scanners were actually unable to read this encoding, but I still implemented it 
/// </summary>
public class NumericEncoder : IEncodingMethod
{
    public BitStream Encode(string input)
    {
        var stream = new BitStream();
        foreach (char[] chunk in input.Chunk(3))
        {
            if (!int.TryParse(chunk, out int number))
            { throw new ArgumentException($"Invalid character occured: {chunk}"); }
            
            switch (number)
            {
                case > 99 and < 1000:
                    stream.WriteInt(number, 10);
                    break;
                case > 9 and < 100:
                    stream.WriteInt(number, 7);
                    break;
                default:
                    stream.WriteInt(number, 4);
                    break;
            }
        }

        return stream;
    }

    public BitStream GenerateHeader(QRCodeVersion version, string input)
    {
        var stream = new BitStream();
        stream.WriteBits([false, false, false, true]);
        
        stream.WriteInt(input.Length, CalculateFieldLength(version));
        return stream;
    }

    private int CalculateFieldLength(int version) =>
        version switch
        {
            < 10 => 10,
            < 27 => 12,
            _ => 14
        };
}