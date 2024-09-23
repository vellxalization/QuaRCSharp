using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Data.Encoding;

public class AlphanumericEncoder : IEncodingMethod
{
    public static readonly Dictionary<char, int> Characters = new()
    {
        { '0', 0 }, { '1', 1 }, { '2', 2 }, { '3', 3 }, { '4', 4 }, { '5', 5 },
        { '6', 6 }, { '7', 7 }, { '8', 8 }, { '9', 9 }, { 'A', 10 }, { 'B', 11 },
        { 'C', 12 }, { 'D', 13 }, { 'E', 14 }, { 'F', 15 }, { 'G', 16 }, { 'H', 17 },
        { 'I', 18 }, { 'J', 19 }, { 'K', 20 }, { 'L', 21 }, { 'M', 22 }, { 'N', 23 }, 
        { 'O', 24 }, { 'P', 25 }, { 'Q', 26 }, { 'R', 27 }, { 'S', 28 }, { 'T', 29 }, 
        { 'U', 30 }, { 'V', 31 }, { 'W', 32 }, { 'X', 33 }, { 'Y', 34 }, { 'Z', 35 }, 
        { ' ', 36 }, { '$', 37 }, { '%', 38 }, { '&', 39 }, { '+', 40 }, { '-', 41 }, 
        { '.', 42 }, { '/', 43 }, { ':', 44 }, 
    };
    
    public BitStream Encode(string input)
    {
        var stream = new BitStream();
        foreach (char[] chunk in input.ToUpper().Chunk(2))
        {
            if (chunk.Length is 1)
            {
                if (!Characters.TryGetValue(chunk[0], out int value))
                { throw new ArgumentException($"Invalid character occured: {chunk}"); }
                
                stream.WriteInt(value, 6);
            }
            else
            {
                if (!Characters.TryGetValue(chunk[0], out int valueA) || !Characters.TryGetValue(chunk[1], out int valueB))
                { throw new ArgumentException($"Invalid character occured: {chunk}"); }
                
                stream.WriteInt(valueA * 45 + valueB, 11);
            }
        }

        return stream;
    }

    public BitStream GenerateHeader(QRCodeVersion version, string input)
    {
        var stream = new BitStream();
        stream.WriteBits([false, false, true, false]);
        
        stream.WriteInt(input.Length, CalculateFieldLength(version));
        return stream;
    }

    private int CalculateFieldLength(int version)
    {
        return version switch
        {
            < 10 => 9,
            < 27 => 11,
            _ => 13
        };
    }
}