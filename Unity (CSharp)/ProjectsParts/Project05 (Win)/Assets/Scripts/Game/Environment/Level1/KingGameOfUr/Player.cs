using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Challenges.KingGameOfUr
{
  [System.Serializable]
  public abstract class Player
  {

	 protected System.Action _OnStart;
	 protected System.Action _OnCompleteMove;
	 protected System.Action<string, bool> _OnMoveToNextSection;
	 [SerializeField]
	 protected Chip _Chip;

	 public Chip Chip { get => _Chip; set => _Chip = value; }

	 [SerializeField]
	 protected Section _StartSection;
	 private Section _ActualSection;
	 public Section ActualSection
	 {
		get
		{
		  return _ActualSection;
		}
	 }

	 public bool LastMove { get; protected set; }

	 [SerializeField]
	 protected List<Section> _Path;
	 [HideInInspector]
	 public int _StepIndex;
	 protected KingGameOfUr _manager;
	 protected abstract Vector3 StartChipPosition { get; }
	 protected abstract Color ChipColor { get; }

	 public virtual void Initiate(KingGameOfUr manager)
	 {
		_manager = manager;
		_StepIndex = -1;
		_ActualSection = null;
		_Chip.Init(manager);
		_Chip.Move(new List<Section>() { _StartSection }, null, null);
	 }

	 public virtual void Step(System.Action start, System.Action moveComplete, System.Action<string, bool> onNextSection)
	 {
		LastMove = false;
		_OnStart = start;
		_OnCompleteMove = moveComplete;
		_OnMoveToNextSection = onNextSection;
	 }

	 protected List<Section> GetPathForward(int steps)
	 {
		List<Section> path = new List<Section>();
		for (int i = 0; i < steps; i++)
		{
		  path.Add(_Path[_StepIndex + i + 1]);
		}
		_StepIndex += steps;
		_ActualSection = _Path[_StepIndex];
		return path;
	 }
	 protected List<Section> GetPathBack()
	 {
		List<Section> path = new List<Section>();
		for (int i = 0; i < 2; i++)
		{
		  path.Add(_Path[_StepIndex - i - 1]);
		}
		_StepIndex -= 2;
		_ActualSection = _Path[_StepIndex];
		return path;
	 }


	 public bool IsSuperSection
	 {
		get { return _ActualSection != null && _ActualSection.Super; }
	 }

	 public bool CheckInOne(string uuid)
	 {
		if (_ActualSection == null)
		  return false;

		if (!uuid.Equals(_ActualSection.Uuid))
		  return false;
		return true;
	 }

	 public bool CheckEqualsSection(System.Action onComplete, Section section)
	 {

		bool inOne = CheckInOne(section.Uuid);

		if (!inOne)
		  return false;

		List<Section> backPath = GetPathBack();

		//_Chip.Move(backPath, onComplete, null);
		GoBack(backPath[backPath.Count - 1], onComplete);
		return true;

	 }

	 private void GoBack(Section target, System.Action onComplete)
	 {
		_Chip.transform.DOScale(Vector3.zero, 0.5f).OnComplete(() =>
		{
		  _Chip.SetPosition(target);
		  _Chip.transform.DOScale(Vector3.one, 0.5f).OnComplete(() =>
		  {
			 onComplete?.Invoke();
		  });
		});
	 }

	 protected bool CheckOutPath(int steps)
	 {
		return _StepIndex + steps >= _Path.Count;
	 }

	 public bool IsComplete()
	 {
		return _StepIndex == _Path.Count - 1;
	 }

  }

}