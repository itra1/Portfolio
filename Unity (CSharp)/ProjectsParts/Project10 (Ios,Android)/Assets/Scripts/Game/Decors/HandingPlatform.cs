using System;
using UnityEngine;
using System.Collections.Generic;
using ExEvent;
using Random = UnityEngine.Random;

/// <summary>
/// Висячте платформы
/// </summary>
public class HandingPlatform : EventBehaviour {

	[HideInInspector]
	public int layerId;
	[SerializeField]
	GameObject prefabPlatform;                                 // Объект платформы
	[SerializeField]
	float platformWidth;
	Vector2 lastSpawnPoint;
	float breakWidth;

  private const string POOLERKEY = "HandingPlatform";


  delegate void ActiveProgramm(bool init = false);
	ActiveProgramm UseFunction;

	public delegate void GenerateHendingPlatform(bool isActive, int type);
	public static event GenerateHendingPlatform generateHendingPlatform;

	List<KeyValuePair<GameObject, int>> platformList;
	List<Transform> generateList;
	List<float> cd;
  
	int platformCount;
	int platformLenghtCount;
	int thisPlatformNum;
	int thisPlatformLenghtNum;
	float incrementHeight;

	bool isActive;
	bool readyEnd;


	void Start() {
		//RunSpawner.OnHandingPlatform += OnHandingPlatform;
		generateList = new List<Transform>();
		StructProcessor();
		LevelPooler.Instance.AddPool(POOLERKEY,platformList, generateList);
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		//RunSpawner.OnHandingPlatform -= OnHandingPlatform;
	}

	void Update() {
		if (!isActive) return;

		if (readyEnd) {
			if (timeToCheckDeactive <= Time.time) {
				timeToCheckDeactive = Time.time + 3;
				for (int i = 0; i < generateList.Count; i++) {
					if (generateList[i].gameObject.activeInHierarchy &&
							generateList[i].position.x < CameraController.displayDiff.leftDif(1.5f)) {
						generateList[i].gameObject.SetActive(false);
					}
				}
			}
			return;
		}

		UseFunction();
	}

	float timeToCheckDeactive;

	Action EndHndingPlatform;

	[ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.SpecialPlatform))]
	void OnHandingPlatform(ExEvent.RunEvents.SpecialPlatform specialPlatform) {

		if (!specialPlatform.isActivate) return;                     // Не соответствует тип
		EndHndingPlatform = specialPlatform.callback;

		isActive = true;

		if (generateHendingPlatform != null)
			generateHendingPlatform(true, specialPlatform.typ);

		switch (specialPlatform.typ) {
			case 1:
				UseFunction = Programm1;
				break;
			case 2:
				UseFunction = Programm2;
				break;
			case 3:
				UseFunction = Programm3;
				break;
			case 4:
				UseFunction = Programm4;
				break;
			case 5:
				UseFunction = Programm5;
				break;
		}

		UseFunction(true);
	}

	GameObject SpawnPlatform(Vector2 pos) {

		GameObject obj = LevelPooler.Instance.GetPooledObject(POOLERKEY, 0, generateList);
		obj.transform.position = new Vector3(pos.x, pos.y, transform.position.z);
		lastSpawnPoint = obj.transform.position;
		obj.SetActive(true);



		//DecorController decor = obj.GetComponent<DecorController>();
		//if(decor != null)
		//  decor.SetOrder(0, layerId);

		return obj;
	}


	void Programm1(bool init = false) {
		if (init) {
			readyEnd = false;
			platformCount = 1;
			platformLenghtCount = Random.Range(3, 5);
			thisPlatformNum = 0;
			thisPlatformLenghtNum = 0;
			incrementHeight = Random.Range(3f, 4.5f);
			lastSpawnPoint = new Vector3(CameraController.displayDiff.rightDif(3), RunnerController.Instance.mapHeight, 0);
		}

		if ((lastSpawnPoint.x + platformWidth + breakWidth) <= CameraController.displayDiff.rightDif(3)) {

			thisPlatformLenghtNum++;
			GameObject platform = SpawnPlatform(new Vector3(lastSpawnPoint.x + platformWidth + breakWidth, RunnerController.Instance.mapHeight + incrementHeight, 0));

			if (thisPlatformLenghtNum == platformLenghtCount) {

				if (generateHendingPlatform != null)
					generateHendingPlatform(false, 1);
				readyEnd = true;
				platform.GetComponent<PlatformDecor>().OnDisableNow += DestroyLastPlatform;
			}
		}

	}



	void Programm2(bool init = false) {
		if (init) {
			readyEnd = false;
			platformCount = Random.Range(3, 5);
			thisPlatformNum = 0;
			platformLenghtCount = Random.Range(3, 5);
			thisPlatformLenghtNum = 0;
			incrementHeight = Random.Range(2.5f, 4.5f);
			lastSpawnPoint = new Vector3(CameraController.displayDiff.rightDif(3), RunnerController.Instance.mapHeight, 0);
		}

		if ((lastSpawnPoint.x + platformWidth + breakWidth) <= CameraController.displayDiff.rightDif(3)) {

			thisPlatformLenghtNum++;

			GameObject platform = SpawnPlatform(new Vector3(lastSpawnPoint.x + platformWidth + breakWidth, RunnerController.Instance.mapHeight + incrementHeight, 0));

			if (breakWidth > 0) breakWidth = 0;

			if (thisPlatformLenghtNum == platformLenghtCount) {
				thisPlatformLenghtNum = 0;
				thisPlatformNum++;
				platformLenghtCount = Random.Range(3, 5);
				breakWidth = Random.Range(1f, 3f);
				incrementHeight = Random.Range(2.5f, 4.5f);

				if (platformCount <= thisPlatformNum) {
					if (generateHendingPlatform != null)
						generateHendingPlatform(false, 2);
					readyEnd = true;
					platform.GetComponent<PlatformDecor>().OnDisableNow += DestroyLastPlatform;
				}
			}
		}

	}

	void Programm3(bool init = false) {
		if (init) {
			readyEnd = false;
			platformCount = Random.Range(3, 5);
			thisPlatformNum = 0;
			platformLenghtCount = Random.Range(2, 4);
			thisPlatformLenghtNum = 0;
			incrementHeight = Random.Range(2.5f, 4.5f);
			lastSpawnPoint = new Vector3(CameraController.displayDiff.rightDif(3), RunnerController.Instance.mapHeight, 0);
		}

		if ((lastSpawnPoint.x + platformWidth + breakWidth) <= CameraController.displayDiff.rightDif(3)) {

			thisPlatformLenghtNum++;

			GameObject platform = SpawnPlatform(new Vector3(lastSpawnPoint.x + platformWidth + breakWidth, RunnerController.Instance.mapHeight + incrementHeight, 0));

			if (breakWidth > 0)
				breakWidth = 0;

			if (thisPlatformLenghtNum == platformLenghtCount) {
				thisPlatformLenghtNum = 0;
				thisPlatformNum++;
				platformLenghtCount = Random.Range(2, 4);
				breakWidth = Random.Range(1f, 3f);
				incrementHeight = Random.Range(2.5f, 4.5f);

				if (platformCount <= thisPlatformNum) {
					if (generateHendingPlatform != null)
						generateHendingPlatform(false, 3);
					readyEnd = true;
					platform.GetComponent<PlatformDecor>().OnDisableNow += DestroyLastPlatform;
				}
			}
		}

	}



	void Programm4(bool init = false) {
		if (init) {
			readyEnd = false;
			platformCount = Random.Range(2, 6);
			thisPlatformNum = 0;
			platformLenghtCount = Random.Range(1, 2);
			thisPlatformLenghtNum = 0;
			incrementHeight = Random.Range(2.5f, 4.5f);
			lastSpawnPoint = new Vector3(CameraController.displayDiff.rightDif(3), RunnerController.Instance.mapHeight, 0);
		}

		if ((lastSpawnPoint.x + platformWidth + breakWidth) <= CameraController.displayDiff.rightDif(3)) {

			thisPlatformLenghtNum++;

			GameObject platform = SpawnPlatform(new Vector3(lastSpawnPoint.x + platformWidth + breakWidth, RunnerController.Instance.mapHeight + incrementHeight, 0));

			if (breakWidth > 0)
				breakWidth = 0;

			if (thisPlatformLenghtNum == platformLenghtCount) {
				thisPlatformLenghtNum = 0;
				thisPlatformNum++;
				platformLenghtCount = Random.Range(1, 2);
				breakWidth = Random.Range(0f, 1f);
				incrementHeight = Random.Range(2.5f, 4.5f);

				if (platformCount <= thisPlatformNum) {
					if (generateHendingPlatform != null)
						generateHendingPlatform(false, 4);
					readyEnd = true;
					platform.GetComponent<PlatformDecor>().OnDisableNow += DestroyLastPlatform;
				}
			}
		}

	}

	void Programm5(bool init = false) {
		if (init) {
			readyEnd = false;
			platformCount = Random.Range(5, 11);
			thisPlatformNum = 0;
			platformLenghtCount = Random.Range(1, 3);
			thisPlatformLenghtNum = 0;
			incrementHeight = Random.Range(2.5f, 4.5f);
			lastSpawnPoint = new Vector3(CameraController.displayDiff.rightDif(3), RunnerController.Instance.mapHeight, 0);
		}

		if ((lastSpawnPoint.x + platformWidth + breakWidth) <= CameraController.displayDiff.rightDif(3)) {

			thisPlatformLenghtNum++;

			GameObject platform = SpawnPlatform(new Vector3(lastSpawnPoint.x + platformWidth + breakWidth, RunnerController.Instance.mapHeight + incrementHeight, 0));

			if (breakWidth > 0)
				breakWidth = 0;

			if (thisPlatformLenghtNum == platformLenghtCount) {
				thisPlatformLenghtNum = 0;
				thisPlatformNum++;
				platformLenghtCount = Random.Range(1, 3);
				breakWidth = Random.Range(2f, 4f);
				incrementHeight = Random.Range(2.5f, 4.5f);

				if (platformCount <= thisPlatformNum) {
					if (generateHendingPlatform != null)
						generateHendingPlatform(false, 5);
					readyEnd = true;
					platform.GetComponent<PlatformDecor>().OnDisableNow += DestroyLastPlatform;
				}
			}
		}

	}


	void DestroyLastPlatform(PlatformDecor subscribe) {
		subscribe.OnDisableNow -= DestroyLastPlatform;
		if (EndHndingPlatform != null)
			EndHndingPlatform();
	}

	void StructProcessor() {
		//Создаем список объектов
		platformList = new List<KeyValuePair<GameObject, int>>();
		cd = new List<float>();
		float sum1 = 0;
		cd.Add(sum1);
		platformList.Add(new KeyValuePair<GameObject, int>(prefabPlatform, 5));
		sum1 += 1;
		cd.Add(sum1);

		if (sum1 != 1f) cd = cd.ConvertAll(x => x / sum1); //normalize cd, if it's not already normalized

	}

}
