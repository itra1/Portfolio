using UnityEngine;
using System.Collections;
using it.UI.Symbols;

namespace it.Game.Items.Symbols
{
  
  public class Symbol : Item, IInteraction
  {
	 public bool IsInteractReady => !_isRemovedProcess;

	 [SerializeField]
	 private it.UI.Symbols.SymbolAnimItem _uiIten;
	 public SymbolAnimItem UiIten { get => _uiIten; set => _uiIten = value; }

	 /// <summary>
	 /// Просеыы удаления символа
	 /// </summary>
	 private bool _isRemovedProcess = false;

	 /// <summary>
	 /// Получить
	 /// </summary>
	 protected virtual void GetThis()
	 {
		Game.Managers.GameManager.Instance.SymbolsManager.AddItem(Uuid);

		GetComponentInChildren<UnityEngine.VFX.VisualEffect>().SendEvent("OnStop");

		DarkTonic.MasterAudio.MasterAudio.PlaySound3DAtTransformAndForget("Effects", transform, 1, null, 0, "GetSymbol");

		InvokeSeconds(() => {
		  gameObject.SetActive(false);
		}, 2);
	 }

	 public virtual void StartInteract()
	 {
		GetThis();
	 }

	 public virtual void StopInteract()
	 {
		return;
	 }
	 private void OnDrawGizmos()
	 {
		Gizmos.DrawIcon(transform.position, "Chaos.png");
	 }
  }
}