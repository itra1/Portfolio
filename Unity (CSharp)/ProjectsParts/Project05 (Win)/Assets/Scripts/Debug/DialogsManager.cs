using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using it.Game.Managers;
using QFSW.QC;

namespace it.Dialogue
{
  public class DialogsManager : MonoBehaviourBase
  {

	 private DialogueLibrary _library;
	 private Dictionary<string, GameObject> _dataPrefabs = new Dictionary<string, GameObject>();
	 private Queue<DialogQueue> _queue = new Queue<DialogQueue>();

	 private bool isVisibleDialog = false;

	 private void Awake()
	 {
		//if (GameManager.IsDevelop)
		//  {
		//	 SickDev.DevConsole.DevConsole.singleton.AddCommand(new SickDev.CommandSystem.ActionCommand<string>(ShowDialog) { className = "Dialog" });
		//	 SickDev.DevConsole.DevConsole.singleton.AddCommand(new SickDev.CommandSystem.ActionCommand<int>(ShowAllDialogLevel) { className = "Dialog" });
		//  }
		_queue = new Queue<DialogQueue>();
	 }

	 private void Start()
	 {
		_library = Resources.Load<DialogueLibrary>(Game.ProjectSettings.DialogsLibrary);
	 }
	 public GameObject GetPrefab(string guid)
	 {
		if (!_dataPrefabs.ContainsKey(guid))
		  LoadPrefab(guid);

		return _dataPrefabs[guid];
	 }
	 private void LoadPrefab(string uuid)
	 {
		_dataPrefabs.Add(uuid, Resources.Load<GameObject>(_library.GetPath(uuid)));
	 }
	 [Command("Dialog.LoadScene")]
	 public void ShowDialog(string uuid)
	 {
		ShowDialog(uuid, null, null, null);
	 }
	 [Command("Dialog.ShowAllDialogLevel")]
	 public void ShowAllDialogLevel(int level)
	 {
		GameObject[] objects = Resources.LoadAll<GameObject>("Prefabs/Dialogs/");
		
		for(int i = 0; i < objects.Length; i++)
		{
		  it.Dialogue.Dialogue dial = objects[i].GetComponent<it.Dialogue.Dialogue>();

		  if(dial != null)
		  {
			 ShowDialog(dial.Uuid);
		  }
		}

	 }

	 public void ShowDialog(string uuid, 
		UnityEngine.Events.UnityAction onStart, 
		UnityEngine.Events.UnityAction<int> onNextFrame, 
		UnityEngine.Events.UnityAction onComplete)
	 {
		_queue.Enqueue(new DialogQueue()
		{
		  uuid = uuid,
		  onStart = onStart,
		  onNextFrame = onNextFrame,
		  onComplete = onComplete
		});

		CheckView();
	 }

	 private void CheckView()
	 {
		if (isVisibleDialog)
		  return;
		if (_queue.Count <= 0)
		  return;

		DialogQueue dialog = _queue.Dequeue();
		GameObject pref = GetPrefab(dialog.uuid);

		isVisibleDialog = true;

		pref.GetComponent<Dialogue>().Show(()=> {
		  dialog.onStart?.Invoke();
		}, 
		(x)=> {
		  dialog.onNextFrame?.Invoke(x);
		}, 
		()=> {
		  dialog.onComplete?.Invoke();
		  isVisibleDialog = false;
		  DOVirtual.DelayedCall(1, () => {
			 CheckView();
		  });
		});
	 }

	 private struct DialogQueue
	 {
		public string uuid;
		public UnityEngine.Events.UnityAction onStart;
		public UnityEngine.Events.UnityAction<int> onNextFrame;
		public UnityEngine.Events.UnityAction onComplete;
	 }

  }
}