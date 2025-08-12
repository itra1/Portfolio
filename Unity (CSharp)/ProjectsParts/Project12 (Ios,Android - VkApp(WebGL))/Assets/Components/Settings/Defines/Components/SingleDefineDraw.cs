using Settings.SymbolOptions.Base;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Settings.Defines.Components {
	public class SingleDefineDraw : DefineDrawBase {
		public string Key;
		public bool Value;
		private bool _startValue = false;
		public IToggleDefine Source { get; set; }

		public override bool IsChange => Value != _startValue;

		public SingleDefineDraw(string title, IToggleDefine define) {
			Title = title;
			Source = define;
		}

		public override void Init(List<string> defines) {
			_startValue = defines.Contains((Source as IToggleDefine).Symbol);
			Value = _startValue;
		}
		public override void Draw() {
			_ = EditorGUILayout.BeginHorizontal();
			GUILayout.Label(Title, GUILayout.Width(300));
			GUILayout.FlexibleSpace();
			Value = EditorGUILayout.Toggle(Value);
			EditorGUILayout.EndHorizontal();
		}

		public override void ConfirmChange() {
			if (!IsChange)
				return;
			if (Value)
				Source.AfterEnable();
			else
				Source.AfterDisable();
		}

		public override void Save(List<string> allDefines, UnityAction<List<string>, string, bool> confirm) {
			confirm?.Invoke(allDefines, Source.Symbol, Value);
		}
	}
}
