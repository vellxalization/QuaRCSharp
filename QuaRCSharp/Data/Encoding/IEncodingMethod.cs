using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Data.Encoding;

/// <summary>
/// An interface for data encoders
/// </summary>
public interface IEncodingMethod
{
    /// <summary>
    /// Encodes input string
    /// </summary>
    /// <param name="input">String to encode</param>
    /// <returns>Bit stream with encoded data</returns>
    public BitStream Encode(string input);
    /// <summary>
    /// Generates a service header containing information about data encoding
    /// </summary>
    /// <param name="version">Version of QR-Code</param>
    /// <param name="input">String to encode</param>
    /// <returns>Bit stream with service header</returns>
    public BitStream GenerateHeader(QRCodeVersion version, string input);
}