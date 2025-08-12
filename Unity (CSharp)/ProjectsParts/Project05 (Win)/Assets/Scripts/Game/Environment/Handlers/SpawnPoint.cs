using it.Game.Scenes;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace it.Game.Environment.Handlers
{

#if UNITY_EDITOR
  [CustomEditor(typeof(SpawnPoint))]
  public class SpawnPointEditor : Editor
  {
	 public override void OnInspectorGUI()
	 {

		if (GUILayout.Button("Установить стартовым"))
		  ((SpawnPoint)target).SetStartInEditor();

			base.OnInspectorGUI();
		}
  }
#endif


  /// <summary>
  /// Точка вероятного спавна
  /// </summary>
  public class SpawnPoint : MonoBehaviourBase, it.Game.Player.IPlayerTriggerEnter
  {

	 [SerializeField] [UUID] private string _uuid;
	 [SerializeField] private string _title;
	 [SerializeField] private bool _noSave = false;
	 [SerializeField] private bool _isFirst = false;
	 [SerializeField] private bool _saveEnter = true;
	 [SerializeField] private bool _saveExit = true;
	 [SerializeField] private Transform _spawnPosition;
	 [Space]
	 [SerializeField] private bool _editorFirst = false;
	 [SerializeField] private bool _editorOnly = false;

	 public string Title => _title;
	 public bool IsFirst => _isFirst;
	 public bool EditorFirst => _editorFirst;
	 public Transform SpawnPosition { get => _spawnPosition; set => _spawnPosition = value; }
	 public string Uuid => _uuid;


	 private void OnDrawGizmos()
	 {
		if (_spawnPosition == null) return;

		Gizmos.DrawIcon(_spawnPosition.position, "SpawnPoint.png");
		Game.Utils.DrawArrow.ForGizmo(_spawnPosition.position, _spawnPosition.forward);
		var collider = GetComponent<Collider>();

		if(collider != null) collider.isTrigger = true;
	 }

	 private void OnDrawGizmosSelected()
	 {
		string name = "SpawnPoint - " + _title;

		if (gameObject.name != name)
		  gameObject.name = name;
	 }

	 public void OnPlayerTriggerEnter()
	 {
		if (!_saveEnter) return;

		if (_noSave) return;

		if (_editorOnly)
		{
#if !UNITY_EDITOR
		  return;
#endif
		}

		SendData(Game.Player.PlayerBehaviour.Instance.transform.localEulerAngles.y);
	 }

	 public void OnPlayerTriggerExit()
	 {
		if (!_saveExit) return;

		if (_noSave) return;
		if (_editorOnly)
		{
#if !UNITY_EDITOR
		  return;
#endif
		}

		SendData(Game.Player.PlayerBehaviour.Instance.transform.localEulerAngles.y);
	 }

	 private void SendData(float roteteY)
	 {
		Game.Events.Messages.SpawnPosition eventData = Game.Events.Messages.SpawnPosition.Allocate();
		eventData.Type = Game.Scenes.LocationManager.EVT_NEW_POSITION;
		eventData.Sender = this;
		eventData.Uuid = Uuid;
		eventData.RotationY = roteteY;
		Game.Events.EventDispatcher.SendMessage(eventData);
	 }

#if UNITY_EDITOR

	 [ContextMenu("SetStart")]
	 public void SetStartInEditor()
	 {
		MonoBehaviour.FindObjectOfType<LocationManager>()._uuidRestorePoint = Uuid;
	 }

#endif

  }
}