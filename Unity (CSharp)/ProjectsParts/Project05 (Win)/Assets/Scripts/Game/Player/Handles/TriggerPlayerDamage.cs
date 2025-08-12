using UnityEngine;
using System.Collections;

namespace it.Game.Player.Handlers
{
  public class TriggerPlayerDamage : MonoBehaviourBase, IPlayerTriggerEnter
  {
	 [Tooltip("Размер наносимого урона")]
	 [SerializeField]
	 private float _value = 34;

	 [Tooltip("Игнорировать временной промежуток между уроном")]
	 [SerializeField]
	 private bool _ignoreGracePerion = false;

	 //private void OnTriggerEnter(Collider other)
	 //{
	 //CheckDamage(other);
	 //}

	 //private void OnTriggerStay(Collider other)
	 //{
	 //CheckDamage(other);
	 //}

	 private void Start()
	 {
		
	 }

	 protected void CheckDamage(Collider collider)
	 {
		if (collider.GetComponent<Player.IPlayer>() != null)
		  return;

		Damage();
	 }

	 protected void Damage()
	 {
		Game.Managers.GameManager.Instance.UserManager.Health.Damage(this,_value, _ignoreGracePerion);
	 }

	 public void OnPlayerTriggerEnter()
	 {
		Damage();
	 }

	 public void OnPlayerTriggerExit()
	 {
		return;
	 }
  }
}