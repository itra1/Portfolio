using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Environment.Microsoft.Windows.Api
{
	public partial class WindowApi
	{
		private static KeyboardLayout[] _layouts = null;

		public class KeyboardLayout
		{
			public UInt32 Id { get; }

			public UInt16 LanguageId { get; }
			public UInt16 KeyboardId { get; }

			public String LanguageName { get; }
			public String KeyboardName { get; }

			internal KeyboardLayout(UInt32 id, UInt16 languageId, UInt16 keyboardId, String languageName, String keyboardName)
			{
				this.Id = id;
				this.LanguageId = languageId;
				this.KeyboardId = keyboardId;
				this.LanguageName = languageName;
				this.KeyboardName = keyboardName;
			}
		}

		public static KeyboardLayout CurrentKeyboardLayout()
		{
			if (_layouts == null)
				_layouts = WindowApi.GetSystemKeyboardLayouts();
			var key = WindowApi.GetKeyboardLayout((uint) WindowApi.GetCurrentThreadId());

			for (int i = 0; i < _layouts.Length; i++)
			{
				if (_layouts[i].Id == key)
					return _layouts[i];
			}
			return _layouts[0];
		}

		public static KeyboardLayout[] GetSystemKeyboardLayouts()
		{
			var keyboardLayouts = new List<KeyboardLayout>();

			var count = GetKeyboardLayoutList(0, null);
			var keyboardLayoutIds = new IntPtr[count];
			_ = GetKeyboardLayoutList(keyboardLayoutIds.Length, keyboardLayoutIds);

			foreach (var keyboardLayoutId in keyboardLayoutIds)
			{
				var keyboardLayout = CreateKeyboardLayout((UInt32) keyboardLayoutId);
				keyboardLayouts.Add(keyboardLayout);
			}

			return keyboardLayouts.ToArray();
		}

		private static KeyboardLayout CreateKeyboardLayout(UInt32 keyboardLayoutId)
		{
			var languageId = (UInt16) (keyboardLayoutId & 0xFFFF);
			var keyboardId = (UInt16) (keyboardLayoutId >> 16);

			return new KeyboardLayout(keyboardLayoutId, languageId, keyboardId, GetCultureInfoName(languageId), GetCultureInfoName(keyboardId));

			String GetCultureInfoName(UInt16 cultureId)
			{
				return CultureInfo.GetCultureInfo(cultureId).DisplayName;
			}
		}

		/// <summary>
		/// Загружает в систему новый идентификатор языка ввода (ранее называемый раскладкой клавиатуры).
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-loadkeyboardlayouta"/>
		/// <param name="pwszKLID"></param>
		/// <param name="flags"></param>
		/// <returns></returns>
		[DllImport("user32.dll",
		CallingConvention = CallingConvention.StdCall,
		CharSet = CharSet.Unicode,
		EntryPoint = "LoadKeyboardLayout",
		SetLastError = true,
		ThrowOnUnmappableChar = false)]
		public static extern uint LoadKeyboardLayout(string pwszKLID, uint flags);

		[DllImport("user32.dll")]
		public static extern uint SetCapture(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern bool ReleaseCapture();

		/// <summary>
		/// Извлекает активный идентификатор языка ввода (ранее называемый раскладкой клавиатуры).
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getkeyboardlayout"/>
		/// <param name="idThread"></param>
		/// <returns></returns>
		[DllImport("user32.dll",
				CallingConvention = CallingConvention.StdCall,
				CharSet = CharSet.Unicode,
				EntryPoint = "GetKeyboardLayout",
				SetLastError = true,
				ThrowOnUnmappableChar = false)]
		public static extern uint GetKeyboardLayout(uint idThread);

		/// <summary>
		/// Устанавливает идентификатор языка ввода (ранее называемый дескриптором раскладки клавиатуры) для вызывающего потока или текущего процесса. Идентификатор языка ввода определяет язык, а также физическую раскладку клавиатуры.
		/// </summary>
		/// <see cref="https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-activatekeyboardlayout"/>
		/// <param name="hkl"></param>
		/// <param name="Flags"></param>
		/// <returns></returns>
		[DllImport("user32.dll",
				CallingConvention = CallingConvention.StdCall,
				CharSet = CharSet.Unicode,
				EntryPoint = "ActivateKeyboardLayout",
				SetLastError = true,
				ThrowOnUnmappableChar = false)]
		public static extern uint ActivateKeyboardLayout(uint hkl, uint Flags);

		[DllImport("user32.dll")]
		private static extern UInt32 GetKeyboardLayoutList(Int32 nBuff, IntPtr[] lpList);
	}
}
