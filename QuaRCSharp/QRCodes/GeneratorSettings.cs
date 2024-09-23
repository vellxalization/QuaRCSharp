namespace QuaRCSharp.QRCodes;

/// <summary>
/// Record containing parameters for QR-Code generator
/// </summary>
/// <param name="CorrectionLevel">Desired level of error correction</param>
/// <param name="ForceByteEncoding">f true - will always use byte encoding.
/// NOTE: some QR-Code scanners were unable to read numeric encoding. Using current parameter will prevent this</param>
/// <param name="Masking">If set to Auto - will automatically determine best mask for the QR-Code</param>
public record GeneratorSettings(CorrectionLevel CorrectionLevel, bool ForceByteEncoding, MaskingOptions Masking);

public enum MaskingOptions
{
    Auto,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven
}