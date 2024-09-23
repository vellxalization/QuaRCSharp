namespace QuaRCSharp.QRCodes;

/// <summary>
/// Struct representing version of a QR-Code
/// </summary>
public struct QRCodeVersion
{
    private int _version;

    public QRCodeVersion(int version)
    {
        if (version is < 1 or > 40)
        { throw new ArgumentException("Invalid version"); }

        _version = version;
    }

    public static implicit operator int(QRCodeVersion qrCodeVersion) => qrCodeVersion._version;
}