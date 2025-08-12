using UnityEngine;
using System.Collections;
using com.ootii.Messages;
using it.Game.Player;
using static it.Game.Player.Interactions.InteractionObject;

namespace it.Game.Environment
{
  public class FormActivator : MonoBehaviourBase
  {
	 [SerializeField]
	 private int targetform = 1;
	 [SerializeField]
	 private GameObject[] _objects;
	 [SerializeField]
	 private bool _setActivate = true;
	 public void Start()
	 {
		ActivateObjects(!_setActivate);
		if(PlayerBehaviour.Instance != null)
		  ActivateObjects(!_setActivate);
		MessageDispatcher.AddListener(EventsConstants.PlayerFormChange, OnFormChange, true);
	 }
	 private void OnDestroy()
	 {

		MessageDispatcher.RemoveListener(EventsConstants.PlayerFormChange, OnFormChange, true);
	 }

	 private void OnFormChange(IMessage message )
	 {
		ChangeForm();
	 }

	 private void ChangeForm()
	 {
		ActivateObjects(PlayerBehaviour.Instance.Form == targetform ? _setActivate : !_setActivate);
		
	 }

	 private void ActivateObjects(bool isActive)
	 {
		for(int i = 0; i < _objects.Length; i++)
		{
		  _objects[i].SetActive(isActive);
		}
	 }

  }
}