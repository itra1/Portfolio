using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Editor.Settings.Base;
using Editor.Settings.Components;
using UnityEditor;
using UnityEngine;

namespace Editor.Settings
{
	public class SettingsWindow : EditorWindow
	{
		[MenuItem("CnpOS/Settings")]
		public static void OpenSetting() => CreateInstance<SettingsWindow>().Show();
		
		private readonly char[] _defineSeparators = { ';', ',', ' ' };
		private readonly List<DefineDrawBase> _symbols = new ();
		
		private bool _isInitialized;
		private DefineDrawBase[] _options;
		private DefineDrawBase[] _optionsServer;
		private string[] _servers;
		private BuildTargetGroup _buildTargetGroup;
		
		private void OnEnable() => _isInitialized = false;
		
		private void OnDisable() => SaveOptions();
		
		private DefineDrawBase GetSingleProperty(IToggleDefine symbol) => 
			new SingleDefineDraw($"{symbol.Description}", symbol);
		
		private DefineDrawBase GetDropdownProperty(IDropdownDefine symbol) => 
			new DropDownDefineDraw($"{symbol.Description}", symbol);
		
		private void FindSymbolsOptions(List<string> currentSymbols)
		{
			_symbols.Clear();
			
			var types = from type in Assembly.GetExecutingAssembly().GetTypes()
				where type.GetInterfaces().Contains(typeof(IDefine))
				select type;
			
			foreach (var type in types)
			{
				if (type.GetInterface(nameof(IToggleDefine)) != null)
				{
					var symbol = GetSingleProperty((IToggleDefine) Activator.CreateInstance(type));
					symbol.Init(currentSymbols);
					_symbols.Add(symbol);
				}
				
				if (type.GetInterface(nameof(IDropdownDefine)) != null)
				{
					var symbol = GetDropdownProperty((IDropdownDefine) Activator.CreateInstance(type));
					symbol.Init(currentSymbols);
					_symbols.Add(symbol);
				}
			}
		}

		private void OnGUI()
		{
			titleContent = new GUIContent("Settings");
			
			if (!_isInitialized || _options == null)
			{
				_isInitialized = true;
				Init();
			}
			
			DrawOptions();
		}

		private void Init()
		{
#if UNITY_STANDALONE
			_buildTargetGroup = BuildTargetGroup.Standalone;
#elif UNITY_IOS
			_buildTargetGroup = BuildTargetGroup.iOS;
#elif UNITY_ANDROID
			_buildTargetGroup = BuildTargetGroup.Android;
#elif UNITY_STANDALONE_OSX
			_buildTargetGroup = BuildTargetGroup.Android;
#endif
			
			var newDefineSymbolsStandalone = PlayerSettings
				.GetScriptingDefineSymbolsForGroup(_buildTargetGroup)
				.Split(_defineSeparators)
				.ToList();
			
			FindSymbolsOptions(newDefineSymbolsStandalone);
			
			_options = Array.Empty<DefineDrawBase>();
			
			for (var i = 0; i < _symbols.Count; i++)
				_options = _options.Append(_symbols[i]).ToArray();
			
			for (var i = 0; i < _options.Length; i++)
				_options[i].Init(newDefineSymbolsStandalone);
		}

		private void DrawOptions()
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Label("Option", EditorStyles.boldLabel, GUILayout.Width(300));
			GUILayout.FlexibleSpace();
			GUILayout.Label("Value", EditorStyles.boldLabel, GUILayout.Width(150));
			EditorGUILayout.EndHorizontal();
			
			for (var i = 0; i < _options.Length; i++)
				_options[i].Draw();
			
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			EditorGUILayout.EndHorizontal();
		}
		private void SaveOptions()
		{
			var currentDefineSymbolsStandalone = PlayerSettings
				.GetScriptingDefineSymbolsForGroup(_buildTargetGroup)
				.Split(_defineSeparators);
			
			var newDefineSymbolsStandalone = new List<string>(currentDefineSymbolsStandalone);
			
			for (var i = 0; i < _options.Length; i++)
			{
				var option = _options[i];
				
				option.Save(newDefineSymbolsStandalone, AddOrRemoveFeatureDefineSymbol);
				option.ConfirmChange();
			}
			
			PlayerSettings.SetScriptingDefineSymbolsForGroup(_buildTargetGroup, string.Join(';', newDefineSymbolsStandalone.ToArray()));
		}
		
		private void AddOrRemoveFeatureDefineSymbol(List<string> defineSymbols, string featureSymbol, bool usesFeature)
		{
			if (usesFeature)
			{
				if (!defineSymbols.Contains(featureSymbol))
					defineSymbols.Add(featureSymbol);
			}
			else
			{
				if (defineSymbols.Contains(featureSymbol))
					defineSymbols.Remove(featureSymbol);
			}
		}
	}
}