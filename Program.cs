// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


/*
Bitmap bitmap = new(@"Rendered.png");

BitmapLuminanceSource luminanceSource = new(bitmap);
HybridBinarizer binarizer = new(luminanceSource);
BinaryBitmap binaryBitmap = new(binarizer);

QRCodeReader reader = new();
Result result = reader.decode(binaryBitmap);

if (result == null) {
	Console.WriteLine("识别失败。");
} else {
	Console.WriteLine("识别成功！");
	File.WriteAllText(@"data.txt", result.Text);
}
*/



Dictionary<EncodeHintType, object> hints = new () {
	{ EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.L }
};

QRCodeWriter writer = new ();
BitMatrix data = writer.encode("", BarcodeFormat.QR_CODE, 400, 400, hints);


BitmapRenderer renderer = new ();
Bitmap bitmap = renderer.Render(data, BarcodeFormat.QR_CODE, "");
bitmap.Save(@"Rendered.png");



bitmap.Dispose();