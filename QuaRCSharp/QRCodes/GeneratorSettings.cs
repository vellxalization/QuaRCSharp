namespace QuaRCSharp.QRCodes;

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