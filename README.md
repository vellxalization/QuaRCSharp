# QuaRCSharp
QuaRCSharp is a small and simple library that includes a QR code generator and .svg exporter. It is written in pure C# and does not depend on any other libraries.


# Usage
```csharp
var generator = new QRCodeGenerator();
var export = new SvgExporter();
var settings = new GeneratorSettings(CorrectionLevel.H, true, MaskingOptions.Auto);

QRCanvas canvas = gen.Generate("Hello World", settings);
export.Export(canvas, @"path:\qrcode.svg", 300);
```

