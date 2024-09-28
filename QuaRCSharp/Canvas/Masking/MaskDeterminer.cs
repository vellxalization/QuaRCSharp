using QuaRCSharp.Canvas.Modifiers;

namespace QuaRCSharp.Canvas.Masking;

/// <summary>
/// Class for automatic determination of the best mask for the canvas
/// </summary>
public class MaskDeterminer
{
    /// <summary>
    /// Determines the best mask for a given canvas by calculating the penalty for each mask and selecting the one with the lowest penalty
    /// </summary>
    /// <param name="canvas">Unmasked and borderless canvas</param>
    /// <returns>New instance of a MaskedQRCanvas canvas</returns>
    public MaskedQRCanvas ApplyBestMask(QRCanvas canvas)
    {
        int minPenalty = int.MaxValue;
        QRCanvas? bestCanvas = null;
        
        for (int i = 0; i < 8; ++i)
        {
            Mask mask = Masks.GetMaskByNumber((MaskNumber)i);
            QRCanvas maskedCanvas = new MaskedQRCanvas(canvas, mask);
            
            FormatModifier formatMod = new FormatModifier(canvas.Data.CorrectionLevel, mask);
            formatMod.ModifyCanvas(ref maskedCanvas);
            
            int penalty = CountPenaltyPoints(maskedCanvas);
            if (penalty < minPenalty)
            {
                bestCanvas = maskedCanvas;
                minPenalty = penalty;
            }
        }
        
        return (MaskedQRCanvas)bestCanvas!;
    }

    private int CountPenaltyPoints(QRCanvas maskedCanvas)
    {
        // Add a penalty for each sequence of white or black bits that is longer than 5
        int penalty = CountLongNonAlternatePenalty(maskedCanvas, false);
        penalty += CountLongNonAlternatePenalty(maskedCanvas, true);
        
        // Add penalty for every WWWWBWBBBWB or BWBBBWBWWWW pattern
        penalty += CountPatternPenalty(maskedCanvas, false);
        penalty += CountPatternPenalty(maskedCanvas, true);
        
        // Add penalty for every 2x2 area of the same color
        penalty += CountSquarePenalty(maskedCanvas);
        // Add penalty for a significant dominance of one color over another
        penalty += CountRatioPenalty(maskedCanvas);

        return penalty;
    }

    private int CountLongNonAlternatePenalty(QRCanvas canvas, bool scanVertically)
    {
        (int X, int Y) position = (0, 0);
        
        int penalty = 0;
        CanvasBitValue? comparableValue = null;
        int currentStreak = 0;
        
        ref int mainAxis = ref scanVertically ? ref position.Y : ref position.X;
        ref int secondaryAxis = ref scanVertically ? ref position.X : ref position.Y;

        while (secondaryAxis < canvas.Size)
        {
            if (mainAxis >= canvas.Size)
            {
                if (currentStreak > 5)
                { penalty += currentStreak - 2; }

                currentStreak = 0;
                comparableValue = null;
                mainAxis = 0;
                ++secondaryAxis;
                continue;
            }
            
            CanvasBitValue currentValue = canvas.GetBit(position).Value;
            if (comparableValue is null)
            {
                comparableValue = currentValue;
                currentStreak = 1;
                ++mainAxis;
                continue;
            }

            if (comparableValue == currentValue)
            { ++currentStreak; }
            else
            {
                if (currentStreak > 5)
                { penalty += currentStreak - 2; }

                currentStreak = 1;
                comparableValue = currentValue;
            }

            ++mainAxis;
        }

        return penalty;
    }

    private int CountSquarePenalty(QRCanvas maskedCanvas)
    {
        int penalty = 0;
        for (int x = 0; x < maskedCanvas.Size - 1; ++x)
        {
            for (int y = 0; y < maskedCanvas.Size - 1; ++y)
            {
                // bootleg way of checking, if 2x2 area contains bits of same value
                if (maskedCanvas.GetBit((x, y)).Value is CanvasBitValue.False &&
                    maskedCanvas.GetBit((x + 1, y)).Value is CanvasBitValue.False &&
                    maskedCanvas.GetBit((x, y + 1)).Value is CanvasBitValue.False &&
                    maskedCanvas.GetBit((x + 1, y + 1)).Value is CanvasBitValue.False)
                { penalty += 3; }
                else if (maskedCanvas.GetBit((x, y)).Value is CanvasBitValue.True &&
                         maskedCanvas.GetBit((x + 1, y)).Value is CanvasBitValue.True &&
                         maskedCanvas.GetBit((x, y + 1)).Value is CanvasBitValue.True &&
                         maskedCanvas.GetBit((x + 1, y + 1)).Value is CanvasBitValue.True)
                { penalty += 3; }
            }
        }

        return penalty;
    }

    private int CountPatternPenalty(QRCanvas canvas, bool scanVertically)
    {
        int penalty = 0;
        (int X, int Y) position = (0, 0);
        CanvasBitValue[] blackWhiteBlackPattern = [CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True];
        CanvasBitValue[] whitePattern = [CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False, CanvasBitValue.False];
        
        ref int mainAxis = ref scanVertically ? ref position.Y : ref position.X;
        ref int secondaryAxis = ref scanVertically ? ref position.X : ref position.Y;

        while (secondaryAxis < canvas.Size)
        {
            if ((mainAxis + 11) >= canvas.Size)
            {
                // if there are less than 11 (length of the whole pattern) bits in row/column to check - skip them
                mainAxis = 0;
                ++secondaryAxis;
                continue;
            }
            
            CanvasBitValue value = canvas.GetBit(position).Value;
            if (value is CanvasBitValue.True)
            {
                if (!TryMatchPattern(blackWhiteBlackPattern, ref mainAxis))
                { continue; }

                if (!TryMatchPattern(whitePattern, ref mainAxis))
                { continue; }

                penalty += 40;
            }
            else if (value is CanvasBitValue.False)
            {
                if (!TryMatchPattern(whitePattern, ref mainAxis))
                { continue; }

                if (!TryMatchPattern(blackWhiteBlackPattern, ref mainAxis))
                { continue; }

                penalty += 40;
            }
        }
        
        return penalty;
        
        bool TryMatchPattern(CanvasBitValue[] pattern, ref int mainAxis)
        {
            foreach (CanvasBitValue bit in pattern)
            {
                if (canvas.GetBit(position).Value != bit)
                { return false; }

                ++mainAxis;
            }
            
            return true;
        }
    }

    
    private int CountRatioPenalty(QRCanvas maskedCanvas)
    {
        int blackCount = 0;
        foreach (var bit in maskedCanvas.GetReadingEnumerator())
        {
            if (bit.Value is CanvasBitValue.True)
            { ++blackCount; }
        }
        
        int penalty = (int)(blackCount / Math.Pow(maskedCanvas.Size, 2) * 100); // percent of black blocks
        penalty = Math.Abs(penalty - (100 - penalty)); // (100 - penalty) = percent of white blocks;
                                                       // penalty = difference between black and white ratio
        penalty = (penalty - 10) / 2; // margin of error = 10% (from 45 to 55); 
        if (penalty <= 0) 
        { return 0; }   // if penalty score is within the margin of error then give no penalty;
                        // otherwise return 10 points for every percent outside of margin of error
        return penalty * 10;
    }
}