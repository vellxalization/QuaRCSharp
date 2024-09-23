using QuaRCSharp.Canvas.Masking;
using QuaRCSharp.Canvas.Modifiers;
using QuaRCSharp.Data;
using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Canvas;

public class QRCanvasPainter
{
    public QRCanvas Canvas => _canvas;
    private QRCanvas _canvas;
    public QRCanvasPainter(QRCanvas canvas) => _canvas = canvas;
    
    public QRCanvasPainter AddServiceInfo()
    {
        ICanvasModifier[] serviceMods = GetAllServiceModifiers(_canvas.Data);
        foreach(ICanvasModifier mod in serviceMods)
        { mod.ModifyCanvas(ref _canvas); }

        return this;
    }

    public QRCanvasPainter WriteDataToCanvas()
    {
        BitStream dataStream = _canvas.Data.DataWithErrorCorrection;
        dataStream.Pointer = 0;

        using IEnumerator<CanvasBit> enumerator = _canvas.GetWritingDataEnumerator().GetEnumerator();
        while (enumerator.MoveNext())
        {
            CanvasBit bit = enumerator.Current;
            if (bit.IsService)
            { continue; }
            
            if (!dataStream.CanRead(1))
            { break; }
            
            _canvas.SetBit(bit.Position, dataStream.ReadBit() ? CanvasBitValue.True : CanvasBitValue.False, false);
        }
        
        if (dataStream.CanRead(1))
        { throw new ArgumentException("Provided canvas is too small for provided data"); }
        
        do 
        {
            // if we've written all the data AND we have bits on canvas to spare, fill them with 0
            CanvasBit bit = enumerator.Current;
            if (bit.Value is not CanvasBitValue.None)
            { continue; }
            
            _canvas.SetBit(bit.Position, CanvasBitValue.False, false);
        } while (enumerator.MoveNext());

        return this;
    }

    public QRCanvasPainter ApplyMask(Mask mask)
    {
        _canvas = new MaskedQRCanvas(_canvas, mask);
        var mod = new FormatModifier(_canvas.Data.CorrectionLevel, mask);
        mod.ModifyCanvas(ref _canvas);
        return this;
    }
    
    public QRCanvasPainter ApplyBestMask()
    {
        MaskDeterminer maskApplier = new MaskDeterminer();
        QRCanvas masked = maskApplier.ApplyBestMask(Canvas);

        _canvas = masked;
        return this;
    }

    public QRCanvasPainter AddBorders()
    {
        _canvas = new BorderedQRCanvas(_canvas);
        return this;
    }
    
    private ICanvasModifier[] GetAllServiceModifiers(QRCodeData data)
    {
        List<ICanvasModifier> mods = [new FinderModifier(), new SynchronizationStripModifier(), new FormatModifier(data.CorrectionLevel, Masks.GetUnmasked)];
        if (data.Version >= 2)
        { mods.Add(new AlignmentModifier()); }
        if (data.Version >= 7)
        { mods.Add(new VersionModifier() ); }

        return mods.ToArray();
    }
}