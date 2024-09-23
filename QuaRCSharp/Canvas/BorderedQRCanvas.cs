namespace QuaRCSharp.Canvas;

public class BorderedQRCanvas : QRCanvas
{
    public BorderedQRCanvas(QRCanvas canvas) : base(canvas.Data)
    {
        if (canvas.IsBordered)
        { throw new ArgumentException("Can't create bordered canvas from another bordered canvas"); }
        
        IsBordered = true;
        Size += 8;
        Mask = canvas.Mask;
        
        Canvas = new CanvasBit[Size, Size];
        CreateBorders();
        CopyCanvasWithOffsets(canvas);
    }

    private void CopyCanvasWithOffsets(QRCanvas canvas)
    {
        foreach (CanvasBit bit in canvas.GetReadingEnumerator())
        {
            var newPos = bit.Position;
            newPos.X += 4;
            newPos.Y += 4;
            
            Canvas[newPos.Y, newPos.X] = bit with { Position = newPos };
        }
    }

    private void CreateBorders()
    {
        for (int x = 0; x < Size; ++x)
        {
            Canvas[0, x] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (x, 0) };
            Canvas[1, x] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (x, 1) };
            Canvas[2, x] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (x, 2) };
            Canvas[3, x] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (x, 3) };
            
            Canvas[Size - 4, x] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (x, Size - 4) };
            Canvas[Size - 3, x] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (x, Size - 3) };
            Canvas[Size - 2, x] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (x, Size - 2) };
            Canvas[Size - 1, x] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (x, Size - 1) };
        }

        for (int y = 4; y < Size - 4; ++y)
        {
            Canvas[y, 0] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (0, y) };
            Canvas[y, 1] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (1, y) };
            Canvas[y, 2] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (2, y) };
            Canvas[y, 3] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (3, y) };
            
            Canvas[y, Size - 4] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (Size - 4, y) };
            Canvas[y, Size - 3] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (Size - 3, y) };
            Canvas[y, Size - 2] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (Size - 2, y) };
            Canvas[y, Size - 1] = new CanvasBit() { Value = CanvasBitValue.False, IsService = true, Position = (Size - 1, y) };
        }
    }
}