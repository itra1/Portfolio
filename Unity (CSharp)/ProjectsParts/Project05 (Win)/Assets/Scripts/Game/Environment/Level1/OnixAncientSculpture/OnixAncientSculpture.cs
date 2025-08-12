using System.Collections;
using System.Collections.Generic;
using it.Game.Player.Save;
using UnityEngine;

namespace it.Game.Environment.Level1.OnixAncientSculpture
{
  /// <summary>
  /// Рогатая скульптура
  /// </summary>
  public class OnixAncientSculpture : Environment
  {
	 // Состояниея
	 // 0 - стартовое состояние
	 // 1 - установлен нож, активна игра
	 // 3 - активен лазер

	 [SerializeField]
	 private Vector3[] _positionsLine = new Vector3[0];

	 [SerializeField]
	 private LineRenderer _lineRenderer = null;

	 public bool IsInteractReady => true;

	 [SerializeField]
	 private RaysManager _rayManager = null;


	 [SerializeField]
	 private Game.Environment.Challenges.ShadowPointPuzzle.ShadowPointPuzzle _pazzleGame;

	 [SerializeField]
	 private UnityEngine.Events.UnityEvent _onActivateLaser;

	 protected override void Start()
	 {
		GetLineDrawner().Clear();
		base.Start();
	 }

	 [ContextMenu("Activate Knife")]
	 public void ActivateKnife()
	 {
		if (State != 0)
		  return;

		State = 1;
		ConfirmState();
		Save();
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		switch (State)
		{
		  case 1:
			 //PositionKnife();
			 DrawLine(ActivateShadowPazzle, isForce);
			 break;
		  case 2:
			 _rayManager.SetVisible(true);
			 DrawLine(ActivateShadowPazzle, isForce);
			 break;
		  default:
			 GetLineDrawner().Clear();
			 break;
		}

	 }

	 private void ActivateShadowPazzle()
	 {
		_pazzleGame.SetActivate();
	 }

	 /// <summary>
	 /// Get separation line
	 /// </summary>
	 /// <returns></returns>
	 private All.LineDrawner GetLineDrawner()
	 {
		var drawner = _lineRenderer.gameObject.AddComponent<All.LineDrawner>();

		if (drawner == null)
		{
		  drawner = _lineRenderer.gameObject.AddComponent<All.LineDrawner>();
		}
		return drawner;
	 }

	 [ContextMenu("Activate laser")]
	 public void ActivateLaser()
	 {
		State++;
		_rayManager.SetVisible();
		Save();
		_onActivateLaser?.Invoke();
	 }

	 private void DrawLine(UnityEngine.Events.UnityAction onComplete, bool force = false)
	 {
		var drawner = GetLineDrawner();

		drawner.Drawn(_positionsLine, 3, () =>
		{
		  Destroy(drawner);
		  onComplete?.Invoke();
		}, force);
	 }

	 #region Editor
#if UNITY_EDITOR
	 [ContextMenu("Copy positions")]
	 private void CopyPositions()
	 {
		_positionsLine = new Vector3[_lineRenderer.positionCount];

		for (int i = 0; i < _lineRenderer.positionCount; i++)
		{
		  _positionsLine[i] = _lineRenderer.GetPosition(i);
		}
	 }


	 [SerializeField]
	 private Transform[] _spline;

	 [ContextMenu("Copy positionsFromSpline")]
	 private void CopyPositionsToLoadLine()
	 {
		_lineRenderer.positionCount = _spline.Length;
		_positionsLine = new Vector3[_lineRenderer.positionCount];

		for (int i = 0; i < _spline.Length; i++)
		{
		  _lineRenderer.SetPosition(i, _spline[i].localPosition);
		  _positionsLine[i] = _spline[i].localPosition;
		}


	 }

#endif
	 #endregion

  }
}