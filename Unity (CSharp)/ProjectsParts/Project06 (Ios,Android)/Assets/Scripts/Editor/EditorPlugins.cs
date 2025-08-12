using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

public class EditorPlugins : EditorWindow {
	private bool isInit = false;


	public int mainToolBar = 0;
	public string[] mainToolBarStrings = new string[] {"Plugins", "Options"};

	private void Start() {
		isInit = false;
	}

	private void OnGUI() {

		if (!isInit || plugins == null || options == null) {
			isInit = true;
			InitPlugins();
			InitOptions();
		}

		EditorGUILayout.BeginVertical();
		mainToolBar = GUILayout.Toolbar(mainToolBar, mainToolBarStrings);
		EditorGUILayout.EndHorizontal();

		switch (mainToolBar) {
			case 0:
				DrawPlugins();
				break;
			case 1:
				DrawOptions();
				break;
		}


	}

	#region Plugins

	struct PluginsParametrs {
		public string value;
		public bool ios;
		public bool android;
		public bool standalone;
		public bool webGl;
	}

	PluginsParametrs[] plugins;

	private readonly char[] defineSeperators = new char[] {';', ',', ' '};

	/// <summary>
	/// Инициализация плагина
	/// </summary>
	void InitPlugins() {
		
		string[] _curDefineSymbolsIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsWebGl = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);

		List<string> _newDefineSymbolsIOS = new List<string>(_curDefineSymbolsIOS);
		List<string> _newDefineSymbolsAndroid = new List<string>(_curDefineSymbolsAndroid);
		List<string> _newDefineSymbolsStandalone = new List<string>(_curDefineSymbolsStandalone);
		List<string> _newDefineSymbolsWebGl = new List<string>(_curDefineSymbolsWebGl);

		plugins = new PluginsParametrs[0];
		//plugins[0].plugin = "PLUGIN_APPTUTTI";
		//plugins[1].plugin = "PLUGIN_FACEBOOK";
		//plugins[2].plugin = "PLUGIN_ADVIDEO";
		//plugins[3].plugin = "PLUGIN_GOOGLEPLAY";
		//plugins[4].plugin = "PLUGIN_AMAZON";

		for (int i = 0; i < plugins.Length; i++) {
			plugins[i].ios = _newDefineSymbolsIOS.Contains(plugins[i].value) ? true : false;
			plugins[i].android = _newDefineSymbolsAndroid.Contains(plugins[i].value) ? true : false;
			plugins[i].standalone = _newDefineSymbolsStandalone.Contains(plugins[i].value) ? true : false;
			plugins[i].webGl = _newDefineSymbolsWebGl.Contains(plugins[i].value) ? true : false;
		}

	}

	void DrawPlugins() {

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Plugin", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.Label("IOS", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.Label("Android", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.Label("Standalone", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.Label("WebGl", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		for (int i = 0; i < plugins.Length; i++) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(plugins[i].value, GUILayout.Width(150));
			GUILayout.FlexibleSpace();
			plugins[i].ios = EditorGUILayout.Toggle(plugins[i].ios);
			GUILayout.FlexibleSpace();
			plugins[i].android = EditorGUILayout.Toggle(plugins[i].android);
			GUILayout.FlexibleSpace();
			plugins[i].standalone = EditorGUILayout.Toggle(plugins[i].standalone);
			GUILayout.FlexibleSpace();
			plugins[i].webGl = EditorGUILayout.Toggle(plugins[i].webGl);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}


		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Save")) {
			SavePlugins();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}

	void SavePlugins() {

		if ((plugins[0].ios && plugins[4].ios) || (plugins[0].android && plugins[4].android)) {
			Debug.LogError("Нельзя использовать сразу несколько платежных систем");
			return;
		}

		string[] _curDefineSymbolsIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsWebGl = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsUnknown = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Unknown).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);

		List<string> _newDefineSymbolsIOS = new List<string>(_curDefineSymbolsIOS);
		List<string> _newDefineSymbolsAndroid = new List<string>(_curDefineSymbolsAndroid);
		List<string> _newDefineSymbolsStandalone = new List<string>(_curDefineSymbolsStandalone);
		List<string> _newDefineSymbolsWebGl = new List<string>(_curDefineSymbolsWebGl);
		List<string> _newDefineSymbolsUnknown = new List<string>(_curDefineSymbolsUnknown);

		for (int i = 0; i < plugins.Length; i++) {
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsIOS, plugins[i].ios, plugins[i].value);
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsAndroid, plugins[i].android, plugins[i].value);
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsUnknown, true, plugins[i].value);
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsStandalone, plugins[i].standalone, plugins[i].value);
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsWebGl, plugins[i].webGl, plugins[i].value);
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, string.Join(";", _newDefineSymbolsIOS.ToArray()));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, string.Join(";", _newDefineSymbolsAndroid.ToArray()));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", _newDefineSymbolsStandalone.ToArray()));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, string.Join(";", _newDefineSymbolsWebGl.ToArray()));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Unknown, string.Join(";", _newDefineSymbolsUnknown.ToArray()));
		ConfirmCondition();

	}

	private void AddOrRemoveFeatureDefineSymbol(List<string> _defineSymbols, bool _usesFeature, string _featureSymbol) {
		if (_usesFeature) {
			if (!_defineSymbols.Contains(_featureSymbol))
				_defineSymbols.Add(_featureSymbol);
		} else {
			if (_defineSymbols.Contains(_featureSymbol))
				_defineSymbols.Remove(_featureSymbol);
		}
	}

	private void ConfirmCondition() {
		for (int i = 0; i < plugins.Length; i++) {
			
		}
	}

	void RenameDictionary(string path, bool isUse) {

		string[] files = Directory.GetFiles(path);
		foreach (string s in files) {

			if (s.Contains(".meta")) {
				File.Delete(s);
				continue;
			}
			
			if (isUse) {
				try {
					File.Move(s, s.Replace("-noUse", ""));
				}
				catch (System.Exception ex) {
					Debug.Log(ex);
				}
			} else {
				try {
					if (!s.Contains("-noUse"))
						File.Move(s, s + "-noUse");
				}
				catch (System.Exception ex) {
					Debug.Log(ex);
				}
			}

		}

		string[] dirs = Directory.GetDirectories(path);
		foreach (string s in dirs) {
			RenameDictionary(s, isUse);
		}

	}

	#endregion

	#region Options

	PluginsParametrs[] options;

	/// <summary>
	/// Инициализация плагина
	/// </summary>
	void InitOptions() {
		string[] _curDefineSymbolsIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsWebGl = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);

		List<string> _newDefineSymbolsIOS = new List<string>(_curDefineSymbolsIOS);
		List<string> _newDefineSymbolsAndroid = new List<string>(_curDefineSymbolsAndroid);
		List<string> _newDefineSymbolsStandalone = new List<string>(_curDefineSymbolsStandalone);
		List<string> _newDefineSymbolsWebGl = new List<string>(_curDefineSymbolsWebGl);

		options = new PluginsParametrs[3];
		options[0].value = "OPTION_DEBUG";
		options[1].value = "OPTION_ARENA";
		options[2].value = "OPTION_SOCIAL";
		//options[0].plugin = "OPTION_NOENERGY";
		//options[1].plugin = "OPTION_NOINAP";
		//options[2].plugin = "OPTION_NOINFO";

		for (int i = 0; i < options.Length; i++) {
			options[i].ios = _newDefineSymbolsIOS.Contains(options[i].value) ? true : false;
			options[i].android = _newDefineSymbolsAndroid.Contains(options[i].value) ? true : false;
			options[i].standalone = _newDefineSymbolsStandalone.Contains(options[i].value) ? true : false;
			options[i].webGl = _newDefineSymbolsWebGl.Contains(options[i].value) ? true : false;
		}

	}

	void DrawOptions() {

		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Plugin", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.Label("IOS", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.Label("Android", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.Label("Standalone", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		GUILayout.Label("WebGl", GUILayout.Width(150));
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();

		for (int i = 0; i < options.Length; i++) {
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(options[i].value, GUILayout.Width(150));
			GUILayout.FlexibleSpace();
			options[i].ios = EditorGUILayout.Toggle(options[i].ios);
			GUILayout.FlexibleSpace();
			options[i].android = EditorGUILayout.Toggle(options[i].android);
			GUILayout.FlexibleSpace();
			options[i].standalone = EditorGUILayout.Toggle(options[i].standalone);
			GUILayout.FlexibleSpace();
			options[i].webGl = EditorGUILayout.Toggle(options[i].webGl);
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}


		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Save")) {
			SaveOptions();
		}
		GUILayout.FlexibleSpace();
		EditorGUILayout.EndHorizontal();
	}

	void SaveOptions() {
		
		string[] _curDefineSymbolsIOS = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsAndroid = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android).Split(defineSeperators , StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsWebGl = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);
		string[] _curDefineSymbolsUnknown = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Unknown).Split(defineSeperators, StringSplitOptions.RemoveEmptyEntries);

		List<string> _newDefineSymbolsIOS = new List<string>(_curDefineSymbolsIOS);
		List<string> _newDefineSymbolsAndroid = new List<string>(_curDefineSymbolsAndroid);
		List<string> _newDefineSymbolsUnknown = new List<string>(_curDefineSymbolsUnknown);
		List<string> _newDefineSymbolsStandalone = new List<string>(_curDefineSymbolsStandalone);
		List<string> _newDefineSymbolsWebGl = new List<string>(_curDefineSymbolsWebGl);

		for (int i = 0; i < options.Length; i++) {
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsIOS, options[i].ios, options[i].value);
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsAndroid, options[i].android, options[i].value);
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsUnknown, true, options[i].value);
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsStandalone, options[i].standalone, options[i].value);
			AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsWebGl, options[i].webGl, options[i].value);
		}

		//PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Unknown, string.Join(";", _newDefineSymbolsUnknown.ToArray()));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, string.Join(";", _newDefineSymbolsIOS.ToArray()));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, string.Join(";", _newDefineSymbolsAndroid.ToArray()));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", _newDefineSymbolsStandalone.ToArray()));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, string.Join(";", _newDefineSymbolsWebGl.ToArray()));
	}

	#endregion

}
