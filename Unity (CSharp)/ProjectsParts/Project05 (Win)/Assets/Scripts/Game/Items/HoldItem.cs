using UnityEngine;
using System.Collections;
using UnityEngine.Events;
using it.Game.Player;

namespace it.Game.Items
{
  /// <summary>
  /// Тип удерживаемой руки
  /// </summary>
  [System.Flags]
  public enum HoldHandType
  {
	 left = 1,
	 right = 2
  }


  /// <summary>
  /// Предмет удерживаемый играком в руке
  /// </summary>
  public class HoldItem : PlayerComponent, IInteraction
  {

	 [Tooltip("Тип удерживания")]
	 [SerializeField]
	 private HoldHandType _handType = HoldHandType.right;

	 /// <summary>
	 /// Типа удерживаемой руки
	 /// </summary>
	 public HoldHandType Hand { get => _handType; }

	 [SerializeField]
	 [Tooltip("Локальное положение в руке")]
	 private Vector3 _LocalPosition = Vector3.zero;
	 /// <summary>
	 /// Локальное положение в руке
	 /// </summary>
	 public Vector3 LocalPosition { get => _LocalPosition; }

	 [Tooltip("Поворот в руке")]
	 [SerializeField]
	 private Vector3 _LocalRotation = Vector3.zero;
	 /// <summary>
	 /// Поворот в руке
	 /// </summary>
	 public Vector3 LocalRotation { get => _LocalRotation; }

	 [SerializeField]
	 [Tooltip("Размер в руке")]
	 private Vector3 _LocalScale = Vector3.one;
	 /// <summary>
	 /// Размер в руке
	 /// </summary>
	 public Vector3 LocalScale { get => _LocalScale; }

	 public bool IsInteractReady
	 {
		get
		{
		  IUsableParent up = GetComponent<IUsableParent>();
		  if (up == null)
			 return true;
		  else
			 return up.IsUseReady;
		}

	 }

	 /// <summary>
	 /// Родитель, до начала удерживания
	 /// </summary>
	 private Transform _BeforeParent;

	 private IHoldParent _holdParent;

	 protected void Awake()
	 {
		_holdParent = GetComponent<IHoldParent>();
		if (_holdParent == null)
		{
		  Debug.LogError("No Exists IHoldParent interface in " + gameObject.name + " object");
		}

	 }

	 private void Start() { }

	 public void StartHold()
	 {
		_holdParent.BeforeStartHold();
		_BeforeParent = transform.parent;

		//PlayerBehaviour.Instance.PlayerHoldItem.StartUse(this);

		_holdParent.AfterStartHold();
	 }

	 public void StopHold()
	 {

		_holdParent.BeforeEndHold();
		transform.SetParent(_BeforeParent);

		//PlayerBehaviour.Instance.PlayerHoldItem.StopUse(this);

		_holdParent.AfterEndHold();
	 }

	 public void StartInteract()
	 {
		StartHold();
	 }

	 public void StopInteract()
	 {
		StopHold();
	 }
  }

}