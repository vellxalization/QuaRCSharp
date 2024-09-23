using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Data.Encoding;

public interface IEncodingMethod
{
    public BitStream Encode(string input);
    public BitStream GenerateHeader(QRCodeVersion version, string input);
}