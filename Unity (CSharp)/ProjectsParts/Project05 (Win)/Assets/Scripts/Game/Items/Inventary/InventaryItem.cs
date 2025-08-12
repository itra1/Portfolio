using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace it.Game.Items.Inventary
{
  /// <summary>
  /// Предмет инвенторя
  /// </summary>
  public class InventaryItem : Item, Game.Items.IInteraction
  {
	 /// <summary>
	 /// Это кармический предмет
	 /// </summary>
	 public bool IsCarmic => _isCarmic;

	 [Tooltip("Это кармический предмет.")]
	 [SerializeField]
	 private bool _isCarmic = false;

	 private bool _checkExistsAndDeactive = true;
	 public bool CheckExistsAndDeactive { get => _checkExistsAndDeactive; set => _checkExistsAndDeactive = value; }

	 public bool IsInteractReady => true;

	 /// <summary>
	 /// Скрытые предметы работы с прогрессом
	 /// </summary>
	 [SerializeField]
	 private bool _isSystem;
	 public bool IsSystem => _isSystem;

	 protected virtual void OnEnable()
	 {
		if (_checkExistsAndDeactive)
		{
		  if (Game.Managers.GameManager.Instance.Inventary.ExistsItem(Uuid))
			 gameObject.SetActive(false);
		}
		return;
	 }

	 public void StartInteract()
	 {
		gameObject.SetActive(false);
	 }
	 public void GetItemAnimate()
	 {
		ColorHide(() =>
		{

		  GetItem();
		});
	 }
	 [ContextMenu("GetItem")]
	 public void GetItem()
	 {
		Game.Managers.GameManager.Instance.Inventary.AddItem(Uuid, 1);
	 }

	 public void StopInteract()
	 {
		return;
	 }

	 public void ClearUsageItem()
	 {

		DestroyImmediate(this);
	 }

	 private void OnDrawGizmos()
	 {
		Gizmos.DrawIcon(transform.position, "Item.png");
	 }

  }
}