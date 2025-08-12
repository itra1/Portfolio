using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif
namespace it.Game.Managers
{
  /// <summary>
  /// Менеджер загрузки
  /// </summary>
  public class ManagersLoader : MonoBehaviour
  {
	 [SerializeField]
	 private GameManager _gameManager;
	 [SerializeField]
	 private UnityEngine.EventSystems.EventSystem _eventSystem;
	 [SerializeField]
	 private UiManager _guiManager;
	 [SerializeField]
	 private DarkTonic.MasterAudio.PlaylistController _masterAudioPlayList;
	 [ContextMenuItem("Instantiate", "MasterAudioInstantiate")]
	 [SerializeField]
	 private DarkTonic.MasterAudio.MasterAudio _masterAudio;

	 private void Awake()
	 {
		if (FindObjectOfType<GameManager>() == null)
		  Instantiate(_gameManager.gameObject);

		if (FindObjectOfType<UnityEngine.EventSystems.EventSystem>() == null)
		  Instantiate(_eventSystem.gameObject);

		if (FindObjectOfType<UiManager>() == null)
		  Instantiate(_guiManager.gameObject);

		if (FindObjectOfType<DarkTonic.MasterAudio.MasterAudio>() == null)
		  Instantiate(_masterAudio.gameObject);

		if (FindObjectOfType<DarkTonic.MasterAudio.PlaylistController>() == null)
		  Instantiate(_masterAudioPlayList.gameObject);

	 }

	 /// <summary>
	 /// Выполняется в редакторе
	 /// </summary>
	 private void MasterAudioInstantiate()
	 {
#if UNITY_EDITOR
		PrefabUtility.InstantiatePrefab(_masterAudio.gameObject);
#endif
	 }

  }
}