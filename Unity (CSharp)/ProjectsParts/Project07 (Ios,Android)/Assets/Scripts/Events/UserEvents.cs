using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ExEvent {
	public class UserEvents {

		public sealed class UserDamage : BaseEvent {
			public float maxHealth;
			public float health;
			public float damage;

			public UserDamage(float maxHealth, float health, float damage) {
				this.maxHealth = maxHealth;
				this.health = health;
				this.damage = damage;
			}
			public static void Call(float maxHealth, float health, float damage) {
				BaseEvent.Call(new UserDamage(maxHealth, health, damage));
			}
			public static void CallAsync(float maxHealth, float health, float damage) {
				BaseEvent.CallAsync(new UserDamage(maxHealth, health, damage));
			}
		}

		public sealed class UserDied : BaseEvent {
			public float health;
			public float damage;
			public UserDied(float health) {
				this.health = health;
			}

			public static void Call(float health) {
				BaseEvent.Call(new UserDied(health));
			}

			public static void CallAsync(float health) {
				BaseEvent.CallAsync(new UserDied(health));
			}
		}

	public sealed class UserMedicineBonus : BaseEvent {
			public float health;
			public float maxHealth;
			
		

		public UserMedicineBonus(float health, float maxHealth) {
				
				this.health = health;
				this.maxHealth = maxHealth;

			}

		public static void Call(float health, float maxHealth) {
			BaseEvent.Call(new UserMedicineBonus(health, maxHealth));
		}

		public static void CallAsync( float health, float maxHealth) {
			BaseEvent.CallAsync(new UserMedicineBonus(health, maxHealth));
		}

	}


}
}