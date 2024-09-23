using QuaRCSharp.Canvas;
using QuaRCSharp.Canvas.Masking;
using QuaRCSharp.Data;
using QuaRCSharp.Data.Encoding;

namespace QuaRCSharp.QRCodes;

/// <summary>
/// Generator class used for generating QR-Codes
/// </summary>
public class QRCodeGenerator
{
    private DataEncoder _encoder = new();
    private CorrectionByteGenerator _errorCorrection = new();

    /// <summary>
    /// Generates a QR-Code from the input
    /// </summary>
    /// <param name="input">Data</param>
    /// <param name="settings">Parameters to use in the process of generating</param>
    /// <returns>Ready-to-export instance of QRCanvas with service info, data written, mask applied and borders added</returns>
    public QRCanvas Generate(string input, GeneratorSettings settings)
    {
        DataEncoder.EncodedDataWithHeader encodedData = _encoder.EncodeInput(input, settings.ForceByteEncoding, settings.CorrectionLevel);
        
        byte[][] bodyChain = ByteChain.CreateByteChainFromBitStream(encodedData.EncodedHeaderWithPaddedData, encodedData.Version, encodedData.Correction);
        byte[][] errorCorrectionChain = _errorCorrection.CreateErrorCorrectionBytesForByteChain(bodyChain, encodedData.Version, encodedData.Correction);

        BitStream dataWithErrorCorrection = ZipBodyAndErrorCorrectionChains(bodyChain, errorCorrectionChain);
        var qrCodeData = new QRCodeData(encodedData.Version, dataWithErrorCorrection, encodedData.Correction);
        var canvas = new QRCanvas(qrCodeData);
        var painter = new QRCanvasPainter(canvas);

        painter = painter.AddServiceInfo().WriteDataToCanvas();
        painter = settings.Masking is MaskingOptions.Auto ? painter.ApplyBestMask() : painter.ApplyMask(Masks.GetMaskByNumber((MaskNumber)settings.Masking));
        painter = painter.AddBorders();

        return painter.Canvas;
    }
    
    
    private BitStream ZipBodyAndErrorCorrectionChains(byte[][] bodyChain, byte[][] errorCorrectionChain)
    {
        BitStream stream = new();
        for (int i = 0; i < bodyChain[^1].Length; ++i)
        {
            foreach (byte[] chain in bodyChain)
            {
                if (i >= chain.Length)
                { continue; }
                
                stream.WriteByte(chain[i]);
            }
        }

        for (int i = 0; i < errorCorrectionChain[0].Length; ++i)
        {
            foreach(byte[] chain in errorCorrectionChain)
            { stream.WriteByte(chain[i]); }
        }

        return stream;
    }
}