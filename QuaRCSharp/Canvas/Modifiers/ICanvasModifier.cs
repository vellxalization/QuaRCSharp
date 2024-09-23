namespace QuaRCSharp.Canvas.Modifiers;

/// <summary>
/// An interface for adding service information to canvas
/// </summary>
public interface ICanvasModifier
{
    /// <summary>
    /// Adds a modifier to canvas
    /// </summary>
    /// <param name="canvas">Canvas to apply modifier to</param>
    public void ModifyCanvas(ref QRCanvas canvas);
}
