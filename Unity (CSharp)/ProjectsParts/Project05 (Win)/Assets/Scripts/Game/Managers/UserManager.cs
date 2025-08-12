using UnityEngine;
using it.Game.Player;
using it.Game.Player.Save;
using it.Game.Player.Stats;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Managers
{

  public class UserManager : MonoBehaviourBase
  {
	 public const string ENT_PLAYER_INSTANT = "PLAYER_INSTANCE";
	 /// <summary>
	 /// Ссылка на инстанс игрока
	 /// </summary>
	 public PlayerBehaviour PlayerBehaviour => _playerInstance;

	 /// <summary>
	 /// Прогресс игрока
	 /// </summary>
	 public PlayerProgress PlayerProgress
	 {
		get
		{
		  if (_playerProgress == null)
			 _playerProgress = Resources.Load<Game.Player.Save.PlayerProgress>(Game.ProjectSettings.PlayerProgress);
		  return _playerProgress;
		}
	 }
	 /// <summary>
	 /// Текущий показатель жизней
	 /// </summary>
	 public Player.Stats.Health Health{ 
		get {

		  if (_health == null)
			 _health = GetComponent<Game.Player.Stats.Health>();
		  if (_health == null)
			 _health = gameObject.AddComponent<Game.Player.Stats.Health>();

		  return _health;

		} set => _health = value; }

	 /// <summary>
	 /// МАксимальный показатель жизней
	 /// </summary>


	 private GameObject _playerPrefab = null;
	 private PlayerBehaviour _playerInstance = null;
	 private Game.Player.Stats.Health _health;
	 private PlayerProgress _playerProgress = null;

	 public void Start()
	 {
		Game.Events.EventDispatcher.AddListener(EventsConstants.PlayerProgressSave, (message) =>
		{
		  if (message is Game.Events.Messages.SaveData)
			 PlayerProgress.SaveData(message as Game.Events.Messages.SaveData);
		});

		Load();
	 }

	 public void NewGame()
	 {
		PlayerPrefs.DeleteAll();
		PlayerProgress.FullClear();
		Load();
	 }

	 public void Load()
	 {
		PlayerProgress.Load();
	 }


	 public void LoadLast()
	 {
		Health.Restore();
		Load();
		if (Game.Managers.GameManager.Instance.LocationManager != null)
		  ResoreUserPosition(Game.Managers.GameManager.Instance.LocationManager.RestorePoint);
		GameManager.Instance.GameInputSource.IsEnabled = true;
	 }

	 /// <summary>
	 /// Создание экземпляра плеера, если такового нет
	 /// </summary>
	 public void InstantiatePlayer()
	 {
		if (_playerInstance != null)
		  return;

		Health.Init();

		if (_playerPrefab == null)
		  LoadPrefab();


		GameObject inst = Instantiate(_playerPrefab);
		_playerInstance = inst.GetComponentInChildren<PlayerBehaviour>();
		_playerInstance.RegisterVars();
		inst.SetActive(true);
		EminInstanceEvent();
	 }

	 private void EminInstanceEvent()
	 {
		Game.Events.EventDispatcher.SendMessage(ENT_PLAYER_INSTANT);
	 }
	 /// <summary>
	 /// Восстановление плеера на соответствующую позицию
	 /// </summary>
	 /// <param name="target"></param>
	 public void ResoreUserPosition(Transform target)
	 {

		if (PlayerBehaviour == null)
		  InstantiatePlayer();

		PlayerBehaviour.ActorController.IsEnabled = true;
		PlayerBehaviour.ActorController.IsGravityEnabled = true;
		PlayerBehaviour.ActorController.ForceGrounding = true;
		PlayerBehaviour.MotionController.IsEnabled = true;
		PlayerBehaviour.transform.parent = null;
		PlayerBehaviour.transform.position = target.position;
		PlayerBehaviour.transform.rotation = Quaternion.Euler(0, GameManager.Instance.UserManager.PlayerProgress.SpawnRotationY,0);
		PlayerBehaviour.PortalJump(PlayerBehaviour.transform);

		//PlayerBehaviour.MotionController.Reset();
		//PlayerBehaviour.ActorController.SetVelocity(Vector3.zero);

		var cameraController = CameraBehaviour.Instance.GetComponent<com.ootii.Cameras.CameraController>();
		  cameraController.Anchor = PlayerBehaviour.transform;

		PlayerBehaviour.MotionController.CameraRig = cameraController;

		CameraBehaviour.Instance.CameraController.ActivateMotor(0);

		InvokeEndFrame(() => {
		  
			 PlayerBehaviour.MotionController.CameraRig.Transform.rotation
				= target.rotation;
		  
		});
	 }
	 
	 private void LoadPrefab()
	 {
		string path = ProjectSettings.PlayerPrefab;
		_playerPrefab = Resources.Load<GameObject>(path);
	 }

	 #region Health

	 #endregion

  }
}