using UnityEngine;
using System.Collections;

namespace it.Game.Items.Symbols
{
  public class SymbolInteraction : MonoBehaviour, it.Game.Player.Interactions.IInteractionCondition
  {
	 private Symbol _symbol;
	 public Symbol Symbol
	 {
		get {
		  if (_symbol == null)
			 _symbol = GetComponentInChildren<Symbol>();
		  return _symbol;
		}
		set
		{
		  _symbol = value;
		}
	 }

	 private string _symbolUuid;
	 private bool _exists;

	 private void Start()
	 {
		_symbolUuid = Symbol.Uuid;
		Check();
	 }

	 private void OnEnable()
	 {
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SymbolLoad, SymbolsLoad);
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.SymbolGetItem, SymbolsChange);
	 }

	 private void OnDisable()
	 {
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SymbolLoad, SymbolsLoad);
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.SymbolGetItem, SymbolsChange);
	 }

	 private void SymbolsLoad(com.ootii.Messages.IMessage handle)
	 {
		Check();
		Symbol.gameObject.SetActive(!_exists);
	 }
	 private void SymbolsChange(com.ootii.Messages.IMessage handle)
	 {
		Check();
	 }

	 private void Check()
	 {
		_exists = Managers.GameManager.Instance.SymbolsManager.ExistsItem(_symbolUuid);
	 }

	 public void Interact()
	 {
		Symbol.StartInteract();
	 }

	 public bool InteractionReady()
	 {
		return !_exists;
	 }
  }
}