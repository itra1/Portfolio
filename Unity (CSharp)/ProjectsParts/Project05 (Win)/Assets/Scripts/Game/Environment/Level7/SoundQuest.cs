using UnityEngine;
using DG.Tweening;

namespace it.Game.Environment.Challenges.Level7.SoundQuest
{
  public class SoundQuest : Challenge
  {
	 /*
    * Состояния:
    * 0 - стандартное
    * 1 - Открытая стена
    * 2 - подобран посох
    * 
    */
	 [SerializeField]
	 private Transform _wall;
	 [SerializeField]
	 private GameObject _posoh;
	 [SerializeField]
	 private GameObject _interactionWall;
	 public override bool IsInteractReady => State == 0;

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);
		if (isForce)
		{
		  _posoh.SetActive(State > 0);
		  _wall.gameObject.SetActive(State == 0);
		  _interactionWall.SetActive(State == 0);
		}
	 }

	 public void InteructUseWall()
	 {
		State = 1;
		Save();
		ConfirmState();
		_wall.DOLocalMove(new Vector3(0, -8, 0), 2f).OnComplete(()=> {
		  _wall.gameObject.SetActive(false);
		  _interactionWall.SetActive(false);
		});

	 }

	 public void AddStaff()
	 {
		State = 2;
		Save();
	 }

  }
}