namespace QuaRCSharp.Data;

/// <summary>
/// Class for storing data as a bit sequence
/// </summary>
public class BitStream
{
    private List<byte> _stream = [0, 0];
    private int _capacity = 8 * 2;
    
    /// <summary>
    /// Length of the stream
    /// </summary>
    public int Length { get; private set; }
    /// <summary>
    /// Zero-based position of the pointer
    /// </summary>
    public int Pointer { get; set; }

    /// <summary>
    /// Writes data bit at the pointer position
    /// </summary>
    /// <param name="bit">Single bit</param>
    public void WriteBit(bool bit)
    {
        if (Pointer + 1 > _capacity)
        { EnlargeStream(); }
        
        var divRem = Math.DivRem(Pointer, 8);
        if (bit)
        { _stream[divRem.Quotient] |= (byte)(1 << (7 - divRem.Remainder)); }
        else
        { _stream[divRem.Quotient] &= (byte)(~(1 << (7 - divRem.Remainder))); }
        
        MoveForward(1);
    }

    /// <summary>
    /// Writes an array of single data bits at the pointer position
    /// </summary>
    /// <param name="bits">Array of bits</param>
    public void WriteBits(bool[] bits)
    {
        foreach(bool bit in bits)
        { WriteBit(bit); }
    }
    
    /// <summary>
    /// Writes a byte at the pointer position
    /// </summary>
    /// <param name="newValue">Byte to write</param>
    public void WriteByte(byte newValue)
    {
        if (Pointer + 8 > _capacity)
        { EnlargeStream(); }
        
        var divRem = Math.DivRem(Pointer, 8);
        if (divRem.Remainder is 0)
        {
            // write full byte
            _stream[divRem.Quotient] = newValue;
            MoveForward(8);
            return;
        }
        
        byte mask = (byte)(0b_1111_1111 >> divRem.Remainder); // mask for reading first N bits from newValue
        byte currentByteValue = _stream[divRem.Quotient];
        currentByteValue &= (byte)~mask; // discarding last N bits of current byte
        currentByteValue |= (byte)((newValue & (mask << divRem.Remainder)) >> divRem.Remainder);  // read first N bits of newValue and
        // insert them at the end of the current byte
        _stream[divRem.Quotient] = currentByteValue;
        MoveForward(8 - divRem.Remainder);
        
        currentByteValue = _stream[divRem.Quotient + 1]; // get next byte
        currentByteValue &= mask; // discard first (8 - N) bits using previous mask
        mask = (byte)(0b_1111_1111 >> (8 - divRem.Remainder)); // create new mask for reading last (8 - N) bits from newValue
        currentByteValue |= (byte)((newValue & mask) << (8 - divRem.Remainder)); // read last (8 - N) bits from newValue and 
        // insert them at the start of the current byte
        _stream[divRem.Quotient + 1] = currentByteValue;
        MoveForward(divRem.Remainder);
    }
    
    /// <summary>
    /// Writes an array of bytes at the pointer position
    /// </summary>
    /// <param name="bytes">Array of bytes</param>
    public void WriteBytes(byte[] bytes)
    {
        foreach(byte @byte in bytes)
        { WriteByte(@byte); }
    }
    
    /// <summary>
    /// Reads a single bit of data at the pointer position
    /// </summary>
    /// <returns>Single bit</returns>
    /// <exception cref="EndOfStreamException">Thrown if the pointer is outside the stream</exception>
    public bool ReadBit()
    {
        if (!CanRead(1))
        { throw new EndOfStreamException(); }
        
        var divRem = Math.DivRem(Pointer, 8);
        var bit = (_stream[divRem.Quotient] & (byte)(1 << (7 - divRem.Remainder))) > 0;
        MoveForward(1);
        
        return bit;
    }

    /// <summary>
    /// Reads a single byte of data at the pointer position
    /// </summary>
    /// <returns>Single byte of data</returns>
    /// /// <exception cref="EndOfStreamException">Thrown if pointer's final position exceeds length of the stream</exception>
    public byte ReadByte()
    {
        if (!CanRead(8))
        { throw new EndOfStreamException(); }
        
        var divRem = Math.DivRem(Pointer, 8);
        byte result = 0;
        if (divRem.Remainder is 0)
        {
            result = _stream[divRem.Quotient];
            MoveForward(8);
            return result;
        }
        
        byte mask = (byte)(0b_1111_1111 >> divRem.Remainder); // mask for reading N last bits of current byte
        result |= (byte)((_stream[divRem.Quotient] & mask) << divRem.Remainder); // read N last bits and insert them at the beginning
        MoveForward(8 - divRem.Remainder);
        
        mask = (byte)(0b_1111_1111 << (8 - divRem.Remainder)); // mask for reading (8 - N) first bits of the next byte
        result |= (byte)((_stream[divRem.Quotient + 1] & mask) >> (8 - divRem.Remainder)); // read (8 - N) first bits of the next byte
        // and insert them at the end
        MoveForward(divRem.Remainder);
        
        return result;
    }
    
    /// <summary>
    /// Writes an integer value using provided amount of bits at the pointer position 
    /// </summary>
    /// <param name="newValue">Integer to write</param>
    /// <param name="numberOfBitsToUse">Number of bits to use</param>
    /// <exception cref="ArgumentException">Thrown if provided value cannot be written with provided amount of bits</exception>
    public void WriteInt(int newValue, int numberOfBitsToUse)
    {
        if (numberOfBitsToUse is < 1 or > 32)
        { throw new ArgumentException("Can't write int using provided amount of bits"); }
        
        if (newValue > (0b_11111111_11111111_11111111_11111111 >> (32 - numberOfBitsToUse)))
        { throw new ArgumentException("Can't fully write int using provided amount of bits"); }

        if (Pointer + numberOfBitsToUse > _capacity)
        { EnlargeStream((int)Math.Ceiling((decimal)numberOfBitsToUse / 8)); }
        
        if (numberOfBitsToUse is 8)
        {
            WriteByte((byte)(newValue & 0b_1111_1111));
            return;
        }

        if (numberOfBitsToUse is 1)
        {
            WriteBit(newValue is 1);
            return;
        }

        while (numberOfBitsToUse > 0)
        {
            var divRem = Math.DivRem(Pointer, 8);
            byte currentByteValue = _stream[divRem.Quotient];
            byte mask = 0b_1111_1111;

            if (numberOfBitsToUse == (8 - divRem.Remainder))
            {
                // current byte can fully accomodate remaining bits and pointer will move to the next byte in the stream
                mask >>= divRem.Remainder;
                currentByteValue &= (byte)(~mask);
                currentByteValue |= (byte)(newValue & mask);
                _stream[divRem.Quotient] = currentByteValue;
                MoveForward(numberOfBitsToUse);
                return;
            }

            if (numberOfBitsToUse < (8 - divRem.Remainder))
            {
                // current byte can fully accomodate remaining bits and pointer will remain on the current byte
                mask >>= (8 - numberOfBitsToUse);
                currentByteValue &= (byte)(~(mask << (8 - numberOfBitsToUse)));
                currentByteValue |= (byte)((mask & newValue) << (8 - numberOfBitsToUse));
                _stream[divRem.Quotient] = currentByteValue;
                MoveForward(numberOfBitsToUse);
                return;
            }
            
            // current byte cannot fully accomodate remaining bits
            mask >>= divRem.Remainder;
            currentByteValue &= (byte)(~mask);
            currentByteValue |= (byte)(mask & (newValue >> (numberOfBitsToUse - (8 - divRem.Remainder))));
            _stream[divRem.Quotient] = currentByteValue;
            MoveForward(8 - divRem.Remainder);
            numberOfBitsToUse -= (8 - divRem.Remainder);
        }
    }

    /// <summary>
    /// Writes another bit stream at the pointer position
    /// </summary>
    /// <param name="stream">Stream to write</param>
    public void WriteBitStream(BitStream stream)
    {
        stream.Pointer = 0;
        while (stream.CanRead(8))
        { WriteByte(stream.ReadByte()); }
        while (stream.CanRead(1))
        { WriteBit(stream.ReadBit()); }
    }
    
    /// <summary>
    /// Checks if stream can read data from the pointer position
    /// </summary>
    /// <param name="positions">Number of bits to read forward</param>
    public bool CanRead(int positions) => (Pointer + positions) <= Length;
    
    private void MoveForward(int positions)
    {
        Pointer += positions;
        if (Pointer > Length)
        { Length = Pointer; }
    }
    
    private void EnlargeStream(int numberOfBytes = 1)
    {
        _stream.AddRange(Enumerable.Repeat((byte)0, numberOfBytes));
        _capacity += 8 * numberOfBytes;
    }
}