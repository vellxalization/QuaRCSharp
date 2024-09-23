using QuaRCSharp.Canvas.Masking;

namespace QuaRCSharp.Canvas;

/// <summary>
/// A canvas with applied mask to it
/// </summary>
public class MaskedQRCanvas : QRCanvas
{
    /// <summary>
    /// Creates a copy of a canvas with applied mask to it
    /// </summary>
    /// <param name="canvas">Canvas to apply mask to</param>
    /// <param name="mask">Mask to apply to</param>
    /// <exception cref="ArgumentException">Thrown if provided canvas already masked</exception>
    public MaskedQRCanvas(QRCanvas canvas, Mask mask) : base(canvas.Data)
    {
        if (canvas.Mask.Number is not MaskNumber.Unmasked)
        { throw new ArgumentException("Cannot apply mask to an already masked canvas"); }
        
        Mask = mask;
        CopyCanvas(canvas);
    }

    private void CopyCanvas(QRCanvas canvas)
    {
        foreach (CanvasBit bit in canvas.GetReadingEnumerator())
        { SetBit(bit.Position, bit.Value, bit.IsService); }
    }
}