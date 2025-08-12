using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using it.Game.Items.Inventary;
using it.Game.Items.Symbols;
using it.Game.Scenes;
using com.ootii.Input;
using Tayx.Graphy;
using it.Game.Handlers;
using it.Dialogue;
using System.Runtime.CompilerServices;

using QFSW.QC;

namespace it.Game.Managers
{
  //[CommandPrefix("game.")]
  public class GameManager : Singleton<GameManager>
  {
	 public const string EVT_GAME_PHASE_CHAGE = "GAME_PAHSE_CHAGE";
	 public const string EVT_GAMEQUITING = "GAMEQUITING";

	 [SerializeField]
	 private GameObject _consolePrefab;

	 public static bool IsQuiting = false;

	 public static bool IsControl { get; private set; }
	 public static bool IsDevelop { get; private set; }
	 public static bool IsLog { get; private set; }

	 /// <summary>
	 /// Управление игровым процессом
	 /// </summary>
	 [SerializeField] private EasyInputSource _gameInputSource;
	 /// <summary>
	 /// Система ввода
	 /// </summary>
	 public EasyInputSource GameInputSource { get => _gameInputSource; }
	 [SerializeField] private EasyInputSource _baseInputSource;
	 /// <summary>
	 /// Общее управление
	 /// </summary>
	 public EasyInputSource BaseInputSource => _baseInputSource;
	 [SerializeField] private EasyInputSource _environmentInputSource;
	 /// <summary>
	 /// Управление при контроле
	 /// </summary>
	 public EasyInputSource EnvironmentInputSource => _environmentInputSource;

	 private GameKeys _gameKeys = new GameKeys();
	 public GameKeys GameKeys { get => _gameKeys; set => _gameKeys = value; }

	 /// <summary>
	 /// Менеджер локации
	 /// </summary>
	 private LocationManager _locationManager;
	 public LocationManager LocationManager
	 {
		get
		{
		  if (_locationManager == null)
		  {
			 _locationManager = FindObjectOfType<LocationManager>();
		  }
		  return _locationManager;
		}
	 }
	 private SceneBehaviour _sceneBahviour;
	 public SceneBehaviour SceneBehaviour
	 {
		get
		{
		  if (_sceneBahviour == null)
		  {
			 _sceneBahviour = FindObjectOfType<SceneBehaviour>();
		  }
		  return _sceneBahviour;
		}
	 }
	 /// <summary>
	 /// Менеджер загрузки сцен
	 /// </summary>
	 public SceneManager SceneManager
	 {
		get
		{
		  if (_sceneManager == null)
		  {
			 _sceneManager = FindObjectOfType<SceneManager>();
		  }
		  return _sceneManager;
		}
	 }
	 /// <summary>
	 /// Инвентарь
	 /// </summary>
	 public InventaryManager Inventary
	 {
		get
		{
		  if (_inventary == null)
			 _inventary = GetComponentInChildren<InventaryManager>();
		  return _inventary;
		}
	 }
	 public SymbolsManager SymbolsManager
	 {
		get
		{
		  if (_symbolsManager == null)
			 _symbolsManager = GetComponentInChildren<SymbolsManager>();
		  return _symbolsManager;
		}
	 }
	 /// <summary>
	 /// Менеджер пользователя
	 /// </summary>
	 public UserManager UserManager
	 {
		get
		{
		  if (_userManager == null)
			 _userManager = GetComponentInChildren<UserManager>();
		  return _userManager;
		}
	 }
	 private EnergyManager _energyManager;
	 public EnergyManager EnergyManager
	 {
		get
		{
		  if (_energyManager == null)
			 _energyManager = GetComponentInChildren<EnergyManager>();
		  return _energyManager;
		}
	 }
	 public GamePhase Phase
	 {
		get => _phase; set
		{
		  if (_phase == value)
			 return;

		  _phase = value;
		  Game.Events.EventDispatcher.SendMessage(EVT_GAME_PHASE_CHAGE);
		}
	 }
	 /// <summary>
	 /// Обработчик событий клавиатуры
	 /// </summary>
	 public KeyListener KeyListener
	 {
		get
		{
		  if (_keyListener == null)
			 _keyListener = GetComponentInChildren<KeyListener>();
		  return _keyListener;
		}
		set => _keyListener = value;
	 }
	 public GameSettings GameSettings
	 {
		get
		{
		  if (_gameSettings == null)
			 _gameSettings = GetComponentInChildren<GameSettings>();
		  return _gameSettings;
		}
		set => _gameSettings = value;
	 }
	 public GraphyManager FpsView
	 {
		get
		{
		  if (_fpsView == null)
			 _fpsView = GetComponentInChildren<GraphyManager>(true);
		  return _fpsView;
		}
		set => _fpsView = value;
	 }
	 public PlayerUICoordinate PlayerCoordinateUI
	 {
		get
		{
		  if (_playerCoordinateUI == null)
			 _playerCoordinateUI = GetComponentInChildren<PlayerUICoordinate>(true);
		  return _playerCoordinateUI;
		}
		set => _playerCoordinateUI = value;
	 }
	 public InteractionsManager InteractionsManager
	 {
		get
		{
		  if (_interactionsManager == null)
			 _interactionsManager = GetComponentInChildren<InteractionsManager>(true);
		  return _interactionsManager;
		}
		set => _interactionsManager = value;
	 }
	 public AudioListener AudioListener
	 {
		get
		{
		  if (_audioListener == null)
			 _audioListener = GetComponentInChildren<AudioListener>(true);
		  return _audioListener;
		}
		set => _audioListener = value;
	 }
	 public DialogsManager DialogsManager
	 {
		get
		{
		  if (_dialogsManager == null)
			 _dialogsManager = GetComponentInChildren<DialogsManager>(true);
		  return _dialogsManager;
		}
		set => _dialogsManager = value;
	 }
	 private int _targetNewLevel;
	 public int TargetNewLevel { get => _targetNewLevel; set => _targetNewLevel = value; }

	 private SceneManager _sceneManager;
	 private UserManager _userManager;
	 private InventaryManager _inventary;
	 private SymbolsManager _symbolsManager;
	 private DialogsManager _dialogsManager;
	 private bool _isLoadindSceneProcess = false;
	 private GamePhase _phase;
	 private KeyListener _keyListener;
	 private GameSettings _gameSettings;
	 private Game.Handlers.PlayerUICoordinate _playerCoordinateUI;
	 private Game.Managers.InteractionsManager _interactionsManager;
	 private AudioListener _audioListener;
	 private string _backgroundName;
	 [SerializeField]
	 private int _targertFPS = 15;
	 private Tayx.Graphy.GraphyManager _fpsView;


	 private void OnApplicationFocus(bool focus)
	 {
		CheckCursor();
	 }

	 private void Awake()
	 {
		_gameKeys = new GameKeys();
		_gameKeys.Initiate();
		_cursorVisibleList = new List<Component>();
		_escapeStack = new List<IEscape>();
		ParceCommandLine();
		//Application.targetFrameRate = _targertFPS;

	 }

	 private void OnEnable()
	 {
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ConsoleOpen, ConsoleOpen);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ConsoleClose, ConsoleClose);
	 }

	 private void OnDisable()
	 {
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ConsoleOpen, ConsoleOpen);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ConsoleClose, ConsoleClose);

		com.ootii.Messages.MessageDispatcher.ClearListeners();
		com.ootii.Messages.MessageDispatcher.ClearMessages();

	 }

	 public void PlayBackgroundMusic(string soundName)
	 {
		if (string.IsNullOrEmpty(soundName) || _backgroundName == soundName)
		  return;

		if (DarkTonic.MasterAudio.MasterAudio.TriggerPlaylistClip(soundName))
		  _backgroundName = soundName;
	 }

	 private void Start()
	 {
		Game.Events.EventDispatcher.AddListener(Game.Player.Stats.Health.EVT_IS_DEAD, PlayerIsDead);
		Game.Events.EventDispatcher.AddListener(it.Game.Scenes.LocationManager.EVT_START_LEVEL, StartLevel, true);
		DOTween.Init();
		Phase = GamePhase.menu;
		CursorDisable();

	 }

	 private void ParceCommandLine()
	 {
		string[] arguments = System.Environment.GetCommandLineArgs();

#if UNITY_EDITOR
		Instantiate(_consolePrefab);
#endif


		for (int i = 0; i < arguments.Length; i++)
		{
		  if (arguments[i] == "-console")
		  {
			 Instantiate(_consolePrefab);
		  }
		  if (arguments[i] == "-develop")
		  {
			 IsDevelop = true;
		  }
		  if (arguments[i] == "-control")
		  {
			 IsControl = true;
		  }
		  if (arguments[i] == "-log")
		  {
			 IsLog = true;
		  }
		}

#if UNITY_EDITOR
		Instantiate(_consolePrefab);
		IsDevelop = true;
		IsControl = true;
		IsLog = true;
		//NewPlayer = true;
#endif

	 }

	 private bool _consoleOpen;
	 private bool BaseInputSourceBeforeConsole;
	 private bool GameInputSourceBeforeConsole;
	 private bool EnvironmentInputSourceBeforeConsole;
	 private void ConsoleOpen(com.ootii.Messages.IMessage handler)
	 {
		_consoleOpen = true;
		GameInputSourceBeforeConsole = GameInputSource.IsEnabled;
		EnvironmentInputSourceBeforeConsole = EnvironmentInputSource.IsEnabled;
		BaseInputSourceBeforeConsole = BaseInputSource.IsEnabled;
		BaseInputSource.IsEnabled = false;
		GameInputSource.IsEnabled = false;
		EnvironmentInputSource.IsEnabled = false;
	 }
	 private void ConsoleClose(com.ootii.Messages.IMessage handler)
	 {
		if (!_consoleOpen) return;
		_consoleOpen = false;
		GameInputSource.IsEnabled = GameInputSourceBeforeConsole;
		EnvironmentInputSource.IsEnabled = EnvironmentInputSourceBeforeConsole;
		BaseInputSource.IsEnabled = BaseInputSourceBeforeConsole;
	 }

	 [QFSW.QC.Command]
	 private static void Test()
	 {

	 }

	 private void StartLevel(com.ootii.Messages.IMessage messag)
	 {
		LocationManager.StartLevel();
		_isLoadindSceneProcess = false;
	 }

	 public void SetPlayerControl(bool isControl)
	 {
		Game.Player.PlayerBehaviour.Instance.PlayerControl = isControl;
	 }
	 public void SetCameraPlayerFollow(bool isFollow)
	 {
		CameraBehaviour.Instance.PlayerFollow = isFollow;
	 }

	 public void CameraFixPosition(Transform targetPosition, bool interpolate = true, bool playerControlDeactive = false)
	 {
		//SetCameraPlayerFollow(false);
		//SetPlayerControl(false);
		MoveTargetCam mc = CameraBehaviour.Instance.gameObject.AddComponent<MoveTargetCam>();
		mc.StartMove(targetPosition);
	 }
	 public void CameraPlayerFollow(bool playerControlActive = true)
	 {
		MoveTargetCam mc = CameraBehaviour.Instance.gameObject.GetComponent<MoveTargetCam>();
		if (mc != null)
		  Destroy(mc);
		//SetCameraPlayerFollow(true);
		//SetPlayerControl(true);
	 }


	 #region Develop

	 [Command("Develop.ShowCoordinate")]
	 private void ShowCoorditate(bool isOpen)
	 {
		Debug.Log("ShowCoorditate");
		PlayerCoordinateUI.gameObject.SetActive(isOpen);
	 }

	 [Command("Develop.ShowFps")]
	 private void ShowFps(bool isOpen)
	 {
		FpsView.gameObject.SetActive(isOpen);
	 }
	 [Command("Develop.ShowFastTeleport")]
	 private void ShowFastTeleport(bool isOpen)
	 {
		var panel = UiManager.GetPanel<it.UI.SpawnNavigation>();
		panel.gameObject.SetActive(isOpen);
	 }
	 [Command("Develop.ShowVideoScene")]
	 private void ShowVideoScene(bool isOpen)
	 {
		var panel = UiManager.GetPanel<it.UI.VideoScene>();
		panel.gameObject.SetActive(isOpen);
	 }

	 [Command("ShowVideoScene.ShowCursor")]
	 private void ShowCursor(bool isOpen)
	 {
		Cursor.visible = isOpen;
	 }
	 [Command("Develop.LoadScene")]
	 private void LoadScene(string sceneName)
	 {
		var scene = UnityEngine.SceneManagement.SceneManager.GetSceneByName(sceneName);

		if (scene == null)
		  return;

		Game.Managers.UiManager.CloseAllPanels();

		StartGame(sceneName);
	 }

	 #endregion

	 public void StartGame(int level = -1)
	 {
		if (_isLoadindSceneProcess)
		  return;

		int levelNum = UserManager.PlayerProgress.Level;

		if (level >= 0)
		{
		  levelNum = level;
		  UserManager.PlayerProgress.SetLevel(levelNum);
		}

		string sceneName = ProjectSettings.LevelScenes[levelNum];

		_isLoadindSceneProcess = true;

		_targetNewLevel = levelNum;

		SceneManager.LoadScene(sceneName, false);
		//StartCoroutine(LoadSceneIenum(sceneName));
	 }

	 public void StartGame(string scene)
	 {
		if (_isLoadindSceneProcess)
		  return;
		SceneManager.LoadScene(scene, false);
	 }

	 public void LoadScene(string sceneName, bool levelchange = false)
	 {

	 }
	 [ContextMenu("NextLevel")]
	 public void NextLevel()
	 {

		int levelNum = GameManager.Instance.LocationManager.LevelIndex - 1;
		levelNum++;

		if (levelNum >= 2)
		{
		  _targetNewLevel = levelNum;
		  LoadWhell();
		  return;
		}

		NextLevel(levelNum);
	 }

	 private void LoadWhell()
	 {
		SceneManager.LoadScene("TimeWhell", false);
	 }

	 public void NextLevel(int level, bool visual = true)
	 {
		UserManager.PlayerProgress.SetLevel(level);

		string sceneName = ProjectSettings.LevelScenes[level];

		_isLoadindSceneProcess = true;
		SceneManager.LoadScene(sceneName, visual);
	 }

	 public void LoadMenu()
	 {
		Phase = GamePhase.menu;
		Game.Managers.GameManager.Instance.SceneManager.LoadScene("Menu", false);
	 }

	 private IEnumerator LoadSceneIenum(string sceneName)
	 {

		UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
		yield break;

		string actialScene = GameManager.Instance.SceneManager.ActiveScene;

		var panel = UiManager.GetPanel<it.UI.SceneLoader.SceneLoader>();
		panel.gameObject.SetActive(true);
		panel.onLoadComplete = () =>
		{
		  InvokeEndFrame(() =>
		  {
			 panel.gameObject.SetActive(false);
			 //Game.Managers.GameManager.Instance.SceneManager.UnloadSceneAsync(actialScene);
			 //LocationManager.StartLevel();
		  });
		};

		Phase = GamePhase.game;

		yield return null;

		Game.Managers.GameManager.Instance.SceneManager.LoadSceneWishUnloadAsync(sceneName, actialScene);
	 }

	 public void QuitGame()
	 {
		com.ootii.Messages.MessageDispatcher.SendMessage(EVT_GAMEQUITING, 0);
		IsQuiting = true;
		Application.Quit();
	 }

	 private bool _gameMenuPanelProcess = false;

	 /// <summary>
	 /// Открытие панели игрового меню
	 /// </summary>
	 public void OpenGameMenuPanel()
	 {
		var panel = UiManager.GetPanel<it.UI.GameMenu.GameMenu>();

		panel.onEnamble = () =>
		{
		  GameInputSource.IsEnabled = false;
		};
		panel.onDisable = () =>
		{
		  GameInputSource.IsEnabled = true;
		};

		if (panel.isActiveAndEnabled)
		{
		  panel.gameObject.SetActive(false);
		  return;
		}

		panel.gameObject.SetActive(true);

		panel.onSettingsButtonClick = () =>
		{
		  panel.gameObject.SetActive(false);
		  GameManager.Instance.GameSettings.OpenSettingsPanel(() =>
		  {
			 panel.gameObject.SetActive(true);
		  });
		};
		panel.onContinueButtonClick = () =>
		{
		  panel.gameObject.SetActive(false);
		};
		panel.onLoadLastClick = () =>
		{
		  if (_gameMenuPanelProcess)
			 return;

		  _gameMenuPanelProcess = true;

		  UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
			 new Color32(0, 0, 0, 255), 1, null, () =>
			{

			  panel.gameObject.SetActive(false);
			  LoadLast();
			}, () =>
			{
			  _gameMenuPanelProcess = false;

			});
		};
		panel.onExitButtonClick = () =>
		{
		  Game.Managers.GameManager.Instance.SceneManager.LoadScene("Menu", false, () =>
		  {
			 Phase = GamePhase.menu;
			 //LoadMenu();
			 panel.gameObject.SetActive(false);
		  });
		};
	 }

	 private void PlayerIsDead(com.ootii.Messages.IMessage message)
	 {
		InvokeSeconds(() =>
		{

		  UiManager.Instance.FillAndRepeatColor(new Color32(0, 0, 0, 0),
			 new Color32(0, 0, 0, 255), 1, null, () =>
			{
			  LoadLast();
			}, () =>
			{
			  _gameMenuPanelProcess = false;

			});

		}, 2);
	 }

	 private void LoadLast()
	 {
		UserManager.LoadLast();
	 }

	 #region Инвентарь

	 private bool _showInventary = false;

	 public void InventaryPanel()
	 {
		_showInventary = !_showInventary;

		var panel = UiManager.GetPanel<it.UI.Inventary.InventaryPanel>();

		panel.onEnamble = () =>
		{
		  GameInputSource.IsEnabled = false;
		};
		panel.onDisable = () =>
		{
		  GameInputSource.IsEnabled = true;
		};
		if (_showInventary)
		  panel.Show();
		else
		  panel.Hide();
		//panel.gameObject.SetActive(_showInventary);
	 }

	 #endregion

	 #region Escape Stack


	 private List<IEscape> _escapeStack = new List<IEscape>();

	 public void AddEscape(IEscape obj)
	 {
		_escapeStack.Add(obj);
	 }
	 public void RemoveEscape(IEscape obj)
	 {
		_escapeStack.Remove(obj);
	 }

	 public void OnEscape()
	 {

		if (_escapeStack.Count == 0)
		{
		  if (SceneBehaviour != null && !SceneBehaviour.IsEscapedToMenu)
			 return;

		  OpenGameMenuPanel();
		  return;
		}

		IEscape escItem = _escapeStack[_escapeStack.Count - 1];

		escItem.Escape();

	 }

	 #endregion

	 #region Cursor

	 private List<Component> _cursorVisibleList = new List<Component>();

	 public void SetCursorVisible(Component behaviour, bool isVisible)
	 {
		if (isVisible)
		{
		  if (!_cursorVisibleList.Contains(behaviour))
		  {
			 _cursorVisibleList.Add(behaviour);
		  }
		}
		else
		{
		  _cursorVisibleList.Remove(behaviour);
		}

		CheckCursor();
	 }

	 private void CheckCursor()
	 {

		if (_cursorVisibleList.Count > 0)
		  CursorEnable();
		else
		  CursorDisable();
	 }

	 private void CursorEnable()
	 {
#if !UNITY_EDITOR
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.Confined;
#endif
	 }
	 private void CursorDisable()
	 {
#if !UNITY_EDITOR
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
#endif
	 }

	 #endregion
  }
  [System.Flags]
  public enum GamePhase
  {
	 none = 0,
	 menu = 1,
	 game = 2,
	 pause = 4
  }

}