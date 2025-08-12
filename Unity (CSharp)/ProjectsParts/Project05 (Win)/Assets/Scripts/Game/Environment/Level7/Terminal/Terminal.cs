using it.Game.Handles;
using UnityEngine;
using Leguar.TotalJSON;
using it.Game.Environment.Handlers;
using DG.Tweening;

namespace it.Game.Environment.Challenges.Level7.Terminal
{
  public class Terminal : Challenge
  {
	 /*
	  * Состояния
	  * 0-готов к использованию
	  * 1- выполнен
	  * 
	  */

	 [SerializeField]
	 private Transform _cameraPoint;
	 /// <summary>
	 /// Выйграшная комбинация
	 /// </summary>
	 private int[] _winCombination = new int[] { 6, 8, 2 };
	 /// <summary>
	 /// Текущие значения
	 /// </summary>
	 private int[] _curemtValue = new int[3];
	 private MouseHandler _mouse0Handle;
	 private TerminalItem[] _items;

	 public override bool IsInteractReady => State == 0;


	 protected override void Start()
	 {
		base.Start();


		SetReadyItems(false);

	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		if (isForce)
		{
		  SetValuesItems();
		}
	 }

	 [ContextMenu("Use")]
	 public override void StartInteract()
	 {
		Initialization();
		SetActivate();

		FocusCamera(_cameraPoint, 0.5f);
		CameraBehaviour.Instance.HidePlayer(true);

	 }

	 [ContextMenu("UnUse")]
	 public override void StopInteract()
	 {
		CameraBehaviour.Instance.HidePlayer(false);
		DeInitialization();
		UnFocusCamera(0.5f);
	 }

	 protected override void Initialization()
	 {
		base.Initialization();
		//todo Запустить движение камеры к точке
		SetReadyItems(true);


		_mouse0Handle = GetComponent<MouseHandler>();
		if (_mouse0Handle == null)
		  _mouse0Handle = gameObject.AddComponent<MouseHandler>();
		_mouse0Handle.onMouseDown = OnMouseDownHandle;
	 }

	 protected override void DeInitialization()
	 {
		base.DeInitialization();
		SetReadyItems(false);
		if (_mouse0Handle != null)
		  Destroy(_mouse0Handle);
	 }

	 private void OnMouseDownHandle()
	 {
		TerminalItem point;

		GetTargetMousePoint(out point);

		if (point == null)
		  return;

		int index = point.Index;

		_curemtValue[index]++;
		if (_curemtValue[index] >= 10)
		  _curemtValue[index] = 0;
		point.SetValue(_curemtValue[index]);
		if (CheckFullCombination())
		{
		  Complete();
		}
	 }

	 protected override void Complete()
	 {
		StopInteract();

		PegasusController pegasus = GetComponentInChildren<PegasusController>();
		if (pegasus != null)
		{
		  pegasus.Activate(() =>
		  {
			 DOVirtual.DelayedCall(1f, () =>
			 {
				base.Complete();
			 });

			 DOVirtual.DelayedCall(2f, () =>
			 {
				pegasus.Deactivate();
			 });

		  });
		}
		else
		{
		  base.Complete();
		}
	 }

	 private void GetTargetMousePoint(out TerminalItem point)
	 {
		point = null;

		Ray ray = CameraBehaviour.Instance.Camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, 10);

		foreach (var hit in hits)
		{
		  if (hit.collider.GetComponent<TerminalItem>())
			 point = hit.collider.GetComponent<TerminalItem>();
		}

	 }

	 private bool CheckFullCombination()
	 {
		for(int i = 0; i < _curemtValue.Length; i++)
		{
		  if (_curemtValue[i] != _winCombination[i])
			 return false;
		}
		return true;
	 }

	 private void SetValuesItems()
	 {

		if (_items == null)
		  _items = GetComponentsInChildren<TerminalItem>();
		for (int i = 0; i < _items.Length; i++)
		{
		  _items[i].SetValue(_curemtValue[i]);
		}
	 }

	 private void SetReadyItems(bool setActive = true)
	 {

		if (_items == null)
		  _items = GetComponentsInChildren<TerminalItem>(); 
		
		for (int i = 0; i < _items.Length; i++)
		{
		  _items[i].SetReady(setActive);
		}
	 }

	 #region Save

	 protected override JValue SaveData()
	 {
		JArray arr = new JArray();
		for (int i = 0; i < _curemtValue.Length; i++)
		{
		  arr.Add(_curemtValue[i]);
		}

		return arr;
	 }
	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);

		JArray arr = data as JArray;
		for (int i = 0; i < arr.Length; i++)
		{
		  Debug.Log(arr[i].ToString());
		  _curemtValue[i] = (arr[i] as JNumber).AsInt();
		}

	 }

	 #endregion

  }
}