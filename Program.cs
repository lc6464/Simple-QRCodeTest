ActionType type;
string[]? strings = null;
if (args.Length > 1) { // 直接读取 CLI 参数
	type = ParseActionType(args[0]);
	if (type == ActionType.Unknow) {
		Console.Error.WriteLine("您输入的操作类型有误！");
		return;
	}
	strings = args[1..];
} else if (args.Length == 1) { // 只传入一个参数，当作数据处理，自动识别操作
	type = ParseActionType(args[0]);
	if (type == ActionType.Unknow) {
		if (File.Exists(args[0])) { // 文件存在，当作解码
			Console.WriteLine("您未传入操作类型，但传入了一个参数，且检测到此文件存在，已进入解码逻辑。");
			type = ActionType.Decode;
			strings = new[] { args[0] };
		} else { // 文件不存在，当作编码
			Console.WriteLine("您未传入操作类型，但传入了一个参数，且检测到此文件不存在，已进入编码逻辑。");
			type = ActionType.Encode;
			strings = new[] { args[0] };
		}
	}
} else { // 什么也没有传入
	Console.Write("请输入操作类型：");
	type = ParseActionType(Console.ReadLine());
	if (type == ActionType.Unknow) {
		Console.Error.WriteLine("您输入的操作类型有误！");
		return;
	}
}

List<string> readList = new();
if (strings is null || strings.Length == 0) {
	Console.WriteLine($"请输入要{(type == ActionType.Encode ? "编码的文本" : "解码的图片路径")}，一行一个：");
	while (true) {
		var read = Console.ReadLine()?.Trim();
		if (string.IsNullOrWhiteSpace(read)) {
			break;
		}
		readList.Add(read);
	}
	strings = readList.ToArray();
	readList.Clear();
}

if (strings.Length == 0) {
	Console.WriteLine($"没有要{(type == ActionType.Encode ? "编码的文本" : "解码的图片路径")}！");
	return;
}


DirectoryInfo directoryInfo = new("Results"); // 创建文件夹
if (!directoryInfo.Exists) {
	if (File.Exists(directoryInfo.Name)) {
		Console.Error.WriteLine($"存在 {directoryInfo.Name} 文件，无法创建文件夹！");
		return;
	}
	try {
		directoryInfo.Create();
	} catch (Exception e) {
		Console.Error.WriteLine($"创建文件夹 {directoryInfo.Name} 时发生异常！");
		Console.Error.Write($"异常：{e.Message}\r\n详细信息：");
		Console.Error.WriteLine(e);
		return;
	}
}

Dictionary<EncodeHintType, object> hints = new() { // 纠错等级：低
	{ EncodeHintType.ERROR_CORRECTION, ZXing.QrCode.Internal.ErrorCorrectionLevel.L },
	{ EncodeHintType.CHARACTER_SET, "UTF-8" }, // UTF-8
	{ EncodeHintType.MARGIN, 0 } // 不留白
};


if (type == ActionType.Encode) { // 编码模式
	foreach (var str in strings) {
		Console.Write($"{(str.Length < 32 ? str : (str[..32] + "..."))} ... ");

		try { // 编码数据
			QRCodeWriter writer = new();
			var data = writer.encode(str, BarcodeFormat.QR_CODE, 1000, 1000, hints);

			try { // 生成二维码
				BitmapRenderer renderer = new();
				using var bitmap = renderer.Render(data, BarcodeFormat.QR_CODE, str);

				var guid = Guid.NewGuid();
				var fileName = $@"Results\{DateTime.Now:HH-mm-ss}_{guid}.png";

				try {
					bitmap.Save(fileName);
					Console.WriteLine(fileName);
				} catch (Exception e) {
					Console.Error.WriteLine($"写入文件 {fileName} 时发生异常！");
					Console.Error.Write($"异常：{e.Message}\r\n详细信息：");
					Console.Error.WriteLine(e);
				}
			} catch (Exception e) {
				Console.Error.WriteLine($"生成二维码时发生异常！");
				Console.Error.Write($"异常：{e.Message}\r\n详细信息：");
				Console.Error.WriteLine(e);
			}
		} catch (Exception e) {
			Console.Error.WriteLine($"编码数据时发生异常！");
			Console.Error.Write($"异常：{e.Message}\r\n详细信息：");
			Console.Error.WriteLine(e);
		}
	}
} else { // 解码模式
	foreach (var str in strings) {
		Console.Write($"{str} ... ");

		try { // 读取文件
			if (!File.Exists(str)) {
				Console.Error.WriteLine($"文件不存在！");
				continue;
			}

			using Bitmap bitmap = new(str);

			try {
				QRCodeReader reader = new();
				var result = reader.decode(new BinaryBitmap(new HybridBinarizer(new BitmapLuminanceSource(bitmap))));

				if (result is null) {
					Console.Error.WriteLine("二维码识别失败。");
				} else {
					var guid = Guid.NewGuid();
					var fileName = $@"Results\{DateTime.Now:HH-mm-ss}_{guid}.txt";

					try {
						File.WriteAllText(fileName, $"{str}\r\n{result.Text}");
						Console.WriteLine(fileName);
					} catch (Exception e) {
						Console.Error.WriteLine($"写入文件 {fileName} 时发生异常！");
						Console.Error.Write($"异常：{e.Message}\r\n详细信息：");
						Console.Error.WriteLine(e);
					}
				}
			} catch (Exception e) {
				Console.Error.WriteLine($"识别二维码时发生异常！");
				Console.Error.Write($"异常：{e.Message}\r\n详细信息：");
				Console.Error.WriteLine(e);
			}
		} catch (Exception e) {
			Console.Error.WriteLine($"读取文件时发生异常！");
			Console.Error.Write($"异常：{e.Message}\r\n详细信息：");
			Console.Error.WriteLine(e);
		}
	}
}



static ActionType ParseActionType(string? s, bool toLower = true) => (toLower ? s?.ToLower() : s) switch {
	"d" or "decode" or "Decode" => ActionType.Decode,
	"e" or "encode" or "Encode" => ActionType.Encode,
	_ => ActionType.Unknow
};

internal enum ActionType { Encode, Decode, Unknow }