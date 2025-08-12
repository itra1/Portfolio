using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace it.UI.SceneLoader
{
  public class SceneLoader : UIDialog
  {
	 [SerializeField]
	 private RectTransform _loaderProgressLine;

	 public UnityEngine.Events.UnityAction onLoadComplete;

	 private void Awake()
	 {
		_loaderProgressLine.localScale = new Vector2(0.001f, 1);
	 }

	 protected override void OnEnable()
	 {
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UIEnable, OtherWindowsEnabled);
		base.OnEnable();
		_loaderProgressLine.localScale = new Vector2(0, 1);
	 }

	 private void OtherWindowsEnabled(com.ootii.Messages.IMessage data)
	 {
		GetComponent<RectTransform>().SetAsLastSibling();
	 }

	 protected override void OnDisable()
	 {
		base.OnDisable();
	 }

	 private void Start()
	 {

		_loaderProgressLine.localScale = new Vector2(0.001f, 1);


		it.Game.Events.EventDispatcher.AddListener(it.Game.Managers.SceneManager.EVT_LOAD_SCENE_PROGRESS, (message) =>
		{
		  if(((it.Game.Events.Messages.LoadScenePercent)message).SceneName == it.Game.Managers.GameManager.Instance.SceneManager.TargetScene)
		  _loaderProgressLine.localScale = new Vector2(((it.Game.Events.Messages.LoadScenePercent)message).Progress, 1);
		});

		it.Game.Events.EventDispatcher.AddListener(it.Game.Managers.SceneManager.EVT_LOAD_SCENE, (message) =>
		{
		  if (((string)message.Data) == it.Game.Managers.GameManager.Instance.SceneManager.TargetScene)
			 onLoadComplete?.Invoke();
		  if(_loaderProgressLine.localScale.x > .5f)
			 _loaderProgressLine.localScale = new Vector2(1, 1);
		});

	 }


  }
}