using QuaRCSharp.Canvas.Modifiers;

namespace QuaRCSharp.Canvas.Masking;

public class MaskDeterminer
{
    public QRCanvas ApplyBestMask(QRCanvas canvas)
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
        
        return bestCanvas!;
    }

    private int CountPenaltyPoints(QRCanvas maskedCanvas)
    {
        int penalty = CountHorizontalPenalty(maskedCanvas);
        penalty += CountVerticalPenalty(maskedCanvas);
        
        penalty += CountHorizontalStripePenalty(maskedCanvas);
        penalty += CountVerticalStripePenalty(maskedCanvas);
        
        penalty += CountSquarePenalty(maskedCanvas);
        penalty += CountRatioPenalty(maskedCanvas);

        return penalty;
    }

    private int CountHorizontalPenalty(QRCanvas maskedCanvas)
    {
        int penalty = 0;
        
        for (int y = 0; y < maskedCanvas.Size; ++y)
        {
            CanvasBitValue currentComparableValue = maskedCanvas.GetBit((0, y)).Value;
            int currentStreak = 1;
            
            for (int x = 1; x < maskedCanvas.Size; ++x)
            {
                CanvasBit bit = maskedCanvas.GetBit((x, y));
                if (bit.Value == currentComparableValue)
                {
                    ++currentStreak;
                    continue;
                }
                if (currentStreak >= 5)
                { penalty += currentStreak - 2; }
                
                currentComparableValue = bit.Value;
                currentStreak = 1;
            }
            
            if (currentStreak >= 5)
            { penalty += currentStreak - 2; }
        }

        return penalty;
    }
    
    private int CountVerticalPenalty(QRCanvas maskedCanvas)
    {
        int penalty = 0;
        
        for (int x = 0; x < maskedCanvas.Size; ++x)
        {
            CanvasBitValue currentComparableValue = maskedCanvas.GetBit((x, 0)).Value;
            int currentStreak = 1;
            
            for (int y = 1; y < maskedCanvas.Size; ++y)
            {
                CanvasBit bit = maskedCanvas.GetBit((x, y));
                if (bit.Value == currentComparableValue)
                {
                    ++currentStreak;
                    continue;
                }
                if (currentStreak >= 5)
                { penalty += currentStreak - 2; }
                
                currentComparableValue = bit.Value;
                currentStreak = 1;
            }
            
            if (currentStreak >= 5)
            { penalty += currentStreak - 2; }
        }
        
        return penalty;
    }

    private int CountSquarePenalty(QRCanvas maskedCanvas)
    {
        int numberOfSquares = 0;
        for (int x = 0; x < maskedCanvas.Size - 1; ++x)
        {
            for (int y = 0; y < maskedCanvas.Size - 1; ++y)
            {
                // bootleg way of checking, if 2x2 area contains bits of same value
                if (maskedCanvas.GetBit((x, y)).Value is CanvasBitValue.False &&
                    maskedCanvas.GetBit((x + 1, y)).Value is CanvasBitValue.False &&
                    maskedCanvas.GetBit((x, y + 1)).Value is CanvasBitValue.False &&
                    maskedCanvas.GetBit((x + 1, y + 1)).Value is CanvasBitValue.False)
                { ++numberOfSquares; }
                else if (maskedCanvas.GetBit((x, y)).Value is CanvasBitValue.True &&
                         maskedCanvas.GetBit((x + 1, y)).Value is CanvasBitValue.True &&
                         maskedCanvas.GetBit((x, y + 1)).Value is CanvasBitValue.True &&
                         maskedCanvas.GetBit((x + 1, y + 1)).Value is CanvasBitValue.True)
                { ++numberOfSquares; }
            }
        }

        return numberOfSquares * 3;
    }

    private int CountHorizontalStripePenalty(QRCanvas maskedCanvas)
    {
        int numberOfStripes = 0;
        
        for (int y = 0; y < maskedCanvas.Size; ++y)
        {
            int x = 0;
            while (x < maskedCanvas.Size)
            {
                switch (maskedCanvas.GetBit((x, y)).Value)
                {
                    case CanvasBitValue.False:
                        if (!TryMatchWhitePattern((x, y), out int? failedAt))
                        {
                            x = failedAt!.Value;
                            continue;
                        }

                        x += 4;
                        if (!TryMatchBlackWhitePattern((x, y), out failedAt))
                        {
                            x = failedAt!.Value;
                            continue;
                        }

                        x += 7;
                        ++numberOfStripes;
                        break;
                    
                    case CanvasBitValue.True:
                        if (!TryMatchBlackWhitePattern((x, y), out failedAt))
                        {
                            x = failedAt!.Value;
                            continue;
                        }

                        x += 7;
                        if (!TryMatchWhitePattern((x, y), out failedAt))
                        {
                            x = failedAt!.Value;
                            continue;
                        }

                        x += 4;
                        ++numberOfStripes;
                        break;
                    
                    default:
                        ++x;
                        break;
                }
            }
        }
        return numberOfStripes * 40;
        
        bool TryMatchWhitePattern((int X, int Y) startPosition, out int? failedAt)
        {
            failedAt = null;
            if (startPosition.X + 4 >= maskedCanvas.Size)
            {
                failedAt = maskedCanvas.Size;
                return false;
            }
            
            for (int i = startPosition.X; i < startPosition.X + 4; ++i)
            {
                if (maskedCanvas.GetBit((i, startPosition.Y)).Value is CanvasBitValue.False) 
                { continue; }
                
                failedAt = i;
                return false;
            }

            return true;
        }

        bool TryMatchBlackWhitePattern((int X, int Y ) startPosition, out int? failedAt)
        {
            failedAt = null;
            if (startPosition.X + 7 >= maskedCanvas.Size)
            {
                failedAt = maskedCanvas.Size;
                return false;
            }
            
            CanvasBitValue[] pattern = [CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True];
            for (int i = 0; i < 7; ++i)
            {
                if (maskedCanvas.GetBit((startPosition.X + i, startPosition.Y)).Value == pattern[i]) 
                { continue; }
                
                failedAt = startPosition.X + i;
                return false;
            }

            return true;
        }
    }
    
    private int CountVerticalStripePenalty(QRCanvas maskedCanvas)
    {
        int numberOfStripes = 0;
        
        for (int x = 0; x < maskedCanvas.Size; ++x)
        {
            int y = 0;
            while (y < maskedCanvas.Size)
            {
                switch (maskedCanvas.GetBit((x, y)).Value)
                {
                    case CanvasBitValue.False:
                        if (!TryMatchWhitePattern((x, y), out int? failedAt))
                        {
                            y = failedAt!.Value;
                            continue;
                        }

                        y += 4;
                        if (!TryMatchBlackWhitePattern((x, y), out failedAt))
                        {
                            y = failedAt!.Value;
                            continue;
                        }

                        y += 7;
                        ++numberOfStripes;
                        break;
                    
                    case CanvasBitValue.True:
                        if (!TryMatchBlackWhitePattern((x, y), out failedAt))
                        {
                            y = failedAt!.Value;
                            continue;
                        }

                        y += 7;
                        if (!TryMatchWhitePattern((x, y), out failedAt))
                        {
                            y = failedAt!.Value;
                            continue;
                        }

                        y += 4;
                        ++numberOfStripes;
                        break;
                    
                    default:
                        ++y;
                        break;
                }
            }
        }
        return numberOfStripes * 40;
        
        bool TryMatchWhitePattern((int X, int Y) startPosition, out int? failedAt)
        {
            failedAt = null;
            if (startPosition.Y + 4 >= maskedCanvas.Size)
            {
                failedAt = maskedCanvas.Size;
                return false;
            }
            
            for (int i = startPosition.Y; i < startPosition.Y + 4; ++i)
            {
                if (maskedCanvas.GetBit((startPosition.X, i)).Value is CanvasBitValue.False) 
                { continue; }
                
                failedAt = i;
                return false;
            }

            return true;
        }

        bool TryMatchBlackWhitePattern((int X, int Y) startPosition, out int? failedAt)
        {
            failedAt = null;
            if (startPosition.Y + 7 >= maskedCanvas.Size)
            {
                failedAt = maskedCanvas.Size;
                return false;
            }
            
            CanvasBitValue[] pattern = [CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.True, CanvasBitValue.False, CanvasBitValue.True];
            for (int i = 0; i < 7; ++i)
            {
                if (maskedCanvas.GetBit((startPosition.X, startPosition.Y + i)).Value == pattern[i]) 
                { continue; }
                
                failedAt = startPosition.Y + i;
                return false;
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