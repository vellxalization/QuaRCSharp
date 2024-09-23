using QuaRCSharp.Canvas.Masking;

namespace QuaRCSharp.Canvas;

public class MaskedQRCanvas : QRCanvas
{
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