using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using it.Game.Handles;
using DG.Tweening;

namespace it.Game.Environment.Level6.BarleyBreak
{
  public class BarleyBreak : Challenge
  {
	 [SerializeField]
	 private BarleyBreakDice[] _diceArr;
	 [SerializeField]
	 private BarleyBreakDice _dicePrefab;

	 [SerializeField]
	 private Transform _cameraFocus;

	 [SerializeField]
	 private Transform _diceParent;


	 [SerializeField]
	 private int _sizeX = 10;
	 [SerializeField]
	 private int _sizeY = 10;

	 [SerializeField]
	 private float _diceDistance = 0.1f;
	 private float _sizeRadius = 0.035f;

	 public override bool IsInteractReady => State < 2 ;

	 protected override void Start()
	 {
		base.Start();
		DeInitialization();
	 }

	 [ContextMenu("Inter")]
	 public override void StartInteract()
	 {
		Initialization();
		base.StartInteract();
		SetActivate();
	 }

	 public override void StopInteract()
	 {
		base.StopInteract();
		DeInitialization();
	 }

	 public override void SetActivate()
	 {
		base.SetActivate();
		ActivePices();

		FocusCamera(_cameraFocus, 0.5f);
		_mouseHandler.onMouseDown = () =>
		{
		  Ray ray = CameraBehaviour.Instance.Camera.ScreenPointToRay(Input.mousePosition);
		  RaycastHit[] hits = Physics.RaycastAll(ray, 20);

		  foreach (var hit in hits)
		  {
			 BarleyBreakDice dice = hit.collider.GetComponentInParent<BarleyBreakDice>();
			 if (dice != null)
			 {
				CheckClickPoint(dice);
				return;
			 }
		  }

		};
	 }

	 protected override void DeInitialization()
	 {
		base.DeInitialization();
		UnFocusCamera(0.5f);
		DeactivePices();
	 }

	 private void ActivePices()
	 {
		for (int i = 0; i < _diceArr.Length; i++)
		{
		  _diceArr[i].gameObject.SetActive(true);
		  _diceArr[i].SetActive();
		}
	 }

	 private void DeactivePices()
	 {
		for (int i = 0; i < _diceArr.Length; i++)
		{
		  var pice = _diceArr[i];
		  pice.SetDeactive();
		  DOVirtual.DelayedCall(1, () =>
		  {
			 pice.gameObject.SetActive(false);

		  });

		}
	 }

	 //protected override void Initiate()
	 //{
	 //FocusCamera(_cameraFocus, 0.5f);
	 //base.Initiate();
	 //var comp = GetComponent<MouseHandler>();
	 //if (comp == null)
	 //  comp = gameObject.AddComponent<MouseHandler>();
	 //comp.onMouseDown = () =>
	 //{
	 //  Ray ray = CameraBehaviour.Instance.Camera.ScreenPointToRay(Input.mousePosition);
	 //  RaycastHit[] hits = Physics.RaycastAll(ray, 20);

	 //  foreach (var hit in hits)
	 //  {
	 //	 BarleyBreakDice dice = hit.collider.GetComponentInParent<BarleyBreakDice>();
	 //	 if (dice != null)
	 //	 {
	 //		CheckClickPoint(dice);
	 //		return;
	 //	 }
	 //  }

	 //};
	 //}

	 public void CheckClickPoint(BarleyBreakDice positionClick)
	 {


		for (int x = 0; x < _sizeX; x++)
		{
		  for (int y = 0; y < _sizeY; y++)
		  {
			 if((_diceArr[x * _sizeY + y].Equals(positionClick)))
			 {
				ChangeDice(x, y);
				CheckComplete();
				return;
			 }
		  }
		}
	 }

	 public void ChangeDice(int x, int y)
	 {
		InverceDice(_diceArr[x * _sizeY + y]);

		if(x > 0)
		  InverceDice(_diceArr[(x-1) * _sizeY + y]);
		if (x < _sizeX-1)
		  InverceDice(_diceArr[(x + 1) * _sizeY + y]);
		if (y > 0)
		  InverceDice(_diceArr[x * _sizeY + (y-1)]);
		if (y < _sizeX - 1)
		  InverceDice(_diceArr[x * _sizeY + (y + 1)]);
	 }

	 private void CheckComplete()
	 {
		for (int x = 0; x < _sizeX; x++)
		{
		  for (int y = 0; y < _sizeY; y++)
		  {
			 if (!_diceArr[x * _sizeY + y].IsSelected)
			 {
				return;
			 }
		  }
		}

		Complete();
	 }
	 [ContextMenu("Complete")]
	 protected override void Complete()
	 {
		base.Complete();
		DeInitialization();
	 }


	 private void InverceDice(BarleyBreakDice dice)
	 {
		dice.Inverce();
	 }

	 //protected override void DeInitiate()
	 //{
		//base.DeInitiate();
		//UnFocusCamera(0.5f);
		//var comp = GetComponent<MouseHandler>();
		//if (comp != null)
		//  Destroy(comp);
	 //}

#if UNITY_EDITOR

	 [ContextMenu("Generate grid")]
	 private void GenerateGrid()
	 {
		if (_dicePrefab == null)
		  return;

		Clear();

		_dicePrefab.gameObject.SetActive(false);

		_diceArr = new BarleyBreakDice[_sizeX * _sizeY];

		for (int x = 0; x < _sizeX; x++)
		{
		  for (int y = 0; y < _sizeY; y++)
		  {
			 GameObject inst = Instantiate(_dicePrefab.gameObject, _diceParent);
			 inst.transform.localPosition = new Vector3(x * _diceDistance, -y * _diceDistance, 0);
			 _diceArr[x * _sizeY + y] = inst.GetComponent<BarleyBreakDice>();
			 inst.SetActive(true);
		  }
		}

	 }

	 private void Clear()
	 {
		for (int i = 0; i < _diceArr.Length; i++)
		{
			 if (_diceArr[i] != null)
			 {
				DestroyImmediate(_diceArr[i]);
			 }
		}
	 }

#endif

  }
}