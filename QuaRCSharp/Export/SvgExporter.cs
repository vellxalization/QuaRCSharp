using System.Text;
using QuaRCSharp.Canvas;

namespace QuaRCSharp.Export;

/// <summary>
/// Class for exporting canvas as a .svg image
/// </summary>
public class SvgExporter
{
    /// <summary>
    /// Exports canvas as a .svg image
    /// </summary>
    /// <param name="canvas">Canvas to export</param>
    /// <param name="path">Where to export</param>
    /// <param name="size">Size in pixels</param>
    public void Export(QRCanvas canvas, string path, int size)
    {
        using var fileStream = new FileStream(path, FileMode.OpenOrCreate);
        var builder = new StringBuilder();
        builder.AppendLine("<?xml version='1.0' encoding='utf-8'?>");
        builder.AppendLine($"<svg width='{size}' height='{size}' viewBox='0 0 {canvas.Size} {canvas.Size}' xmlns='http://www.w3.org/2000/svg'>");
        CanvasBitValue? currentValue = null;
        int streak = 0;
        foreach (CanvasBit bit in canvas.GetReadingEnumerator())
        {
            // combining horizontal bits data to a single rectangle to reduce space
            if (currentValue is null)
            {
                currentValue = bit.Value;
                ++streak;
                continue;
            }

            if (bit.Value == currentValue)
            { ++streak; }
            else
            {
                builder.AppendLine($"<rect width='{streak}' height='1.1' x='{bit.Position.X - streak}' y='{bit.Position.Y}' fill='{(currentValue is CanvasBitValue.True ? "black" : "white")}'/>");
                streak = 1;
                currentValue = bit.Value;
            }

            if (bit.Position.X == canvas.Size - 1)
            {
                builder.AppendLine($"<rect width='{streak}' height='1.1' x='{bit.Position.X - streak + 1}' y='{bit.Position.Y}' fill='{(currentValue is CanvasBitValue.True ? "black" : "white")}'/>");
                streak = 0;
                currentValue = null;
            }
        }

        builder.AppendLine("</svg>");

        fileStream.Write(Encoding.UTF8.GetBytes(builder.ToString()));
        fileStream.Flush();
    }
}