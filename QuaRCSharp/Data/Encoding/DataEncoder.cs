﻿using QuaRCSharp.QRCodes;

namespace QuaRCSharp.Data.Encoding;

public class DataEncoder
{
    private readonly ByteEncoder _byteEncoder = new();
    private readonly AlphanumericEncoder _alphanumericEncoder = new();
    private readonly NumericEncoder _numericEncoder = new();
    
    public EncodedDataWithHeader EncodeInput(string input, bool forceByteEncoding, CorrectionLevel correction)
    {
        IEncodingMethod encoder =  forceByteEncoding ? _byteEncoder : GetBestEncoderForInput(input);
        BitStream encodedContent = encoder.Encode(input);
        BitStream header = CreateHeader(input, correction, encodedContent, encoder, out QRCodeVersion version);
        AddPaddingToContent(header.Length + encodedContent.Length, QRCodeData.GetAvailableDataBits(version, correction), encodedContent);
        
        header.WriteBitStream(encodedContent);
        return new EncodedDataWithHeader(header, version, correction);
    }

    private void AddPaddingToContent(int lengthOfHeaderAndContent, int maxLength, BitStream encodedContent)
    {
        if (lengthOfHeaderAndContent > maxLength)
        { throw new ArgumentException("Argument lengthOfHeaderAndContent exceeds argument maxLength"); }
        
        if (lengthOfHeaderAndContent == maxLength)
        { return; }

        if (lengthOfHeaderAndContent % 8 != 0)
        {
            var padding = Enumerable.Repeat(false, 8 - (lengthOfHeaderAndContent % 8)).ToArray();
            lengthOfHeaderAndContent += padding.Length;
            encodedContent.WriteBits(padding);
        }
    
        if (lengthOfHeaderAndContent > maxLength)
        { throw new Exception("Something went terribly wrong, while adding first padding for bit stream"); }
    
        if (lengthOfHeaderAndContent == maxLength)
        { return; }

        bool alternate = false;
        while (lengthOfHeaderAndContent < maxLength)
        {
            lengthOfHeaderAndContent += 8;
            encodedContent.WriteByte(alternate ? (byte)17 : (byte)236);
            alternate = !alternate;
        }
    }
    
    private BitStream CreateHeader(string input, CorrectionLevel correction, BitStream content, IEncodingMethod encoder, out QRCodeVersion version)
    {
        version = GetOptimalVersion(content.Length, correction);
        BitStream header = encoder.GenerateHeader(version, input);
        int maxLength = QRCodeData.GetAvailableDataBits(version, correction);
        
        int length = content.Length + header.Length;
        if (length > maxLength)
        {
            version = new QRCodeVersion(version + 1); 
            header = encoder.GenerateHeader(version, input); 
        }
        
        return header;
    }
    
    private IEncodingMethod GetBestEncoderForInput(string input)
    {
        bool isNumeric = true;
        bool isAlphaNumeric = true;
        foreach (char character in input)
        {
            if (isNumeric && !char.IsDigit(character))
            { isNumeric = false; }

            if (isAlphaNumeric && !AlphanumericEncoder.Characters.ContainsKey(char.ToUpper(character)))
            { isAlphaNumeric = false; }

            if (isAlphaNumeric is false && isNumeric is false)
            { return _byteEncoder; }
        }

        return isNumeric ? _numericEncoder : _alphanumericEncoder;
    }
    
    private QRCodeVersion GetOptimalVersion(int bitLength, CorrectionLevel correction)
    {
        for (int i = 0; i < 40; ++i)
        {
            if (bitLength < QRCodeData.GetAvailableDataBits(new QRCodeVersion(i + 1), correction))
            { return new QRCodeVersion(i + 1); }
        }
        
        throw new ArgumentException("Argument bitLength exceeds every possible data length for provided level correction");
    }
    
    public record EncodedDataWithHeader(BitStream EncodedHeaderWithPaddedData, QRCodeVersion Version, CorrectionLevel Correction);
}

