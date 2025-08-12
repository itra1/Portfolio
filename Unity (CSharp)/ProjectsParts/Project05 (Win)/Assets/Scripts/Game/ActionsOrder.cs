using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace it
{
  public class ActionsOrder : Singleton<ActionsOrder>
  {
	 private Queue<IActionOrder> _actionOrder = new Queue<IActionOrder>();

	 private int _id = 0;
	 private bool _isRunning = false;

	 private void Awake()
	 {
		_actionOrder = new Queue<IActionOrder>();
		_id = 0;
	 }

	 private void Start()
	 {
		_id = 0;
		_isRunning = false;
		Game.Events.EventDispatcher.AddListener(EventsConstants.PlayerProgressLoad, LoadHandler);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ActionsOrderAdd, NewItem);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.ActionsOrderRemove, RemoveItem);
	 }
	 public void LoadHandler(com.ootii.Messages.IMessage handler)
	 {
		_id = 0;
		_isRunning = false;
		_actionOrder.Clear();
	 }


	 private void NewItem(com.ootii.Messages.IMessage handle)
	 {
		IActionOrder newAction = (IActionOrder)handle.Data;

		_actionOrder.Enqueue(newAction);

		if (!_isRunning)
		  ActivateNext();
	 }

	 private void RemoveItem(com.ootii.Messages.IMessage handle)
	 {
		int removeId = (int)handle.Data;

		if (removeId != _id)
		  return;

		_isRunning = false;

		ActivateNext();

	 }

	 private void ActivateNext()
	 {
		while (!_isRunning && _actionOrder.Count > 0)
		{
		  var action = _actionOrder.Dequeue();

		  if (action == null)
			 return;

		  
		  _isRunning = true;
		  action.ActionOrderID = ++_id;
		  action.ActionOrder();
		}

	 }

  }

  public interface IActionOrder
  {
	 int ActionOrderID { get; set; }
	 void ActionOrder();
  }

}
