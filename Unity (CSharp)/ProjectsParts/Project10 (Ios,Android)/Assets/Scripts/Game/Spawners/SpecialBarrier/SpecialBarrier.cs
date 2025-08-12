using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SpecialBarrier : MonoBehaviour {

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

	private void Update() {

		if (RunnerController.Instance.runnerPhase == RunnerPhase.run)
			UpdateProcess();
	}

	protected virtual void Init(RunSpawner runSpawner){}

	protected virtual void UpdateProcess() {}

	protected abstract void GetConfig();

}
