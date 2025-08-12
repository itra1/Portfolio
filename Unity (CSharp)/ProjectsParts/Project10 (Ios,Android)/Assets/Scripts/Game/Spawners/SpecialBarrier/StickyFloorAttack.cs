using UnityEngine;

/// <summary>
/// Генератор скользкого пола
/// </summary>
public class StickyFloorAttack : ExEvent.EventBehaviour {

	[SerializeField]
	SpecialBarriersTypes barrierType;                  // Тип препядствия
	[SerializeField]
	public GameObject stickFlorPrefab;                 // Префаб пола

	[SerializeField]
	float sizeElement;                                 // Размер пролета
	[SerializeField]
	IntSpan countElements;                             // Количество пролетов в элементе
	[SerializeField]
	IntSpan countWork;                                 // Количество генерируемых секторов
	int needCount;
	int thisCount;
	float activeDistantion;

	float nextDistantionElement;                                        // Рассчет следующей генерации

	bool isActive;

	void Start() {
		//RunSpawner.OnSpecialBarrier += OnSpecialBarrier;
	}

	protected override void OnDestroy() {
		base.OnDestroy();
		//RunSpawner.OnSpecialBarrier -= OnSpecialBarrier;
	}

	void Update() {
		if (!isActive) return;

		if (nextDistantionElement <= RunnerController.playerDistantion) GenerateElenemt();

		if (thisCount == needCount) {
			isActive = false;
			EndBarrier();
		}
	}

	System.Action EndBarrier;

	/// <summary>
	/// Подпись на выполнение препядствия
	/// </summary>
	/// <param name="barrier">Тип атаки</param>
	/// <param name="CallBack"></param>
	[ExEvent.ExEventHandler(typeof(ExEvent.RunEvents.SpecialBarrier))]
	void OnSpecialBarrier(ExEvent.RunEvents.SpecialBarrier specialBarrier) {

		if (!isActive || specialBarrier.barrier == null || barrierType != specialBarrier.barrier) return;                     // Не соответствует тип
		//if (specialBarrier.callback != null)
		//	EndBarrier = specialBarrier.callback;
		CalcGenerateAttack();
	}

	/// <summary>
	/// Рассчет генерации
	/// </summary>
	void CalcGenerateAttack() {
		//activeDistantion = RunnerController.playerDistantion + Random.Range(distantionWork.min, distantionWork.max);
		needCount = Random.Range(countWork.min, countWork.max);
		thisCount = 0;
		isActive = true;
		nextSize = Random.Range(countElements.min, countElements.max + 1);
	}

	int nextSize;

	void GenerateElenemt() {
		GameObject clone = (GameObject)Instantiate(stickFlorPrefab,new Vector3(CameraController.displayDiff.rightDif(2f), CameraController.displayDiff.transform.position.y, transform.position.z),Quaternion.identity);
		thisCount++;
		clone.GetComponent<StickyFloor>().SetSize(nextSize);
		nextDistantionElement = RunnerController.playerDistantion + nextSize * sizeElement / 2;
		nextSize = Random.Range(countElements.min, countElements.max + 1);
		nextDistantionElement += nextSize * sizeElement / 2;
	}

}
