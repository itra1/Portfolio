using UnityEngine;
using System.Collections;
using System;

public class HouseController : MonoBehaviour, IPlayerDamage {
	public bool DamageReady {
		get { return true; }
	}

	public static event System.Action<float> hitTarget;
	
	public void Damage(float damage) {
		if (hitTarget != null) hitTarget(damage);
	}
	
}
