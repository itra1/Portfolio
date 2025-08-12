using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

	public GameObject damageEffect;
	float helth = 30f;

	void Start () {
		damageEffect.SetActive(false);

	}
	
	void Update () {
		
	}

	private void OnDestroy() {
	}

	void Damage(Enemy enemy) {
		helth -= 5;

		if (helth <1) {


		}


		damageEffect.SetActive(true);
		Invoke("DamageEffectOff", 0.2f);

	}



	void DamageEffectOff() {

		damageEffect.SetActive(false);
	}

}
