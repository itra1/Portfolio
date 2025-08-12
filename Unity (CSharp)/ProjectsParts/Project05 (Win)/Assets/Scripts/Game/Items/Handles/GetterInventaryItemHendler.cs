using UnityEngine;
using System.Collections;

namespace it.Game.Items.Handles
{

  /// <summary>
  /// Компонент сообщает, если предмет инвентаря подобрат
  /// </summary>
  public class GetterInventaryItemHendler : MonoBehaviourBase
  {
	 [SerializeField]
	 private string _targetItemUuid = "";

	 [SerializeField]
	 private UnityEngine.Events.UnityEvent _onGet;

	 private bool _isComplete = false;

	 private void Start()
	 {
		_isComplete = false;
		Subscrive();
		CheckExists();
	 }
	 
	 private void OnDestroy()
	 {
		UnSubscrive();
	 }

	 private void Subscrive()
	 {

		Events.EventDispatcher.AddListener(EventsConstants.InventaryGetItem, InventaryGetItem);
		Events.EventDispatcher.AddListener(EventsConstants.InventaryLoad, InventaryLoad);
	 }
	 private void UnSubscrive()
	 {

		Events.EventDispatcher.RemoveListener(EventsConstants.InventaryGetItem, InventaryGetItem);
		Events.EventDispatcher.RemoveListener(EventsConstants.InventaryLoad, InventaryLoad);
	 }

	 //com.ootii.Messages.IMessage rMessage
	 private void InventaryLoad(com.ootii.Messages.IMessage rMessage)
	 {
		CheckExists();

	 }
	 private void InventaryGetItem(com.ootii.Messages.IMessage rMessage)
	 {
		CheckExists();
	 }

	 private void CheckExists()
	 {
		if (_isComplete)
		  return;

		if (Managers.GameManager.Instance.Inventary.ExistsItem(_targetItemUuid))
		{
		  _isComplete = true;
		  _onGet?.Invoke();
		  UnSubscrive();
		}

	 }

  }
}