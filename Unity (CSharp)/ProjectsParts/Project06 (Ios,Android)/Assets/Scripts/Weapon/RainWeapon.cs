using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Weapon
{
  
public class RainWeapon : Bullet {

	public override void Shot(Vector3 tapStart, Vector3 tapEnd) { }
	public override void Move() { }
	public override void OnGround() { }

	public override void OnTriggerEnter2D(Collider2D col) {}

	private void OnTriggerStay2D(Collider2D collision) {
		if (LayerMask.LayerToName(collision.gameObject.layer) == "Enemy") {
			collision.GetComponent<Enemy>().SetRain();
		}
	}



	//public override void On (Collider2D col) {

	//	if (LayerMask.LayerToName(col.gameObject.layer) == "Enemy") {
	//		DamageEnemy(col.gameObject);
	//		//DeactiveThis();
	//	}
	//}

}


}