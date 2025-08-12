using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace it.Game.Environment.Level2
{
  public class DogotShield : Environment
  {

	 [SerializeField]
	 private Renderer _shield;
	 [SerializeField]
	 private it.Game.NPC.Enemyes.Enemy _dagot;

	 /// <summary>
	 /// Игрок вошел в триггер
	 /// </summary>
	 public void PlayerEnterTrigger()
	 {
		if (State != 0)
		  return;
		_dagot.GetComponent<PlayMakerFSM>().SendEvent("OnPlayerVisible");

		State = 1;
		Save();
		//TODO запускаем откртие щита
	 }

	 protected override void ConfirmState(bool isForce = false)
	 {
		base.ConfirmState(isForce);

		if (isForce)
		{
		  if(State == 1)
		  {
			 _shield.gameObject.SetActive(false);
		  }
		  else
		  {
			 _shield.material.SetFloat("_Displace", 1);
			 _shield.gameObject.SetActive(true);
		  }
		}

	 }

	 public void OpenShield()
	 {
		if (State != 1)
		  return;

		DOTween.To(() => _shield.material.GetFloat("_Displace"), (x) => _shield.material.SetFloat("_Displace", x), 0, 1).OnComplete(()=> {
		  _shield.gameObject.SetActive(false);
		  _dagot.GetComponent<PlayMakerFSM>().SendEvent("OnShieldOpen");
		});

		State = 2;
		Save();

	 }

  }
}