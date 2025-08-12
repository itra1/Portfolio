using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace it.Game.Environment.Handlers
{
  /// <summary>
  /// Отрисовка линии
  /// </summary>
  [ExecuteInEditMode]
  public class RayTripleDrawner : MonoBehaviourBase
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

	 private LineRenderer _mainLine = null;
	 private LineRenderer _subLine1 = null;
	 private LineRenderer _subLine2 = null;


	 /// <summary>
	 /// Draw
	 /// </summary>
	 private bool _isDraw = false;

	 private void Start(){ }

	 /// <summary>
	 /// Start Visual Line
	 /// </summary>
	 [ContextMenu("Start")]
	 public void StartVisualLine()
	 {
		_isDraw = true;
		_lastTime = Time.time;

		Transform line = transform.Find("MainLine");

		if (line != null)
		  _mainLine = line.GetComponent<LineRenderer>();

		if (_mainLine == null)
		  _mainLine = CreateInstance();
		_mainLine.gameObject.name = "MainLine";
		_mainLine.SetPosition(0, _startPosition.position);
		_mainLine.SetPosition(1, _endPosition.position);
		_mainLine.widthMultiplier = 1f;
		_mainLine.material.SetFloat("_FadeLevel", _fadeStart);
		_mainLine.material.SetFloat("_FrontFadeLevel", _fadeEnd);


		Transform line1 = transform.Find("SubLine1");

		if (line1 != null)
		  _subLine1 = line1.GetComponent<LineRenderer>();

		if (_subLine1 == null)
		  _subLine1 = CreateInstance();
		_subLine1.gameObject.name = "SubLine1";
		_subLine1.SetPosition(0, _startPosition.position);
		_subLine1.SetPosition(1, _endPosition.position);
		_subLine1.widthMultiplier = 0.15f;
		_subLine1.material.SetFloat("_FadeLevel", _fadeStart);
		_subLine1.material.SetFloat("_FrontFadeLevel", _fadeEnd);


		Transform line2 = transform.Find("SubLine2");

		if (line2 != null)
		  _subLine2 = line2.GetComponent<LineRenderer>();

		if (_subLine2 == null)
		  _subLine2 = CreateInstance();
		_subLine2.gameObject.name = "SubLine2";
		_subLine2.SetPosition(0, _startPosition.position);
		_subLine2.SetPosition(1, _endPosition.position);
		_subLine2.widthMultiplier = 0.15f;
		_subLine2.material.SetFloat("_FadeLevel", _fadeStart);
		_subLine2.material.SetFloat("_FrontFadeLevel", _fadeEnd);

	 }


	 private void Update()
	 {
		if (!_isDraw)
		  return;

		if(_startPosition == null || _endPosition == null)
		{
		  Game.Logger.LogWarning("No exists link to _startPoint or _endPoint draw line");
		}

		if (_lastTime > Time.time)
		  return;
		
		_lastTime = Time.time + (1 / _rate);

		DrawSections(_subLine1);
		DrawSections(_subLine2);

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
		lr.material = Instantiate(_material);
		lr.positionCount = 2;
		return lr;
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