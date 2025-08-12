using System.Collections.Generic;
using Editor.Settings.Base;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Editor.Settings.Components
{
	public class SingleDefineDraw : DefineDrawBase
	{
		private bool _startValue;
		
		public bool Value  { get; set; }
		public IToggleDefine Source { get; set; }

		public override bool IsChange => Value != _startValue;

		public SingleDefineDraw(string title, IToggleDefine define)
		{
			Title = title;
			Source = define;
		}

		public override void Init(List<string> defines)
		{
			_startValue = defines.Contains(Source.Symbol);
			Value = _startValue;
		}
		
		public override void Draw()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(Title, GUILayout.Width(300));
			GUILayout.FlexibleSpace();
			Value = EditorGUILayout.Toggle(Value);
			EditorGUILayout.EndHorizontal();
		}

		public override void ConfirmChange()
		{
			if (!IsChange) 
				return;
			
			if (Value)
				Source.AfterEnable();
			else
				Source.AfterDisable();
		}

		public override void Save(List<string> allDefines, UnityAction<List<string>, string, bool> confirm)
		{
			confirm?.Invoke(allDefines, Source.Symbol, Value);
		}
	}
}
