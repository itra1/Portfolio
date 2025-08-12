using UnityEngine;
using System.Collections.Generic;
using ExEvent;
using System;

[System.Serializable]
public class EnemySpawnerShip : IEnemySpawner {

	private EnemySpawner enemySpawner;

	public List<EnemyTypes> enemyList;                // Список типов которые участвуют в генераци
	public List<float> timeGenerate;
	private RunnerPhase runPhase;
	private List<EnemyTypes> enemyExistsNames;              // Список сгенерированных врагов
	private float nextTimeGenerate;
	private int nextNum;
	private float timeStart;

	public void Init(EnemySpawner enemySpawner) {
		this.enemySpawner = enemySpawner;
		enemyExistsNames = new List<EnemyTypes>();
		nextTimeGenerate = -1;
		nextNum = 0;
	}

	public void DeInit() { }

	public void ChangePhase(RunnerPhase runPhase) {
		if(runPhase == RunnerPhase.run) {
			timeStart = Time.time;
		}
		this.runPhase = runPhase;
	}

	public void Update() {
		if(runPhase == RunnerPhase.run)
			RunGenerate();
	}

	void RunGenerate() {
		if(nextTimeGenerate >= 0 && nextTimeGenerate <= Time.time) {
			Generate();
			CalcNextGenerate();
		}
	}

	void CalcNextGenerate() {
		if(enemyExistsNames.Count == enemyList.Count || nextNum == enemyList.Count - 1) {
			nextTimeGenerate = -1;
			return;
		}

		nextTimeGenerate = timeGenerate[nextNum] + timeStart;
		nextNum++;
	}

	public void DeadCloud() {
		//if (!deadCloudYes) {
		//	deadCloudInstance = MonoBehaviour.Instantiate(deadCloud, Vector3.zero, Quaternion.identity) as GameObject;
		//	deadCloudYes = true;
		//}
	}

	public void OnSpecialPlatform(ExEvent.RunEvents.SpecialPlatform specialPlatform) {}

	void Generate() {
		int needNum;

		do {
			needNum = UnityEngine.Random.Range(0, enemyList.Count);
		} while(enemyExistsNames.Exists(x => x == enemyList[needNum]));

		enemyExistsNames.Add(enemyList[needNum]);

		Vector3 newPosition = new Vector3(CameraController.displayDiff.leftDif(1.3f), RunnerController.Instance.mapHeight + .5f, 0);

		GameObject obj = enemySpawner.GetInstantEnemy(enemyList[needNum]);
		obj.transform.position = newPosition;
		obj.GetComponent<GeneralEnemy>().ChangePhase(runPhase);
		obj.SetActive(true);

		if(enemyList[needNum] == EnemyTypes.whoreGemini) {
			GameObject obj1 = enemySpawner.GetInstantEnemy(enemyList[needNum]);
			obj1.transform.position = newPosition;
			obj.transform.parent = enemySpawner.transform;
			obj.GetComponent<GeneralEnemy>().ChangePhase(runPhase);
			obj.SetActive(true);
		}
	}

	public void GetConfig() { }

	public void OnSpecialBarrier(RunEvents.SpecialBarrier specialBarrier) {
		throw new NotImplementedException();
	}
}
