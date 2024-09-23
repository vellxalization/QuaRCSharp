namespace QuaRCSharp.Canvas.Modifiers;

public class SynchronizationStripModifier : ICanvasModifier
{
    public void ModifyCanvas(ref QRCanvas canvas)
    {
        AddHorizontalStrip(canvas);
        AddVerticalStrip(canvas);
    }
    
    private void AddHorizontalStrip(QRCanvas canvas)
    {
        bool setBlack = true;
        for (int x = 8; x < canvas.Size - 8; ++x)
        {
            canvas.SetBit((x,6), setBlack ? CanvasBitValue.True : CanvasBitValue.False, true);
            setBlack = !setBlack;
        }
    }
    
    private void AddVerticalStrip(QRCanvas canvas)
    {
        bool setBlack = true;
        for (int y = 8; y < canvas.Size - 8; ++y)
        {
            canvas.SetBit((6, y), setBlack ? CanvasBitValue.True : CanvasBitValue.False, true);
            setBlack = !setBlack;
        }
    }
}