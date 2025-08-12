using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Интерфейс контакта с играком
/// </summary>
public interface IOnPlayerTrigger {

	void OnTriggerPlayer(Player.Jack.PlayerController player);
}
