using QuaRCSharp.Canvas.Masking;
using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Canvas.Modifiers;

public class FormatModifier : ICanvasModifier
{
    private readonly string _code;
    
    public FormatModifier(CorrectionLevel correction, Mask mask)
    {
        _code = GetCode(correction, mask);
    }
    
    public void ModifyCanvas(ref QRCanvas canvas)
    {
        AddTopLeft(canvas);
        AddBottomLeftAndTopRight(canvas);
    }

    private void AddTopLeft(QRCanvas canvas)
    {
        int pointer = 0;
        for (int x = 0; x < 9; ++x)
        {
            if (x == 6)
            { continue; }

            canvas.SetBit((x, 8), _code[pointer++] is '1' ? CanvasBitValue.True : CanvasBitValue.False, true);
        }

        for (int y = 7; y >= 0; --y)
        {
            if (y == 6)
            { continue; }

            canvas.SetBit((8, y), _code[pointer++] is '1' ? CanvasBitValue.True : CanvasBitValue.False, true);
        }
    }
    
    private void AddBottomLeftAndTopRight(QRCanvas canvas)
    {
        int pointer = 0;
        for (int y = canvas.Size - 1; y > canvas.Size - 8; --y)
        { canvas.SetBit((8, y), _code[pointer++] is '1' ? CanvasBitValue.True : CanvasBitValue.False, true); }
    
        for (int x = canvas.Size - 8; x < canvas.Size; ++x)
        { canvas.SetBit((x, 8), _code[pointer++] is '1' ? CanvasBitValue.True : CanvasBitValue.False, true); }
    }
    
    private string GetCode(CorrectionLevel correction, Mask mask)
    {
        return correction switch
        {
            CorrectionLevel.L => mask.Number switch
            {
                MaskNumber.Zero => "111011111000100",
                MaskNumber.One => "111001011110011",
                MaskNumber.Two => "111110110101010",
                MaskNumber.Three => "111100010011101",
                MaskNumber.Four => "110011000101111",
                MaskNumber.Five => "110001100011000",
                MaskNumber.Six => "110110001000001",
                MaskNumber.Seven => "110100101110110",
                _ => "000000000000000"
            },
            CorrectionLevel.M => mask.Number switch
            {
                MaskNumber.Zero => "101010000010010",
                MaskNumber.One => "101000100100101",
                MaskNumber.Two => "101111001111100",
                MaskNumber.Three => "101101101001011",
                MaskNumber.Four => "100010111111001",
                MaskNumber.Five => "100000011001110",
                MaskNumber.Six => "100111110010111",
                MaskNumber.Seven => "100101010100000",
                _ => "000000000000000"
            },
            CorrectionLevel.Q => mask.Number switch
            {
                MaskNumber.Zero => "011010101011111",
                MaskNumber.One => "011000001101000",
                MaskNumber.Two => "011111100110001",
                MaskNumber.Three => "011101000000110",
                MaskNumber.Four => "010010010110100",
                MaskNumber.Five => "010000110000011",
                MaskNumber.Six => "010111011011010",
                MaskNumber.Seven => "010101111101101",
                _ => "000000000000000"
            },
            _ => mask.Number switch
            {
                MaskNumber.Zero => "001011010001001",
                MaskNumber.One => "001001110111110",
                MaskNumber.Two => "001110011100111",
                MaskNumber.Three => "001100111010000",
                MaskNumber.Four => "000011101100010",
                MaskNumber.Five => "000001001010101",
                MaskNumber.Six => "000110100001100",
                MaskNumber.Seven => "000100000111011",
                _ => "000000000000000"
            }
        };
    }
}