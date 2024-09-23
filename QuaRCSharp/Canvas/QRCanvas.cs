using QuaRCSharp.Canvas.Masking;
using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Canvas;

/// <summary>
/// Class representing a QRCode
/// </summary>
public class QRCanvas
{
    public QRCanvas(QRCodeData data)
    {
        Data = data;
        Size = 4 * data.Version + 17;
        Canvas = new CanvasBit[Size, Size];
        FillDefaultCanvas();
    }
    
    /// <summary>
    /// Actual data that needs to be written on the canvas
    /// </summary>
    public QRCodeData Data { get; }
    /// <summary>
    /// Bit height/width of the canvas (if bordered - includes borders)
    /// </summary>
    public int Size { get; protected init; }
    /// <summary>
    /// Is canvas have a 4 bit wide border (false by default)
    /// </summary>
    public bool IsBordered { get; protected init; } = false;
    /// <summary>
    /// Current mask that will be applied to all non-service bits of data (Unmasked by default)
    /// </summary>
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

    /// <summary>
    /// Gets bit from the canvas
    /// </summary>
    /// <param name="position">(X,Y) position of a bit ((0,0) is top-left)</param>
    /// <returns></returns>
    /// <exception cref="ArgumentException">Thrown in case provided position is less than 0 or exceeds size of the canvas</exception>
    public CanvasBit GetBit((int X, int Y) position)
    {
        if ((position.X < 0 || position.X >= Size) || (position.Y < 0 || position.Y >= Size))
        { throw new ArgumentException("Position out of canvas"); }

        return Canvas[position.Y, position.X];
    }

    /// <summary>
    /// Sets bit value on the canvas and automatically applies stored mask
    /// </summary>
    /// <param name="position">(X,Y) position of a bit ((0,0) is top-left)</param>
    /// <param name="value">Value to se</param>
    /// <param name="isService">Should bit be marked as service (if true - mask won't be applied to it)</param>
    /// <exception cref="Exception">Thrown in case canvas has borders</exception>
    /// <exception cref="ArgumentException">Thrown in case provided position is less than 0 or exceeds size of the canvas</exception>
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

    /// <summary>
    /// Enumerator for reading data row by row, column by column for top left corner of the canvas (0, 0)
    /// </summary>
    /// <returns>IEnumerable of bits</returns>
    public IEnumerable<CanvasBit> GetReadingEnumerator()
    {
        var enumerator = Canvas.GetEnumerator();
        enumerator.Reset();

        while (enumerator.MoveNext())
        { yield return (CanvasBit)enumerator.Current; }
    }

    /// <summary>
    /// Custom enumerator for reading data in a snake-like pattern from bottom left corner (Size - 1, Size - 1) to top right (0,0).
    /// Used for writing data
    /// </summary>
    /// <returns>IEnumerable of bits</returns>
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

/// <summary>
/// Struct that stores data on canvas
/// </summary>
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