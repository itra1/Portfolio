using System;
using System.Collections.Generic;
using System.Linq;
using Game.Common.Attributes.Defines;
using UnityEditor;
using UnityEngine.Events;

namespace Game.Editor.Settings {
	public class DropDownDefineDraw :DefineDrawBase {
		public IDropdownDefine Source { get; set; }
		private string _value;
		private string _startValue = "";
		private int _startIndex = -1;
		private int _index = -1;
		private string[] _valuesArr;
		private string[] _keyArr;

		public override bool IsChange => _startValue != _value;

		public DropDownDefineDraw(string title, IDropdownDefine define) {
			Title = title;
			Source = define;
		}
		public override void Draw() {
			_index = EditorGUILayout.Popup(Title, _index, _keyArr);
		}

		public override void Init(List<string> defines) {
			var values = new Dictionary<string, string>();

			if (Source.MayByNone) {
				values.Add("none", "none");
			}

			foreach (var val in Source.DefineDict) {
				values.Add(val.Key, val.Value);
				if (defines.Contains(val.Value))
					_startValue = val.Value;
			}

			_valuesArr = values.Values.ToArray();
			_keyArr = values.Keys.ToArray();
			_startIndex = Array.IndexOf(_valuesArr, _startValue);
			_index = _startIndex;
		}

		public override void ConfirmChange() {
			if (!IsChange)
				return;
			Source.AfterEnable();
		}

		public override void Save(List<string> defines, UnityAction<List<string>, string, bool> confirm) {
			if (_index >= 0)
				_value = _valuesArr[_index];
			foreach (var val in Source.DefineDict.Values) {
				confirm?.Invoke(defines, val, _value == val);
			}
		}
	}
}
