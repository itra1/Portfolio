using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.All
{
  /// <summary>
  /// Отрисовка линии
  /// </summary>
  public class LineDrawner : MonoBehaviourBase
  {
	 [SerializeField]
	 private Vector3[] _positions;

	 private float _speed;

	 private int _index = 0;

	 private List<Vector3> positionsList = new List<Vector3>();

	 private LineRenderer _lineRenderer;
	 private LineRenderer LineRenderer
	 {
		get
		{
		  if (_lineRenderer == null)
			 _lineRenderer = GetComponent<LineRenderer>();
		  return _lineRenderer;
		}
		set
		{
		  _lineRenderer = value;
		}
	 }
	 private System.Action _onComplete = null;

	 private bool _drawn = false;

	 private Vector3 actialPosition;

	 public void Clear()
	 {
		LineRenderer.positionCount = 0;
	 }

	 public void Drawn(Vector3[] positions, float speed, System.Action onComplete, bool force = false)
	 {
		if (force)
		{
		  LineRenderer.positionCount = positions.Length;
		  LineRenderer.SetPositions(positions);
		  this._onComplete = onComplete;
		  _onComplete?.Invoke();
		  return;
		}


		this._onComplete = onComplete;
		this._speed = speed;
		this._positions = positions;
		this.LineRenderer = GetComponent<LineRenderer>();

		positionsList.Clear();

		_index = 1;
		actialPosition = _positions[0];
		positionsList.Add(_positions[0]);
		positionsList.Add(_positions[0]);
		LineRenderer.positionCount = 2;
		LineRenderer.SetPositions(positionsList.ToArray());

		_drawn = true;

	 }

	 private void Update()
	 {
		if (!_drawn)
		  return;

		Vector3 nexPoint = _positions[_index];
		Vector3 direction = (_positions[_index] - actialPosition).normalized;

		Vector3 newPosition = actialPosition + direction * _speed * Time.deltaTime;

		if((nexPoint - actialPosition).sqrMagnitude < (newPosition - actialPosition).sqrMagnitude)
		{
		  positionsList[positionsList.Count - 1] = nexPoint;
		  actialPosition = nexPoint;
		  positionsList.Add(nexPoint);
		  _lineRenderer.positionCount = positionsList.Count;
		  _lineRenderer.SetPositions(positionsList.ToArray());
		  _index++;

		  if(_index >= _positions.Length)
		  {
			 // приезали
			 _drawn = false;
			 _onComplete?.Invoke();
			 return;
		  }

		}
		else
		{
		  actialPosition = newPosition;
		  _lineRenderer.SetPosition(_index, actialPosition);
		}

	 }


  }
}