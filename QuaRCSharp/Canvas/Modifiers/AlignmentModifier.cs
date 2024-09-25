using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Canvas.Modifiers;

/// <summary>
/// Modifier for adding a 5x5 alignment patterns on a canvas (version 2 and higher)
/// </summary>
public class AlignmentModifier : ICanvasModifier
{
    private static readonly bool[,] AlignmentPattern = new[,]
    {
        { true, true, true, true, true },
        { true, false, false, false, true },
        { true, false, true, false , true },
        { true, false, false, false, true },
        { true, true, true, true, true },
    };
    
    public void ModifyCanvas(ref QRCanvas canvas)
    {
        if (canvas.Data.Version < 2)
        { throw new ArgumentException("Cannot generate alignment patters for provided version"); }
        
        foreach (var position in GetAvailablePositions(canvas.Data.Version))
        {
            for(int x = 0; x < 5; ++x)
            {
                for (int y = 0; y < 5; ++y)
                { canvas.SetBit((position.X + x, position.Y + y), AlignmentPattern[y,x] ? CanvasBitValue.True : CanvasBitValue.False, true); }
            }
        }
    }
    
    private (int X, int Y)[] GetAvailablePositions(QRCodeVersion version)
    {
        int[] corners = GetTopLeftCorners(version);
        if (corners.Length is 1)
        { return [(corners[0], corners[0])]; }
        
        List<(int X, int Y)> combinations = new();
        foreach(var first in corners)
        {
            foreach (var second in corners)
            { combinations.Add((first, second)); }
        }
        
        combinations.RemoveAt(corners.Length * (corners.Length - 1)); // remove combination of last and first
        combinations.RemoveAt(corners.Length - 1); // remove combination of first and last
        combinations.RemoveAt(0); // remove combination of first and first

        return combinations.ToArray();
    }

    private int[] GetTopLeftCorners(QRCodeVersion version)
    {
        switch ((int)version)
        {
            case 2: return [16];
            case 3: return [20];
            case 4: return [24];
            case 5: return [28];
            case 6: return [32];
            case 7: return [4, 20, 36];
            case 8: return [4, 22, 40];
            case 9: return [4, 24, 44];
            case 10: return [4, 26, 48];
            case 11: return [4, 28, 52];
            case 12: return [4, 30, 56];
            case 13: return [4, 32, 60];
            case 14: return [4, 24, 44, 64];
            case 15: return [4, 24, 46, 68];
            case 16: return [4, 24, 48, 72];
            case 17: return [4, 28, 52, 76];
            case 18: return [4, 28, 54, 80];
            case 19: return [4, 28, 56, 84];
            case 20: return [4, 32, 60, 88];
            case 21: return [4, 26, 48, 70, 92];
            case 22: return [4, 24, 48, 72, 96];
            case 23: return [4, 28, 52, 76, 100];
            case 24: return [4, 26, 52, 78, 104];
            case 25: return [4, 30, 56, 82, 108];
            case 26: return [4, 28, 56, 84, 112];
            case 27: return [4, 32, 60, 88, 116];
            case 28: return [4, 24, 48, 72, 96, 120];
            case 29: return [4, 28, 52, 76, 100, 124];
            case 30: return [4, 24, 50, 76, 102, 128];
            case 31: return [4, 28, 54, 80, 106, 132];
            case 32: return [4, 32, 58, 84, 110, 136];
            case 33: return [4, 28, 56, 84, 112, 140];
            case 34: return [4, 32, 60, 88, 116, 144];
            case 35: return [4, 28, 52, 76, 100, 124, 148];
            case 36: return [4, 22, 48, 74, 100, 126, 152];
            case 37: return [4, 26, 52, 78, 104, 130, 156];
            case 38: return [4, 30, 56, 82, 108, 134, 160];
            case 39: return [4, 24, 52, 80, 108, 136, 164];
            case 40: return [4, 28, 56, 84, 112, 140, 168];
            default:
            { throw new Exception("Can't generate alignment patterns for version 1"); }
        }
    }
}