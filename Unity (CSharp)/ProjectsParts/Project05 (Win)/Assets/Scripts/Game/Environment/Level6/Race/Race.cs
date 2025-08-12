using UnityEngine;
using System.Collections;

namespace it.Game.Environment.Level6.Race
{
  public class Race : Environment
  {
	 [SerializeField]
	 private float _raceTime = 50;
	 [SerializeField]
	 private BowlData[] _bowls;
	 private bool _isActive;

	 [System.Serializable]
	 public struct BowlData
	 {
		public Bowl bowl1;
		public Bowl bowl2;
	 }

	 protected override void Start()
	 {
		base.Awake();
		Clear();
	 }

	 public void Activate()
	 {
		if (_isActive)
		  return;

		StartCoroutine(ActivateBowls());
		_isActive = true;
		StartCoroutine(DeactiveBowls());
	 }

	 private IEnumerator ActivateBowls()
	 {

		for (int i = 0; i < _bowls.Length; i++)
		{
		  _bowls[i].bowl1.Activate(true);
		  _bowls[i].bowl2.Activate(true);
		  yield return new WaitForSeconds(0.1f);
		}
	 }

	 private void Deactive()
	 {
		_isActive = false;
	 }

	 private IEnumerator DeactiveBowls()
	 {

		for (int i = 0; i < _bowls.Length; i++)
		{
		  yield return new WaitForSeconds(_raceTime / _bowls.Length);
		  _bowls[i].bowl1.Activate(false);
		  _bowls[i].bowl2.Activate(false);
		}
		Deactive();
	 }

	 private void Clear()
	 {
		for(int i = 0; i < _bowls.Length; i++)
		{
		  _bowls[i].bowl1.Activate(false,true);
		  _bowls[i].bowl2.Activate(false,true);
		}
	 }

  }
}