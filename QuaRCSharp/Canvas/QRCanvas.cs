using QuaRCSharp.Canvas.Masking;
using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Canvas;

public class QRCanvas
{
    public QRCanvas(QRCodeData data)
    {
        Data = data;
        Size = 4 * data.Version + 17;
        Canvas = new CanvasBit[Size, Size];
        FillDefaultCanvas();
    }
    
    public QRCodeData Data { get; }
    public int Size { get; protected init; }
    public bool IsBordered { get; protected init; } = false;
    public Mask Mask { get; protected init; } = Masks.GetUnmasked;
    protected CanvasBit[,] Canvas;

    
    private void FillDefaultCanvas()
    {
        for (int y = 0; y < Size; ++y)
        {
            for(int x = 0; x < Size; ++x)
            { Canvas[y,x] = new CanvasBit(){ Value = CanvasBitValue.None, IsService = false, Position = (x,y)}; }
        }
    }

    public CanvasBit GetBit((int X, int Y) position)
    {
        if ((position.X < 0 || position.X >= Size) || (position.Y < 0 || position.Y >= Size))
        { throw new ArgumentException("Position out of canvas"); }

        return Canvas[position.Y, position.X];
    }

    public void SetBit((int X, int Y) position, CanvasBitValue value, bool isService)
    {
        if (IsBordered)
        { throw new Exception("Can't modify bordered canvas"); }
        if ((position.X < 0 || position.X >= Size) || (position.Y < 0 || position.Y >= Size))
        { throw new ArgumentException("Position out of canvas"); }

        if (isService)
        { Canvas[position.Y, position.X] = new CanvasBit(){ Position = position, IsService = isService, Value = value }; }
        else
        { Canvas[position.Y, position.X] = Mask.ApplyMaskToBit(new CanvasBit(){ Position = position, IsService = isService, Value = value }); }
    }

    public IEnumerable<CanvasBit> GetReadingEnumerator()
    {
        var enumerator = Canvas.GetEnumerator();
        enumerator.Reset();

        while (enumerator.MoveNext())
        { yield return (CanvasBit)enumerator.Current; }
    }

    public IEnumerable<CanvasBit> GetWritingDataEnumerator()
    {
        bool readToTop = true;
        for (int x = Size - 1; x >= 0; x -= 2)
        {
            if (x == 6)
            {
                x += 1;
                continue;
            }

            int y;
            if (readToTop)
            {
                for (y = Size - 1; y >= 0; --y)
                {
                    yield return GetBit((x, y));
                    yield return GetBit((x - 1, y));
                }
            }
            else
            {
                for (y = 0; y < Size; ++y)
                {
                    yield return GetBit((x, y));
                    yield return GetBit((x - 1, y));
                }
            }

            readToTop = !readToTop;
        }
    }
}

public struct CanvasBit
{
    public CanvasBit()
    { }

    public (int X, int Y) Position { get; init; } = (0, 0);
    public CanvasBitValue Value { get; init; } = CanvasBitValue.None;
    public bool IsService { get; init; } = false;
}


public enum CanvasBitValue
{
    False,
    True,
    None
}