using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Handlers
{
  [ExecuteInEditMode]
  public class Lighting : MonoBehaviourBase
  {

	 [SerializeField]
	 private Transform _startPosition;
	 [SerializeField]
	 private Transform _endPosition;
	 [SerializeField]
	 private Material _material;

	 [SerializeField]
	 private float _rate = 15f;
	 private float _lastTime = 0;

	 [SerializeField]
	 private float _minSubSection = 0.3f;

	 [SerializeField]
	 private float _maxSubSection = 0.6f;

	 [SerializeField]
	 private float _minDifPoint = -0.1f;

	 [SerializeField]
	 private float _maxDifPoint = 0.1f;

	 [SerializeField]
	 private float _fadeStart = 0.3f;
	 [SerializeField]
	 private float _fadeEnd = 0.3f;

	 [SerializeField]
	 private float _widthLine = 0.15f;

	 private LineRenderer _line = null;

	 private bool _isDraw = false;

	 private void Start() { }


	 /// <summary>
	 /// Start Visual Line
	 /// </summary>
	 [ContextMenu("Start")]
	 public void StartVisualLine()
	 {
		_isDraw = true;
		_lastTime = Time.time;

		Transform line = transform.Find("LineRenderer");

		if (line != null)
		  _line = line.GetComponent<LineRenderer>();

		if (_line == null)
		  _line = CreateInstance();
		_line.gameObject.name = "LineRenderer";
		_line.SetPosition(0, _startPosition.position);
		_line.SetPosition(1, _endPosition.position);
		_line.widthMultiplier = _widthLine;
		_line.material.SetFloat("_FadeLevel", _fadeStart);
		_line.material.SetFloat("_FrontFadeLevel", _fadeEnd);
		
	 }

	 public void Stop()
	 {
		_line.positionCount = 0;
		_isDraw = false;
	 }

	 /// <summary>
	 /// Create one line instance
	 /// </summary>
	 /// <returns></returns>
	 private LineRenderer CreateInstance()
	 {

		GameObject inst = new GameObject();
		inst.transform.SetParent(transform);
		inst.transform.localPosition = Vector3.zero;

		LineRenderer lr = inst.AddComponent<LineRenderer>();
		//lr.material = Instantiate(_material);
		lr.material = _material;
		lr.positionCount = 2;
		return lr;
	 }

	 private void Update()
	 {
		if (!_isDraw)
		  return;

		if (_startPosition == null || _endPosition == null)
		{
		  Game.Logger.LogWarning("Exists not link to _startPoint or _endPoint draw line");
		  _isDraw = false;
		  return;
		}
		if (_startPosition.Equals(_endPosition))
		{
		  Game.Logger.LogWarning("Correct not link to _startPoint or _endPoint draw line");
		  _isDraw = false;
		  return;
		}
		if (_line == null)
		{
		  Game.Logger.LogWarning("Exists not Line");
		  _isDraw = false;
		  return;
		}

		if (_lastTime > Time.time)
		  return;

		_lastTime = Time.time + (1 / _rate);

		DrawSections(_line);

	 }


	 /// <summary>
	 /// Draw subline
	 /// </summary>
	 /// <param name="subLine"></param>
	 private void DrawSections(LineRenderer subLine)
	 {
		if (subLine == null)
		  return;

		List<Vector3> points = new List<Vector3>();
		Vector3 direction = (_endPosition.position - _startPosition.position).normalized;

		Vector3 lastPosition = _startPosition.position;
		points.Add(lastPosition);
		bool end = false;

		while (!end)
		{
		  Vector3 newPosition = lastPosition + direction * Random.Range(_minSubSection, _maxSubSection);

		  if ((_endPosition.position - lastPosition).sqrMagnitude < (newPosition - lastPosition).sqrMagnitude)
		  {

			 lastPosition = _endPosition.position;
			 points.Add(lastPosition);
			 end = true;
		  }
		  else
		  {
			 lastPosition = newPosition;
			 points.Add(lastPosition + new Vector3(Random.Range(_minDifPoint, _maxDifPoint), Random.Range(_minDifPoint, _maxDifPoint), Random.Range(_minDifPoint, _maxDifPoint)));
		  }

		}

		subLine.positionCount = points.Count;
		subLine.SetPositions(points.ToArray());

	 }

  }
}