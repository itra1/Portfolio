using UnityEngine;
using System.Collections;
using DG.Tweening;
using Leguar.TotalJSON;

namespace it.Game.Environment.Level6.Numbers
{
  /// <summary>
  /// пазл чисел
  /// </summary>
  public class NumbersPuzzle : Environment
  {
	 private Way[] _ways;

	 [SerializeField]
	 private Transform _column;
	 [SerializeField]
	 private ParticleSystem _columnParticles;
	 private Vector3 _startPosition;
	 //9.9 -3.33
	 private int _completeCount = 0;

	 [SerializeField]
	 private PlayMakerFSM _pegasus;

	 protected override void Awake()
	 {
		base.Awake();
		_ways = GetComponentsInChildren<Way>();

		for (int i = 0; i < _ways.Length; i++)
		  _ways[i].WhellPuzzle.OnComplete.AddListener(WhellPuzzleComplete);

		_completeCount = 0;
		_startPosition = _column.transform.position;
	 }
	 private void WhellPuzzleComplete()
	 {
		_completeCount++;
		_pegasus.SendEvent("StartFSM");

		if(_completeCount >= _ways.Length)
		{
		  State = 2;
		  Save();
		}

	 }

	 public void SetReset()
	 {
		_completeCount = 0;
		MoveColumnDown();
	 }

	 protected override JValue SaveData()
	 {
		JSON save = new JSON();
		save.Add("completes", _completeCount);
		return save;

	 }

	 public void PefasusComplete()
	 {
		MoveColumnDown();
	 }

	 protected override void LoadData(JValue data)
	 {
		base.LoadData(data);

		JSON load = (JSON)data;

		_completeCount = load.GetInt("completes");

	 }


	 [ContextMenu("Move column")]
	 public void MoveColumnDown()
	 {
		_column.DOMove(_startPosition + Vector3.down * _completeCount*4.4f, 6f).OnStart(()=> {
		  _columnParticles.Play();
		}).OnComplete(()=> {
		  _columnParticles.Play();
		  _pegasus.SendEvent("OnComplete");
		});
	 }


  }
}