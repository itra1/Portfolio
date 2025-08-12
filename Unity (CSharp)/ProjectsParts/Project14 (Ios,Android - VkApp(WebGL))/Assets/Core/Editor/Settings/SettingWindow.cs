using Core.Engine.App.Base.Attributes.Defines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Build;
using UnityEngine;

namespace Core.Editor.Settings {
	public class SettingWindow : EditorWindow {
		private bool _isInit = false;
		private DefineDrawBase[] _options;
		private readonly DefineDrawBase[] _optionsServer;
		private readonly string[] _servers;
		private NamedBuildTarget _buildTarget;

		public List<DefineDrawBase> Symbols = new();
		private void OnEnable() {
			_isInit = false;
		}

		private void OnDisable() {
			SaveOptions();
		}

		[MenuItem("App/Settings")]
		public static void OpenSetting() {
			var w = ScriptableObject.CreateInstance<SettingWindow>();
			w.Show();
		}

		public DefineDrawBase GetSingleProperty(IToggleDefine symbol) {
			return new SingleDefineDraw($"{symbol.Description} ({symbol.Symbol})", symbol);
		}
		public DefineDrawBase GetDropdownProperty(IDropdownDefine symbol) {
			return new DropDownDefineDraw($"{symbol.Description}", symbol);
		}
		public void FindSymbolsOptions(List<string> currentSymbols) {
			Symbols.Clear();

			var playerAssemblies = UnityEditor.Compilation.CompilationPipeline.GetAssemblies();

			foreach (var assamble in playerAssemblies) {
				VisibleSettings(currentSymbols, assamble);
			}
		}

		private void VisibleSettings(List<string> currentSymbols, UnityEditor.Compilation.Assembly assamble) {

			Debug.Log(assamble.name);
			var types = from t in Assembly.Load(assamble.name).GetTypes()
									where t.GetInterfaces().Contains(typeof(IDefine))
									select t;

			foreach (var elemClass in types) {
				if (elemClass.GetInterface(nameof(IToggleDefine)) != null) {
					var s = GetSingleProperty(((IToggleDefine)Activator.CreateInstance(elemClass)));
					s.Init(currentSymbols);
					Symbols.Add(s);
				}
				if (elemClass.GetInterface(nameof(IDropdownDefine)) != null) {
					var s = GetDropdownProperty(((IDropdownDefine)Activator.CreateInstance(elemClass)));
					s.Init(currentSymbols);
					Symbols.Add(s);
				}
			}
		}

		private void OnGUI() {
			titleContent = new GUIContent("Настройки");

			if (!_isInit || _options == null) {
				_isInit = true;
				Init();
			}

			DrawOptions();
		}
		private readonly char[] defineSeperators = new char[] { ';', ',', ' ' };

		private void Init() {
			_options = new DefineDrawBase[0];

#if UNITY_STANDALONE
			_buildTarget = NamedBuildTarget.Standalone;
#elif UNITY_IOS
			_buildTarget = NamedBuildTarget.iOS;
#elif UNITY_ANDROID
			_buildTarget = NamedBuildTarget.Android;
#elif UNITY_STANDALONE_OSX
			_buildTarget = NamedBuildTarget.Standalone;
#elif UNITY_WEBGL
			_buildTarget = NamedBuildTarget.WebGL;
#endif

			var _newDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbols(_buildTarget).Split(defineSeperators).ToList();

			FindSymbolsOptions(_newDefineSymbolsStandalone);

			_options = new DefineDrawBase[] { };
			for (int i = 0; i < Symbols.Count; i++)
				_options = _options.Append(Symbols[i]).ToArray();

			for (int i = 0; i < _options.Length; i++) {
				_options[i].Init(_newDefineSymbolsStandalone);
			}
		}

		private void DrawOptions() {
			_ = EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Option", GUILayout.Width(300));
			GUILayout.FlexibleSpace();
			GUILayout.Label("Value", GUILayout.Width(150));
			EditorGUILayout.EndHorizontal();

			for (int i = 0; i < _options.Length; i++)
				_options[i].Draw();

			_ = EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}

		private void SaveOptions() {
			string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbols(_buildTarget).Split(defineSeperators);

			List<string> _newDefineSymbolsStandalone = new(_curDefineSymbolsStandalone);

			for (int i = 0; i < _options.Length; i++) {
				_options[i].Save(_newDefineSymbolsStandalone, AddOrRemoveFeatureDefineSymbol);
				_options[i].ConfirmChange();
			}
			PlayerSettings.SetScriptingDefineSymbols(_buildTarget, string.Join(";", _newDefineSymbolsStandalone.ToArray()));
		}

		private void AddOrRemoveFeatureDefineSymbol(List<string> _defineSymbols, string _featureSymbol, bool _usesFeature) {
			if (_usesFeature) {
				if (!_defineSymbols.Contains(_featureSymbol))
					_defineSymbols.Add(_featureSymbol);
			} else {
				if (_defineSymbols.Contains(_featureSymbol))
					_ = _defineSymbols.Remove(_featureSymbol);
			}
		}
	}
}