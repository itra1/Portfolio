using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace it.Editor.Settings
{
	public class Settings : EditorWindow
	{

		private bool _isInit = false;

		private OptionsParametrs[] _options = new OptionsParametrs[] {
			//	new OptionsParametrs() { title = "Отключить сокеты (editor)", key = "DIS_SOCKETS" }
			//, new OptionsParametrs() { title = "Отключить отправку состояния (editor)", key = "DIS_SEND_STATE" }
			//, new OptionsParametrs() { title = "Отключить входящих экшенов (editor)", key = "DIS_FRONT_ACT" }
			//, new OptionsParametrs() { title = "Включить прокси в редакторе (editor)", key = "ENABLE_PROXY_EDITOR" }
			//, new OptionsParametrs() { title = "Использовать бланковый окна (editor)", key = "ENABLE_BLANK_EDITOR" }
			//, new OptionsParametrs() { title = "Чистить кеш браузера", key = "WEB_CLEAR" }
			//, new OptionsParametrs() { title = "VLC player", key = "VLC_PLAYER" }
			//, new OptionsParametrs() { title = "UMP player", key = "UMP_PLAYER" }
			//, new OptionsParametrs() { title = "Отключить рабочие столы", key = "DISABLE_DESKTOPS" }
			};

		private OptionsParametrs[] _optionsServer = new OptionsParametrs[]{
				new OptionsParametrs() { title = "Тестовый сервер", key = "SERVER_DEV" }
			, new OptionsParametrs() { title = "Релизный сервер", key = "SERVER_RELEASE" }
			};
		private string[] _servers;
		private int _selectServer = 0;

		private void OnEnable()
		{
			_isInit = false;
		}

		private void OnDestroy()
		{
			SaveOptions();
		}
		struct OptionsParametrs
		{
			public string title;
			public string key;
			public bool value;
			public string Name => string.Format("{0} ({1})", title, key);
		}

		[MenuItem("Garilla Poker/Settings")]
		public static void OpenSetting()
		{
			EditorWindow w = new Settings();
			w.Show();
		}

		private void OnGUI()
		{

			titleContent = new GUIContent("Настройки");

			if (!_isInit || _options == null)
			{
				_isInit = true;
				Init();
			}

			DrawOptions();
		}
		private readonly char[] defineSeperators = new char[] { ';', ',', ' ' };

		private void Init()
		{
#if UNITY_WEBGL
			string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL).Split(defineSeperators);
#else
			string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(defineSeperators);
#endif
			List<string> _newDefineSymbolsStandalone = new List<string>(_curDefineSymbolsStandalone);

			for (int i = 0; i < _options.Length; i++)
			{
				_options[i].value = _newDefineSymbolsStandalone.Contains(_options[i].key);
			}

			_servers = new string[_optionsServer.Length];
			for (int i = 0; i < _optionsServer.Length; i++)
				_servers[i] = _optionsServer[i].Name;

			for (int i = 0; i < _optionsServer.Length; i++)
			{
				if (_newDefineSymbolsStandalone.Contains(_optionsServer[i].key))
					_selectServer = i;
			}

		}

		void DrawOptions()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Option", GUILayout.Width(300));
			GUILayout.FlexibleSpace();
			GUILayout.Label("Value", GUILayout.Width(150));
			EditorGUILayout.EndHorizontal();

			_selectServer = EditorGUILayout.Popup("Server", _selectServer, _servers);

			for (int i = 0; i < _options.Length; i++)
			{
				EditorGUILayout.BeginHorizontal();
				GUILayout.Label(_options[i].title, GUILayout.Width(300));
				GUILayout.FlexibleSpace();
				_options[i].value = EditorGUILayout.Toggle(_options[i].value);
				EditorGUILayout.EndHorizontal();
			}

			EditorGUILayout.BeginHorizontal();
			//if (GUILayout.Button("Save"))
			//{
			//	SaveOptions();
			//}
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		void SaveOptions()
		{
#if UNITY_WEBGL
			string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL).Split(defineSeperators);
#else
			string[] _curDefineSymbolsStandalone = PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone).Split(defineSeperators);
#endif


			List<string> _newDefineSymbolsStandalone = new List<string>(_curDefineSymbolsStandalone);


			for (int i = 0; i < _optionsServer.Length; i++)
			{
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsStandalone, _optionsServer[i].key, _selectServer == i);
			}

			for (int i = 0; i < _options.Length; i++)
			{
				AddOrRemoveFeatureDefineSymbol(_newDefineSymbolsStandalone, _options[i].key, _options[i].value);
			}

#if UNITY_WEBGL
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.WebGL, string.Join(";", _newDefineSymbolsStandalone.ToArray()));
#else
			PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, string.Join(";", _newDefineSymbolsStandalone.ToArray()));
#endif
		}

		private void AddOrRemoveFeatureDefineSymbol(List<string> _defineSymbols, string _featureSymbol, bool _usesFeature)
		{
			if (_usesFeature)
			{
				if (!_defineSymbols.Contains(_featureSymbol))
					_defineSymbols.Add(_featureSymbol);
			}
			else
			{
				if (_defineSymbols.Contains(_featureSymbol))
					_defineSymbols.Remove(_featureSymbol);
			}
		}

	}
}