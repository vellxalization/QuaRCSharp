namespace QuaRCSharp.Canvas.Modifiers;

public class FinderModifier : ICanvasModifier
{
    private static readonly bool[,] FinderPattern = 
    {
        { true, true, true, true, true, true, true },
        { true, false, false, false, false, false, true },
        { true, false, true, true, true, false, true },
        { true, false, true, true, true, false, true },
        { true, false, true, true, true, false, true },
        { true, false, false, false, false, false, true },
        { true, true, true, true, true, true, true },
    };
    
    public void ModifyCanvas(ref QRCanvas canvas)
    {
        CreateBackground(canvas,0, 0);
        CreateBackground(canvas,0, canvas.Size - 8);
        CreateBackground(canvas,canvas.Size - 8, 0);
        
        AddPattern(canvas,0, 0);
        AddPattern(canvas,0, canvas.Size - 7);
        AddPattern(canvas,canvas.Size - 7, 0);
        canvas.SetBit((8, canvas.Size - 8), CanvasBitValue.True, true);
    }

    private void AddPattern(QRCanvas canvas, int xStart, int yStart)
    {
        for (int y = 0; y < 7; ++y)
        {
            for (int x = 0; x < 7; ++x)
            { canvas.SetBit((x + xStart, y + yStart), FinderPattern[y, x] ? CanvasBitValue.True : CanvasBitValue.False, true); }
        }
    }

    private void CreateBackground(QRCanvas canvas, int xStart, int yStart)
    {
        for (int y = 0; y < 8; ++y)
        {
            for (int x = 0; x < 8; ++x)
            { canvas.SetBit((x + xStart, y + yStart), CanvasBitValue.False, true); }
        }
    }
}