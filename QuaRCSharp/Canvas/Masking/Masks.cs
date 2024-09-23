namespace QuaRCSharp.Canvas.Masking;

public enum MaskNumber
{
    Unmasked = -1,
    Zero = 0,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven
};

public abstract class Mask(MaskNumber number)
{
    public MaskNumber Number { get; } = number;
    public CanvasBit ApplyMaskToBit(CanvasBit bit) => ShouldInvert(bit.Position) ? InvertCanvasBit(bit) : bit;
    
    protected abstract Predicate<(int X, int Y)> ShouldInvert { get; }
    protected CanvasBit InvertCanvasBit(CanvasBit bit) => bit.Value switch
    {
        CanvasBitValue.False => bit with { Value = CanvasBitValue.True },
        CanvasBitValue.True => bit with { Value = CanvasBitValue.False },
        _ => bit
    };
}

public static class Masks
{
    public static Mask GetUnmasked { get; } = new Unmasked();
    public static Mask GetMaskZero { get; } = new MaskZero();
    public static Mask GetMaskOne { get; } = new MaskOne();
    public static Mask GetMaskTwo { get; } = new MaskTwo();
    public static Mask GetMaskThree { get; } = new MaskThree();
    public static Mask GetMaskFour { get; } = new MaskFour();
    public static Mask GetMaskFive { get; } = new MaskFive();
    public static Mask GetMaskSix { get; } = new MaskSix();
    public static Mask GetMaskSeven { get; } = new MaskSeven();

    public static Mask GetMaskByNumber(MaskNumber number)
    {
        return number switch
        {
            MaskNumber.Unmasked => GetUnmasked,
            MaskNumber.Zero => GetMaskZero,
            MaskNumber.One => GetMaskOne,
            MaskNumber.Two => GetMaskTwo,
            MaskNumber.Three => GetMaskThree,
            MaskNumber.Four => GetMaskFour,
            MaskNumber.Five => GetMaskFive,
            MaskNumber.Six => GetMaskSix,
            _ => GetMaskSeven
        };
    }
    
    private class Unmasked() : Mask(MaskNumber.Unmasked)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = _ => false;
    }

    private class MaskZero() : Mask(MaskNumber.Zero)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = position => (position.X+position.Y) % 2 == 0;
    }

    private class MaskOne() : Mask(MaskNumber.One)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = position => position.Y % 2 == 0;
    }

    private class MaskTwo() : Mask(MaskNumber.Two)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = position => position.X % 3 == 0;
    }

    private class MaskThree() : Mask(MaskNumber.Three)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = position => (position.X + position.Y) % 3 == 0;
    }

    private class MaskFour() : Mask(MaskNumber.Four)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = position => (position.X / 3 + position.Y / 2) % 2 == 0;
    }

    private class MaskFive() : Mask(MaskNumber.Five)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = position => position.X * position.Y % 2 + position.X * position.Y % 3 == 0;
    }

    private class MaskSix() : Mask(MaskNumber.Six)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = position => (position.X * position.Y % 2 + position.X * position.Y % 3) % 2 == 0;
    }

    private class MaskSeven() : Mask(MaskNumber.Seven)
    {
        protected override Predicate<(int X, int Y)> ShouldInvert { get; } = position => (position.X * position.Y % 3 + position.X + position.Y % 2) % 2 == 0;
    }
}