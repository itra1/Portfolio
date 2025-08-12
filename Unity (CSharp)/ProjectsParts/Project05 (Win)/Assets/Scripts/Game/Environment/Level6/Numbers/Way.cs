using UnityEngine;
using System.Collections;
using it.Game.Environment.Challenges.WheelPuzzle;

namespace it.Game.Environment.Level6.Numbers
{
  public class Way : MonoBehaviour
  {
	 [SerializeField]
	 private Way _opponent;

	 public System.Action<Way> OnRollWheel;
	 
	 [SerializeField]
	 private Platform[] _platforms;
	 private int _indexPlatform;

	 private it.Game.Environment.Challenges.WheelPuzzle.WheelPuzzle _whellPuzzle;
	 public WheelPuzzle WhellPuzzle { 
		get {
		  if (_whellPuzzle == null)
			 _whellPuzzle = GetComponentInChildren<it.Game.Environment.Challenges.WheelPuzzle.WheelPuzzle>();
		  return _whellPuzzle;

		}
		set => _whellPuzzle = value; 
	 }


	 private void Awake()
	 {
		_indexPlatform = -1;

		_opponent.WhellPuzzle.OnRoll.AddListener(OpponentRoll);

	 }

	 private void OpponentRoll()
	 {
		_indexPlatform++;

		if (_indexPlatform >= _platforms.Length)
		  return;

		_platforms[_indexPlatform].SetHidePlatform(true);
	 }

	 public void SetReset()
	 {
		for(int i = 0; i < _platforms.Length; i++)
		{
		  _platforms[i].SetHidePlatform(false);
		}
	 }


  }
}