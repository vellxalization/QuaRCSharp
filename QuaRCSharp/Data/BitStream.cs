﻿namespace QuaRCSharp.Data;

public class BitStream
{
    private List<byte> _stream = [0, 0];
    private int _capacity = 8 * 2;
    
    public int Length { get; private set; }
    public int Pointer { get; set; }

    public void WriteBit(bool bit)
    {
        if (bit)
        {
            var divRem = Math.DivRem(Pointer, 8);
            _stream[divRem.Quotient] |= (byte)(1 << (7 - divRem.Remainder));
        }
        
        MoveForward(1);
    }

    public void WriteBits(bool[] bits)
    {
        foreach(bool bit in bits)
        { WriteBit(bit); }
    }
    
    public void WriteByte(byte value)
    {
        var divRem = Math.DivRem(Pointer, 8);
        if (divRem.Remainder is 0)
        {
            // write full byte
            _stream[divRem.Quotient] = value;
            MoveForward(8);
            return;
        }

        // write fist n bits of value to current byte
        byte mask = 0b_1111_1111;
        mask <<= divRem.Remainder;
        _stream[divRem.Quotient] |= (byte)((value & mask) >> divRem.Remainder);
        MoveForward(8 - divRem.Remainder);

        // write remaining 8 - n bits of value to the next byte
        mask = 0b_1111_1111;
        mask >>= 8 - divRem.Remainder;
        _stream[divRem.Quotient + 1] |= (byte)((value & mask) << (8 - divRem.Remainder));
        MoveForward(divRem.Remainder);
    }
    
    public void WriteBytes(byte[] bytes)
    {
        foreach(byte @byte in bytes)
        { WriteByte(@byte); }
    }
    
    public bool ReadBit()
    {
        if (Pointer >= Length)
        { throw new EndOfStreamException(); }
        
        var divRem = Math.DivRem(Pointer, 8);
        var bit = (_stream[divRem.Quotient] & (byte)(1 << (7 - divRem.Remainder))) > 0;
        MoveForward(1);
        
        return bit;
    }

    public byte ReadByte()
    {
        // same algorithm as writing a byte
        var divRem = Math.DivRem(Pointer, 8);
        byte result = 0;
        if (divRem.Remainder is 0)
        {
            result = _stream[divRem.Quotient];
            MoveForward(8);
            return result;
        }
    
        
        byte mask = 0b_1111_1111;
        mask >>= divRem.Remainder;
        result |= (byte)((_stream[divRem.Quotient] & mask) << divRem.Remainder);
        MoveForward(8 - divRem.Remainder);
        
        mask = 0b_1111_1111;
        mask <<= 8 - divRem.Remainder;
        result |= (byte)((_stream[divRem.Quotient + 1] & mask) >> (8 - divRem.Remainder));
        MoveForward(divRem.Remainder);
        
        return result;
    }

    public void WriteInt(int value, int numberOfBitsToUse)
    {
        if (numberOfBitsToUse is < 1 or > 32)
        { throw new ArgumentException("Can't write int using provided amount of bits"); }

        uint mask = 0b_11111111_11111111_11111111_11111111;
        mask >>= 32 - numberOfBitsToUse;
        if (value > mask)
        { throw new ArgumentException("Can't fully write int using provided amount of bits"); }

        if (numberOfBitsToUse is 1)
        {
            WriteBit((value & mask) > 0);
            return;
        }

        if (numberOfBitsToUse is 8)
        {
            WriteByte((byte)(value & mask));
            return;
        }

        while (numberOfBitsToUse > 0)
        {
            var divRem = Math.DivRem(Pointer, 8);
            if (numberOfBitsToUse < (8 - divRem.Remainder))
            {
                // current byte can can fully accommodate remaining bits and there will be spare bits in byte
                _stream[divRem.Quotient] |= (byte)((value & mask) << (8 - divRem.Remainder - numberOfBitsToUse));
                MoveForward(numberOfBitsToUse);
                return;
            }

            // current byte can partially or fully (with no spare bits in byte remaining) accommodate remaining bits
            mask <<= (numberOfBitsToUse - (8 - divRem.Remainder));
            _stream[divRem.Quotient] |= (byte)((value & mask) >> (numberOfBitsToUse - (8 - divRem.Remainder)));

            MoveForward(8 - divRem.Remainder);
            numberOfBitsToUse -= 8 - divRem.Remainder;

            mask = 0b_11111111_11111111_11111111_11111111;
            mask >>= 32 - numberOfBitsToUse;
        }
    }

    public void WriteBitStream(BitStream stream)
    {
        stream.Pointer = 0;
        while (stream.CanRead(8))
        { WriteByte(stream.ReadByte()); }
        while (stream.CanRead(1))
        { WriteBit(stream.ReadBit()); }
    }
    
    public bool CanRead(int positions) => (Pointer + positions) <= Length;
    
    private void MoveForward(int positions)
    {
        for (int i = 0; i < positions; ++i)
        {
            if (Pointer == Length)
            {
                ++Pointer;
                ++Length;
                EnsureCapacity();
            }
            else
            { ++Pointer; }
        }
    }

    private void EnsureCapacity()
    {
        if (Pointer >= _capacity)
        {
            _stream.AddRange([0, 0]);
            _capacity += 8 * 2;
        }
    }
}