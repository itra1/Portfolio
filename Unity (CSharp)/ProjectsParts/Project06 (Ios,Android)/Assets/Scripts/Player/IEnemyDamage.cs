using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public interface IEnemyDamage : IDamage {
	bool Damage(GameObject damager, float value, float newSpeedDelay = 0, float newTimeSpeedDelay = 0, Component parent = null);
	bool IsMelafon { get; }

	bool playerContact { get; }
}
