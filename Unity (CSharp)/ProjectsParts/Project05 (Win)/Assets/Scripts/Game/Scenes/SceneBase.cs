using UnityEngine;
using it.Game.Managers;

#if UNITY_EDITOR
using UnityEditor;
#endif


namespace it.Game.Scenes
{
  public abstract class SceneBehaviour : MonoBehaviourBase
  {

	 [SerializeField]
	 private string _backgroundMusic;
	 /// <summary>
	 /// Допустимость нажатия Escape
	 /// </summary>
	 [SerializeField]
	 private bool _isEscapedToMenu = true;
	 public bool IsEscapedToMenu { get => _isEscapedToMenu; set => _isEscapedToMenu = value; }

	 protected virtual void Start()
	 {
		GameManager.Instance.PlayBackgroundMusic(_backgroundMusic);
	 }


#if UNITY_EDITOR

	 [ContextMenu("Find objects With Missing Script")]
	 public void FundObjects()
	 {

		Transform[] list = GameObject.FindObjectsOfType<Transform>(true);
		for(int i = 0; i < list.Length; i++)
		{
		  int count = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(list[i].gameObject);
		  if(count > 0)
			 Debug.Log(list[i].gameObject.name);
		}
		//
		//Debug.Log(count);
	 }

#endif


  }

}