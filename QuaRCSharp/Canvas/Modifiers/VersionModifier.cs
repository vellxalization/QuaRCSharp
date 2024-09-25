using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Canvas.Modifiers;

/// <summary>
/// Modifier for adding blocks about version on a canvas (version 7 and higher)
/// </summary>
public class VersionModifier : ICanvasModifier
{
    public void ModifyCanvas(ref QRCanvas canvas)
    {
        if (canvas.Data.Version < 7)
        { throw new ArgumentException("Cannot generate version blocks for provided version"); }
        
        string code = GetCode(canvas.Data.Version);
        AddCodeToBottomLeft(canvas, code);
        AddCodeToTopRight(canvas, code);
    }

    private void AddCodeToBottomLeft(QRCanvas canvas, string code)
    {
        int pointer = 0;
        for (int y = canvas.Size - 11; y < canvas.Size - 8; ++y)
        {
            for(int x = 0; x < 6; ++x)
            { canvas.SetBit((x, y), code[pointer++] is '1' ? CanvasBitValue.True : CanvasBitValue.False, true); }
        }
    }
    
    private void AddCodeToTopRight(QRCanvas canvas, string code)
    {
        int pointer = 0;
        for (int x = canvas.Size - 11; x < canvas.Size - 8; ++x)
        {
            for(int y = 5; y >= 0; --y)
            { canvas.SetBit((x, y), code[pointer++] is '1' ? CanvasBitValue.True : CanvasBitValue.False, true); }
        }
    }

    private string GetCode(QRCodeVersion version)
    {
        return (int)version switch
        {
            7 => "000010011110100110",
            8 => "010001011100111000",
            9 => "110111011000000100",
            10 => "101001111110000000",
            11 => "001111111010111100",
            12 => "001101100100011010",
            13 => "101011100000100110",
            14 => "110101000110100010",
            15 => "010011000010011110",
            16 => "011100010001011100",
            17 => "111010010101100000",
            18 => "100100110011100100",
            19 => "000010110111011000",
            20 => "000000101001111110",
            21 => "100110101101000010",
            22 => "111000001011000110",
            23 => "011110001111111010",
            24 => "001101001101100100",
            25 => "101011001001011000",
            26 => "110101101111011100",
            27 => "010011101011100000",
            28 => "010001110101000110",
            29 => "110111110001111010",
            30 => "101001010111111110",
            31 => "001111010011000010",
            32 => "101000011000101101",
            33 => "001110011100010001",
            34 => "010000111010010101",
            35 => "110110111110101001",
            36 => "110100100000001111",
            37 => "010010100100110011",
            38 => "001100000010110111",
            39 => "101010000110001011",
            40 => "111001000100010101",
            _ => throw new ArgumentOutOfRangeException(nameof(version), version, "Can't create version code for provided version.")
        };
    }
}