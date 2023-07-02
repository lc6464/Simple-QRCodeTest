/// <summary>
/// string 的扩展方法。
/// </summary>
internal static class StringExtensions {
	/// <summary>
	/// 获取判断性字符串（Y/N）指示的值，默认值默认为 No。
	/// </summary>
	/// <param name="s">输入字符串</param>
	/// <param name="defaultIsYesOrNo">默认值，Yes 为 <see cref="true"/>, No 为 <see cref="false"/>, 默认为 No.</param>
	/// <returns>Yes 则返回 <see cref="true"/>, No 则返回 <see cref="false"/>. <paramref name="s"/> 为空则直接返回默认值 <paramref name="defaultIsYesOrNo"/>，否则判断是否是默认值相反的结果，如果是，则返回默认值相反的结果，否则返回默认值。</returns>
	public static bool GetIsYesOrNo(this string? s, bool defaultIsYesOrNo = false) {
		if (string.IsNullOrWhiteSpace(s)) { // 如果为 null、空或空白字符，直接返回默认值
			return defaultIsYesOrNo;
		} else if (defaultIsYesOrNo) { // 默认为 Yes，则判断是否为 No
			if (string.Equals(s, "no", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(s, "n", StringComparison.OrdinalIgnoreCase)
				|| s == "否") {
				return false; // 若为 No，则返回 false
			} else {
				return true; // 若不为 No，则返回默认值 Yes（true)
			}
		} else { // 默认为 No，则判断是否为 Yes
			if (string.Equals(s, "yes", StringComparison.OrdinalIgnoreCase)
				|| string.Equals(s, "y", StringComparison.OrdinalIgnoreCase)
				|| s == "是") {
				return true; // 若为 No，则返回 false
			} else {
				return false; // 若不为 No，则返回默认值 Yes（true)
			}
		}
	}


	/// <summary>
	/// 获取用户判断性输入。
	/// </summary>
	/// <param name="confirmInformation">提示信息</param>
	/// <param name="defaultIsYesOrNo">默认值，Yes 为 <see cref="true"/>, No 为 <see cref="false"/>, 默认为 No，此参数会影响提示信息和默认判断行为。</param>
	/// <returns>Yes 则返回 <see cref="true"/>, No 则返回 <see cref="false"/>. 输入为空则直接返回默认值，否则判断是否是默认值相反的结果，如果是，则返回默认值相反的结果，否则返回默认值。</returns>
	public static bool GetUserConfirm(string? confirmInformation, bool defaultIsYesOrNo = false) {
		Console.Write($"{confirmInformation} ({(defaultIsYesOrNo ? "Y/n" : "y/N")})");
		return Console.ReadLine().GetIsYesOrNo(defaultIsYesOrNo);
	}

	/// <summary>
	/// 获取用户判断性输入，并执行相应回调，将会传入用户输入对应的布尔值。
	/// </summary>
	/// <param name="confirmInformation">提示信息</param>
	/// <param name="callback">回调，将会传入用户输入对应的布尔值</param>
	/// <param name="defaultIsYesOrNo">默认值对应的布尔值</param>
	public static void GetUserConfirm(string? confirmInformation, Action<bool> callback, bool defaultIsYesOrNo = false) =>
		callback(GetUserConfirm(confirmInformation, defaultIsYesOrNo));

	/// <summary>
	/// 获取用户判断性输入，并执行相应回调，将会传入用户输入对应的布尔值。
	/// </summary>
	/// <param name="confirmInformation">提示信息</param>
	/// <param name="callback">回调，将会传入用户输入对应的布尔值</param>
	/// <param name="defaultIsYesOrNo">默认值对应的布尔值</param>
	/// <returns>回调返回结果</returns>
	public static T GetUserConfirm<T>(string? confirmInformation, Func<bool, T> callback, bool defaultIsYesOrNo = false) =>
		callback(GetUserConfirm(confirmInformation, defaultIsYesOrNo));

	/// <summary>
	/// 获取用户判断性输入，并执行相应回调。
	/// </summary>
	/// <param name="confirmInformation">提示信息</param>
	/// <param name="yesCallback">用户传入是时执行的回调</param>
	/// /// <param name="noCallback">用户传入否时执行的回调</param>
	/// <param name="defaultIsYesOrNo">默认值对应的布尔值</param>
	public static void GetUserConfirm(string? confirmInformation, Action? yesCallback, Action? noCallback, bool defaultIsYesOrNo = false) {
		var callback = GetUserConfirm(confirmInformation, defaultIsYesOrNo) ? yesCallback : noCallback;
		callback?.Invoke();
	}

	/// <summary>
	/// 获取用户判断性输入，并执行相应回调。
	/// </summary>
	/// <param name="confirmInformation">提示信息</param>
	/// <param name="yesCallback">用户传入是时执行的回调</param>
	/// /// <param name="noCallback">用户传入否时执行的回调</param>
	/// <param name="defaultIsYesOrNo">默认值对应的布尔值</param>
	/// <returns>回调返回结果，若对应回调不存在则返回泛型 <typeparamref name="T"/> 默认值（可能为 <see cref="null"/>）。</returns>
	public static T? GetUserConfirm<T>(string? confirmInformation, Func<T>? yesCallback, Func<T>? noCallback, bool defaultIsYesOrNo = false) {
		var callback = GetUserConfirm(confirmInformation, defaultIsYesOrNo) ? yesCallback : noCallback;
		return callback != null ? callback() : default;
	}
}