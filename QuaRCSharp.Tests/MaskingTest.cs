using QuaRCSharp.Canvas;
using QuaRCSharp.Canvas.Masking;

namespace QuaRCSharp.Tests;

public class MaskingTest
{
    // this is ugly
    [Theory]
    [InlineData(MaskNumber.Unmasked, new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False })] 
    [InlineData(MaskNumber.Zero, 
        new[]{ CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[]{ CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True },
        new[]{ CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[]{ CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True },
        new[]{ CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[]{ CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True }) ]
    [InlineData(MaskNumber.One, 
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False }) ]
    [InlineData(MaskNumber.Two, 
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False }) ]
    [InlineData(MaskNumber.Three, 
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True },
        new[] { CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True },
        new[] { CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False }) ]
    [InlineData(MaskNumber.Four, 
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False }) ]
    [InlineData(MaskNumber.Five, 
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False }) ]
    [InlineData(MaskNumber.Six, 
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True }) ]
    [InlineData(MaskNumber.Seven, 
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True },
        new[] { CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True },
        new[] { CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False },
        new[] { CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.False }) ]
    public void MaskCanvas(MaskNumber maskNumber, params CanvasBitValue[][] expectedPattern)
    {
        CanvasBit[,] canvas = new CanvasBit[6,6];
        for (int x = 0; x < 6; ++x)
        {
            for (int y = 0; y < 6; ++y)
            { canvas[x, y] = new CanvasBit() { Value = CanvasBitValue.False, Position = (x, y)}; }
        }

        Mask mask = Masks.GetMaskByNumber(maskNumber);
        for (int x = 0; x < 6; ++x)
        {
            for (int y = 0; y < 6; ++y)
            { canvas[x, y] = mask.ApplyMaskToBit(canvas[x, y]); }
        }
        
        for (int x = 0; x < 6; ++x)
        {
            for (int y = 0; y < 6; ++y)
            { Assert.True(canvas[x, y].Value == expectedPattern[x][y]); }
        }
    }
}