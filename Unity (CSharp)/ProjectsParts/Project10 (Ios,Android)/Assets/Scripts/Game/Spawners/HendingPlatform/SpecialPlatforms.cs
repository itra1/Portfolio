using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialPlatforms : MonoBehaviour {

	public GameMode gameMode;
	public List<SpecialBarrierAttack> spawners;

	protected virtual void Awake() {

		if (gameMode != GameManager.activeLevelData.gameMode) {
			Destroy(gameObject);
			return;
		}
		GetConfig();
		Init(RunSpawner.Instance);
	}
	
	protected abstract void GetConfig();

	protected virtual void Init(RunSpawner runSpawner) { }
}
