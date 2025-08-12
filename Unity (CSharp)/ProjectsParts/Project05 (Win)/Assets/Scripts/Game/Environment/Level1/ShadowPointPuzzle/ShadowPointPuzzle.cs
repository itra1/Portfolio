using System.Collections.Generic;
using it.Game.Handles;
using UnityEngine;
using UnityEngine.Events;
using it.Game.Items;

namespace it.Game.Environment.Challenges.ShadowPointPuzzle
{
  [RequireComponent(typeof(Game.Handles.CheckToPlayerDistance))]
  public class ShadowPointPuzzle : Challenge
  {
	 /*
	  * Состояния:
	  * 0 - не дотупно к использованию
	  * 1 - готово к использованию
	  * 2 - выполнено
	  * 
	  */

	 [SerializeField]
	 private Transform _cameraPoint;

	 [SerializeField]
	 private Line _line;

	 [SerializeField]
	 private Point[] _points;

	 private List<Point> _selectPoints = new List<Point>();

	 [SerializeField]
	 private int[] _winCombination;
	 

	 private MouseHandler _mouse0Handle;

	 public override bool IsInteractReady
	 {
		get
		{
		  return State == 1;
		}
	 }

	 protected override void Awake()
	 {
		base.Awake();
		CheckToPlayerDistance cmp = GetComponent<CheckToPlayerDistance>();
		cmp.onPlayerInDistance = PlayerInDistance;
		cmp.onPlayerOutDistance = PlayerOutDistance;
		DeInitialization();
	 }

	 protected override void Start()
	 {
		base.Start();
		PlayerOutDistance();
		DeInitialization();
		if(State == 0);
		  VisibleButtons(false);
	 }

	 [ContextMenu("Use")]
	 public override void StartInteract()
	 {
		Initialization();
		SetActivate();
		//Game.Managers.GameManager.Instance.CameraFixPosition(_cameraPoint, false, true);
		FocusCamera(_cameraPoint, 0.5f);

		PlayerInDistance();
	 }

	 [ContextMenu("UnUse")]
	 public override void StopInteract()
	 {
		DeInitialization();
		UnFocusCamera(0.5f);
		//Game.Managers.GameManager.Instance.CameraPlayerFollow(true);
		if (IsComplete)
		  PlayerOutDistance();
	 }

	 public override void SetActivate()
	 {
		if (IsActive)
		  return;
		base.SetActivate();
		State = 1;
		ConfirmState();
		Save();
	 }

	 private void VisibleButtons(bool visible)
	 {
		foreach (var elem in _points)
		  elem.gameObject.SetActive(visible);
	 }
	 
	 private void OnMouseDownHandle()
	 {
		if (IsComplete)
		  return;
	 }

	 private void OnMouseUpHandle()
	 {

		if (IsComplete)
		  return;

		if (!CheckCombination())
		{
		  _line.Clear();
		  _selectPoints.ForEach(x => x.Selected(false));
		  _selectPoints.Clear();
		}
		else
		{
		  Complete();
		}
	 }

	 [ContextMenu("Complete")]
	 protected override void Complete()
	 {
		State = 1;
		_line.RemoveLast();
		StopInteract();
		base.Complete();
		Save();
	 }

	 private void OnMouseHoldHandle()
	 {

		if (IsComplete)
		  return;

		Point point;
		Vector3 backHit;

		GetTargetMousePoint(out point, out backHit);

		if (backHit != Vector3.zero)
		{
		  _line.UpdateLastPoint(backHit);
		}

		if (point != null && !_selectPoints.Contains(point) && (_selectPoints.Count == 0 || CheckDistance(_selectPoints[_selectPoints.Count - 1], point)))
		{
		  point.Selected(true);
		  _selectPoints.Add(point);
		  _line.AppPoint(point.transform.position);
		}
	 }

	 private bool CheckDistance(Point start, Point end)
	 {
		return Mathf.Abs(start.Coordinate.x - end.Coordinate.x) <= 1 &&
				 Mathf.Abs(start.Coordinate.y - end.Coordinate.y) <= 1;
	 }

	 private void GetTargetMousePoint(out Point point, out Vector3 backHit)
	 {
		point = null;
		backHit = Vector3.zero;

		Ray ray = CameraBehaviour.Instance.Camera.ScreenPointToRay(Input.mousePosition);
		RaycastHit[] hits = Physics.RaycastAll(ray, 10);

		Collider baseCollider = GetComponent<Collider>();

		foreach (var hit in hits)
		{
		  if (hit.collider.GetComponent<Point>())
			 point = hit.collider.GetComponent<Point>();
		  if (hit.collider.Equals(baseCollider))
		  {
			 backHit = hit.point;
		  }
		}

	 }

	 private bool CheckCombination()
	 {
		if (_selectPoints.Count != _winCombination.Length)
		  return false;

		if (_selectPoints[0].Index != _winCombination[0]
		 && _selectPoints[0].Index != _winCombination[_winCombination.Length - 1])
		  return false;

		bool forward = _selectPoints[0].Index == _winCombination[0];

		for (int i = 0; i < _selectPoints.Count; i++)
		{
		  if (_selectPoints[i].Index != _winCombination[forward ? i : _winCombination.Length - 1 - i])
			 return false;


		}

		return true;

	 }


	 /// <summary>
	 /// Деинициализация
	 /// </summary>
	 protected override void DeInitialization()
	 {
		base.DeInitialization();
		EnablePonts(false);
	 }

	 protected override void Initialization()
	 {
		base.Initialization();
		//todo Запустить движение камеры к точке
		_line.Initiate();
		_selectPoints.ForEach(x => x.Selected(false));
		EnablePonts(true);

		_mouseHandler.onMouseDown = OnMouseDownHandle;
		_mouseHandler.onMouseUp = OnMouseUpHandle;
		_mouseHandler.onMouseHold = OnMouseHoldHandle;
	 }

	 private void EnablePonts(bool isInite)
	 {
		foreach (var point in _points)
		{
		  point.enabled = isInite;
		}
	 }

	 private void OnDrawGizmosSelected()
	 {

		if (_winCombination.Length <= 0)
		  return;

		for (int i = 0; i < _winCombination.Length; i++)
		{
		  if (i == 0)
			 continue;

		  Gizmos.DrawLine(FindPoint(_winCombination[i - 1]).transform.position, FindPoint(_winCombination[i]).transform.position);
		}

	 }
	 
	 private Point FindPoint(int index)
	 {
		foreach (var point in _points)
		{
		  if (point.Index == index)
			 return point;
		}

		return null;
	 }

	 public void PlayerInDistance()
	 {
		GetComponent<BoxCollider>().enabled = true;
	 }
	 public void PlayerOutDistance()
	 {
		if (State == 1)
		{
		  StopInteract();
		}
		GetComponent<BoxCollider>().enabled = false;
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		VisibleButtons(State > 0);

		if(State >= 2 && isForce)
		  ForceComplete();


		if (State <= 1)
		  _selectPoints.ForEach(x => x.Selected(false));

	 }

	 private void ForceComplete()
	 {
		foreach (var elem in _winCombination)
		{
		  var p = FindPoint(elem);
		  p.Selected(true);
		}
	 }
  }
}