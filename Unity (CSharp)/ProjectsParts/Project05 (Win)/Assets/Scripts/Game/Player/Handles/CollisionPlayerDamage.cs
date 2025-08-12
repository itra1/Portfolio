using UnityEngine;
using System.Collections;

namespace it.Game.Player.Handlers
{
  /// <summary>
  /// Нанисение урона игроку при касании
  /// </summary>
  public class CollisionPlayerDamage : MonoBehaviourBase
  {
	 [Tooltip("Размер наносимого урона")]
	 [SerializeField]
	 private float _value = 34;


	 [Tooltip("Игнорировать временной промежуток между уроном")]
	 [SerializeField]
	 private bool _ignoreGracePerion = false;
	 public void OnCollisionEnter(Collision collision)
	 {
		CheckDamage(collision.collider);
	 }

	 public void OnCollisionStay(Collision collision)
	 {
		CheckDamage(collision.collider);
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

  }
}