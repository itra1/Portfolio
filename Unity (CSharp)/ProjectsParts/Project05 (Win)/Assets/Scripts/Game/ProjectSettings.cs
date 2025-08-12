using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game
{

#if UNITY_EDITOR
  [CustomEditor(typeof(ProjectSettings))]
  public class SettingsEditor : Editor
  {

	 public override void OnInspectorGUI()
	 {
		base.OnInspectorGUI();

		if (GUILayout.Button("Save"))
		{
		  ((ProjectSettings)target).Save();
		}

	 }

  }
#endif

  /// <summary>
  /// Настройки
  /// </summary>
  [CreateAssetMenu(fileName = "Settings", menuName = "Tools/Create settings", order = 1)]
  public class ProjectSettings : ScriptableObject
  {
	 public static ProjectSettings Instance
	 {
		get
		{
		  if (_instance == null)
			 _instance = Resources.Load<ProjectSettings>("Settings/ProjectSettings");

		  return _instance;
		}
	 }
	 public static List<ColorData> Colors => Instance._colors;
	 /// <summary>
	 /// Разрешение на печать логов
	 /// </summary>
	 public static bool PrintDebug  => Instance._printDebug;
	 /// <summary>
	 /// Пусть к превабу плеер
	 /// </summary>
	 public static string PlayerPrefab  => Instance._playerPrefab;
	 /// <summary>
	 /// Библиотека инвентаря
	 /// </summary>
	 public static string InventaryLibrary  => Instance._inventaryLibrary;
	 /// <summary>
	 /// Расположение предметов инвентаря
	 /// </summary>
	 public static string InventaryItems => Instance._inventaryItems;

	 public static string DialogsItems  => Instance._dialogsItems;
	 public static string DialogsLibrary  => Instance._dialogsLibrary;
	 /// <summary>
	 /// Файл сохранения игрока
	 /// </summary>
	 public static string PlayerProgress  => Instance._playerProgress;
	 /// <summary>
	 /// Библиотека GUI
	 /// </summary>
	 public static string GuiLibrary  => Instance._guiLibrary;
	 /// <summary>
	 /// Список уровней
	 /// </summary>
	 public static string[] LevelScenes => Instance._levelScenes;
	 /// <summary>
	 /// Расположение катсцен
	 /// </summary>
	 public static string Catroons => Instance._cartoons;
	 public static string CartoonLibrary => Instance._cartoonLibrary;

	 /// <summary>
	 /// Расположение объектов интерфейса
	 /// </summary>
	 public static string GuiFolder  => Instance._guiFolder;
	 /// <summary>
	 /// Библиотека символов
	 /// </summary>
	 public static string SymbolsLibrary => Instance._symbolsLibrary;
	 /// <summary>
	 /// Расположение символов
	 /// </summary>
	 public static string SymbolsItams  => Instance._symbolsItams;

	 /// <summary>
	 /// Максимальное количество сохранений
	 /// </summary>
	 public static int MaxSaveToUser => Instance._maxSaveToUser;

	 public static string InteractionMotionsFolder => Instance._interactionMotionsFolder;

	 public static string InteractionMotionsLibrary => Instance._interactionMotionsLibrary;
	 /// <summary>
	 /// Длои для перемещения по земле
	 /// </summary>
	 public static LayerMask GroundLayerMaks => Instance._groundLayerMaks;
	 public static LayerMask GroundAndClimbLayerMaks => Instance._groundAndClimbLayerMaks;
	 public static LayerMask PlayerLayerMask => Instance._playerLayerMask;

	 [SerializeField]
	 private Material[] _playerTailsMaterials;
	 public static Material[] PlayerTailsMaterials => Instance._playerTailsMaterials;

	 private static ProjectSettings _instance;

	 [Tooltip("Настройки цветов")]
	 [SerializeField]
	 private List<ColorData> _colors;


	 [Tooltip("Печать логи")]
	 [SerializeField]
	 private bool _printDebug = true;

	 [Tooltip("Пусть к игроку")]
	 [SerializeField]
	 private string _playerPrefab = "Prefabs/Player/Player";

	 [Tooltip("Библиотека инвентаря")]
	 [SerializeField]
	 private string _inventaryLibrary = "Prefabs/Items/InventaryLibrary";

	 [Tooltip("Расположение предметов инвентаря")]
	 [SerializeField]
	 private string _inventaryItems = "Prefabs/Items/Inventary";

	 [Tooltip("Библиотека диалогов")]
	 [SerializeField]
	 private string _dialogsLibrary = "Prefabs/Dialogs/DialogueLibrary";
	 [Tooltip("Расположение диалогов")]
	 [SerializeField]
	 private string _dialogsItems = "Prefabs/Dialogs";

	 [Tooltip("Расположение катсцен")]
	 [SerializeField]
	 private string _cartoons = "Prefabs/Cartoons";
	 private string _cartoonLibrary = "Prefabs/Cartoons/CartoonLibrary";

	 [Tooltip("Сохранение игрока")]
	 [SerializeField]
	 private string _playerProgress = "Prefabs/Player/PlayerProgress";

	 [Tooltip("Библиотека инвентаря")]
	 [SerializeField]
	 private string _symbolsLibrary = "Prefabs/Items/SymbolsLibrary";
	 [Tooltip("Символы")]
	 [SerializeField]
	 private string _symbolsItams = "Prefabs/Items/Symbols";

	 [Tooltip("Библиотека панелей UI")]
	 [SerializeField]
	 private string _guiLibrary = "Prefabs/UI/UILibrary";

	 [Tooltip("Расположение UI")]
	 [SerializeField]
	 private string _guiFolder = "Prefabs/UI/Panels";

	 [Tooltip("Максимальное количество сохранений")]
	 [SerializeField]
	 private int _maxSaveToUser = 20;

	 [Tooltip("Библиотека интеракци")]
	 [SerializeField]
	 private string _interactionMotionsLibrary = "Prefabs/Player/InteractionLibrary";

	 [Tooltip("Доступные интеракций")]
	 [SerializeField]
	 private string _interactionMotionsFolder = "Prefabs/Player/InteractionMotions";

	 [SerializeField]
	 private string[] _levelScenes = new string[] {
		  "Level_1"
		, "Level_2"
		, "Level_3"
		, "Level_4"
		, "Level_5"
		, "Level_6"
		, "Level_7"};

	 [Tooltip("Слои для передвижения")]
	 [SerializeField]
	 private LayerMask _groundLayerMaks;
	 [Tooltip("Слои для передвижения")]
	 [SerializeField]
	 private LayerMask _groundAndClimbLayerMaks;
	 [Tooltip("Слой игрока")]
	 [SerializeField]
	 private LayerMask _playerLayerMask;

	 [System.Serializable]
	 public class ColorData
	 {
		public string title;
		[ColorUsage(true,true)]
		public Color color;
	 }

#if UNITY_EDITOR

	 public void Save()
	 {
		EditorUtility.SetDirty(this);
	 }
#endif

  }
}