using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using Spine;
using Spine.Unity;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlatformDecor))]
[CanEditMultipleObjects]
public class PlatformdecorEditor : Editor {

	int configNum;

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Copy")) {
			//((PlatformDecor)target).CopyParametrs();
		}

		EditorGUILayout.BeginHorizontal();
		configNum = EditorGUILayout.IntField("Form", configNum);
		if (GUILayout.Button("Set param")) {

			foreach (PlatformDecor script in targets)
				script.SetForm(configNum);

		}
		EditorGUILayout.EndHorizontal();
		if (GUILayout.Button("Reset Spine")) {
			foreach (PlatformDecor script in targets)
				script.SetForm(configNum);
		}
	}
}

#endif

[System.Serializable]
public struct PratformDecorationParametrs {
	public List<PlatformType> types;
	public List<PlatformCategory> categories;
	public List<RegionType> setting;
	public PlatformBoneParametrs param;
}

[System.Serializable]
public struct PlatformBoneParametrs {
	public GameObject prefab;
	public List<bonesParameters> bonesExcludingGroup;               // Группы для исключения
	public bool invertArray;                                        // Инвертировать список, тоесть включать только указанные кости
	public bonesParameters randomExec;                              // Рандомное отключение
	public List<randomActiveParametrs> randomActiveInCondition;     // Рандомное включение в зависимости от параметров
	public List<bonesLinksCheckers> dopBonesLinksActive;            // Проверка на обязательную связь
	public List<bonesParametersExec> randomExecInGroup;             // Рандомное исключение из группы с вероятностью
	public List<bonesDopExctParam> dopExecForRandExac;
	public List<bonesLinksCheckers> bonesLinksChecker;              // Проверка на обязательную связь
	public FloatSpan rotateAngle;
	public FloatSpan transperentParam;                              // Настройка прозрачности
	public List<parametrArray> decorObjects;
	public bool decorObjectsCheckOnly;                              // Только проверять
}

public class PlatformDecor : SpawnAbstract {

	public List<PratformDecorationParametrs> pratformDecorationParametrs;
	public Transform graphic;

	public BoxCollider2D boxCollider;

	public PlatformType type;
	public PlatformCategory category;
	public RegionType setting;

	private List<string> allNeedsBone = new List<string>();
	public Action<PlatformDecor> OnDisableNow;

	public SkeletonRenderer skeletonRenderer;                   // Рендер кости
	public MeshRenderer mesh;
	private Bone bone;

	public int countThis;
	public int sortingOrderKoef;
	int sortingOrderOriginal;

	public GameObject netPref;                                  // Префаб паутины для паука
	GameObject netInst;                                         // Экземпляр паутины
	
	public List<GameObject> decorObjects;
	
	public void Inicialize(int? num = null) {
		skeletonRenderer.enabled = true;
		allNeedsBone.Clear();
		sortingOrderOriginal = mesh.sortingOrder;

		graphic.localScale = new Vector3(GameManager.activeLevelData.moveVector == MoveVector.left?-1:1,1,1);
		
		StartCoroutine(Init(num));

		GraphicSort();
		boxCollider.enabled = type == PlatformType.breaks;
	}

	void GraphicSort() {
		// Сортировка мешей, что бы не перепрыгивала графика
		if (sortingOrderKoef != 0 && transform.parent.GetComponent<DecorController>()) {
			if (transform.parent.GetComponent<DecorController>().countThis > 0 && sortingOrderKoef != 0) {
				mesh.sortingOrder = sortingOrderOriginal + ((transform.parent.GetComponent<DecorController>().countThis % sortingOrderKoef) * (int)Mathf.Sign(sortingOrderKoef));
			}
		}
	}

	ExposedList<Slot> slotList;

	public void Init(PlatformType newType, PlatformCategory newCategory) {
		type = newType;
		category = newCategory;
		setting = Regions.type;
		boxCollider.enabled = newType == PlatformType.breaks;
	}

	IEnumerator Init(int? num) {
		yield return new WaitForEndOfFrame();
		slotList = skeletonRenderer.skeleton.slots;
		SetyTrsnsperent(num);

		yield return new WaitForEndOfFrame();
		skeletonRenderer.enabled = false;
	}
	
	void OnDisable() {
		if (OnDisableNow != null) OnDisableNow(this);
		mesh.sortingOrder = sortingOrderOriginal;
		DestroyNet();
	}

	int i, j, n;

	void SetyTrsnsperent(int? needNumber) {

		decorObjects.ForEach(x => x.SetActive(false));

		PratformDecorationParametrs parametrs = new PratformDecorationParametrs();

		if (needNumber != null)
			parametrs = pratformDecorationParametrs[(int)needNumber];
		else
			parametrs = pratformDecorationParametrs.Find(x => x.categories.Exists(y => y == category) && x.types.Exists(y => y == type) && x.setting.Exists(y => y == setting));

		if (parametrs.param.transperentParam.min == 0 && parametrs.param.transperentParam.max == 0) {
			parametrs.param.transperentParam.min = 1;
			parametrs.param.transperentParam.max = 1;
		}

		transform.eulerAngles = new Vector3(transform.localRotation.x, transform.localRotation.y, Random.Range(parametrs.param.rotateAngle.min, parametrs.param.rotateAngle.max));

		try {
			if (parametrs.param.bonesExcludingGroup.Count != 0) {

				int num = Random.Range(0, parametrs.param.bonesExcludingGroup.Count);
				bool inArray;
				float alpha;

				for (i = 0; i < slotList.Items.Length; i++) {

					alpha = Random.Range(parametrs.param.transperentParam.min, parametrs.param.transperentParam.max);

					if (alpha == 1) {
						alpha = (parametrs.param.invertArray ? 0 : 1);
					}

					slotList.Items[i].A = alpha;
					inArray = false;

					for (j = 0; j < parametrs.param.bonesExcludingGroup[num].boneName.Count; j++) {
						if (slotList.Items[i].Bone.ToString() == parametrs.param.bonesExcludingGroup[num].boneName[j]) {
							
							inArray = true;
							slotList.Items[i].A = (parametrs.param.invertArray ? 1 : 0);
						}
					}

					try {
						if (!inArray) {
							skeletonRenderer.skeleton.SetAttachment(slotList.Items[i].Data.Name, slotList.Items[i].Data.Name);
						}
					} catch {
						slotList.Items[i].A = alpha;
					}

				}
			}
		} catch {
			Debug.LogError("Platform = " + category + " : " + type + " : " + setting);
		}

		if (parametrs.param.randomExec.boneName.Count > 0) {

			List<string> bonesName = parametrs.param.randomExec.boneName;

			for (i = 0; i < slotList.Items.Length; i++) {
				for (j = 0; j < bonesName.Count; j++) {
					if (slotList.Items[i].Bone.ToString() == bonesName[j]) {
						
						slotList.Items[i].A = (Random.value <= 0.5f ? 1 : 0);
					}
				}
			}

		}

		if (parametrs.param.randomExecInGroup.Count > 0) {
			string execBone;
			for (i = 0; i < parametrs.param.randomExecInGroup.Count; i++) {
				if (parametrs.param.randomExecInGroup[i].probility >= Random.value) {
					execBone = parametrs.param.randomExecInGroup[i].boneName[Random.Range(0, parametrs.param.randomExecInGroup[i].boneName.Length)];
					for (j = 0; j < slotList.Items.Length; j++) {
						if (slotList.Items[j].Bone.ToString() == execBone) {
							slotList.Items[j].A = 0;
						}
					}
				}
			}
		}

		if (parametrs.param.dopExecForRandExac.Count > 0) {

			bool exec = false;
			string execBone;
			for (i = 0; i < parametrs.param.dopExecForRandExac.Count; i++) {

				exec = false;

				for (j = 0; j < slotList.Items.Length; j++) {
					if (slotList.Items[j].Bone.ToString() == parametrs.param.dopExecForRandExac[i].boneName && slotList.Items[j].A <= 0) {
						exec = true;
					}
				}

				if (exec && parametrs.param.dopExecForRandExac[i].probility >= Random.value) {
					execBone = parametrs.param.dopExecForRandExac[i].boneNameLinks[Random.Range(0, parametrs.param.dopExecForRandExac[i].boneNameLinks.Length)];
					for (j = 0; j < slotList.Items.Length; j++) {
						if (slotList.Items[j].Bone.ToString() == execBone) {
							slotList.Items[j].A = 0;
						}
					}
				}
			}
		}

		if (parametrs.param.randomActiveInCondition.Count > 0) {

			for (i = 0; i < parametrs.param.randomActiveInCondition.Count; i++) {
				if (parametrs.param.randomActiveInCondition[i].probility >= Random.value) {
					for (j = 0; j < slotList.Items.Length; j++) {
						for (n = 0; n < parametrs.param.randomActiveInCondition[i].boneName.Length; n++) {
							if (slotList.Items[j].Bone.ToString() == parametrs.param.randomActiveInCondition[i].boneName[n]) {
								slotList.Items[j].A = 1;
							}
						}
					}
				}
			}
		}

		if (parametrs.param.dopBonesLinksActive.Count > 0) {
			for (i = 0; i < parametrs.param.dopBonesLinksActive.Count; i++) {
				// рекурсивный вызов для полной проверки
				CheckDopActivate(parametrs, parametrs.param.dopBonesLinksActive[i].boneName);
			}
		}

		//CheckLinks(parametrs);

		if (parametrs.param.decorObjects.Count > 0) {

			for (j = 0; j < parametrs.param.decorObjects.Count; j++)
				for (n = 0; n < parametrs.param.decorObjects[j].prefabs.Length; n++)
					parametrs.param.decorObjects[j].prefabs[n].prefab.SetActive(false);

			bool fixedDecor;
			float scaleX = 1;
			float scaleY = 1;
			int needNum;

			for (i = 0; i < parametrs.param.decorObjects.Count; i++)
				if (parametrs.param.decorObjects[i].generate && Random.value <= parametrs.param.decorObjects[i].probility) {
					fixedDecor = true;
					needNum = Random.Range(0, parametrs.param.decorObjects[i].prefabs.Length);

					scaleX = 1;
					scaleY = 1;

					for (j = 0; j < slotList.Items.Length; j++) {

						if (!parametrs.param.decorObjects[i].prefabs[needNum].disableBoneScale) {
							if (slotList.Items[j].Bone.ToString() == parametrs.param.decorObjects[i].prefabs[needNum].boneName) {
								scaleX = slotList.Items[j].bone.ScaleX * slotList.Items[j].bone.worldSignX;//( bon.bone. ? -1 : 1);
								scaleY = slotList.Items[j].bone.ScaleY * slotList.Items[j].bone.worldSignY;//( bon.bone. ? -1 : 1 );
							}
						} else {
							scaleX = Random.Range(parametrs.param.decorObjects[i].prefabs[needNum].sceles.min, parametrs.param.decorObjects[i].prefabs[needNum].sceles.max);
							scaleY = scaleX;
						}

						for (n = 0; n < parametrs.param.decorObjects[i].prefabs[needNum].NeedBone.Length; n++) {
							if (slotList.Items[j].Bone.ToString() == parametrs.param.decorObjects[i].prefabs[needNum].NeedBone[n]) {
								if (!parametrs.param.decorObjectsCheckOnly) {
									slotList.Items[j].A = 1;
									allNeedsBone.Add(parametrs.param.decorObjects[i].prefabs[needNum].NeedBone[n]);
								} else {
									if (slotList.Items[j].A == 0)
										fixedDecor = false;
								}
							}

						}

						for (n = 0; n < parametrs.param.decorObjects[i].prefabs[needNum].ExecBone.Length; n++) {

							if (!parametrs.param.decorObjectsCheckOnly && allNeedsBone.Exists(x => x == parametrs.param.decorObjects[i].prefabs[needNum].ExecBone[n])) {
								fixedDecor = false;
								continue;
							}

							if (slotList.Items[j].Bone.ToString() == parametrs.param.decorObjects[i].prefabs[needNum].ExecBone[n]) {
								if (!parametrs.param.decorObjectsCheckOnly)
									slotList.Items[j].A = 0;
								else {
									if (slotList.Items[j].A > 0)
										fixedDecor = false;
								}
							}

						}
					}

					if (fixedDecor) {
						parametrs.param.decorObjects[i].prefabs[needNum].prefab.SetActive(true);
						parametrs.param.decorObjects[i].prefabs[needNum].prefab.transform.localScale = new Vector3(scaleX, scaleY, 1);

						parametrs.param.decorObjects[i].prefabs[needNum].prefab.GetComponent<BoneFollower>().boneName = parametrs.param.decorObjects[i].prefabs[needNum].boneName;
						parametrs.param.decorObjects[i].prefabs[needNum].prefab.GetComponent<BoneFollower>().Reset();
					}
				}
		}

		CheckLinks(parametrs);

	}

	public void SetColor(Color newColor) {

		for (i = 0; i < slotList.Items.Length; i++) {
			slotList.Items[i].R = newColor.r;
			slotList.Items[i].G = newColor.g;
			slotList.Items[i].B = newColor.b;
		}
	}

	void CheckDopActivate(PratformDecorationParametrs parametrs, string boneNames) {

		bool activeBone;

		//foreach (bonesLinksCheckers one in dopBonesLinksActive) {
		for (j = 0; j < parametrs.param.dopBonesLinksActive.Count; j++) {
			if (parametrs.param.dopBonesLinksActive[j].boneName == boneNames) {
				activeBone = false;

				//foreach(Slot bon in skeletonRenderer.skeleton.slots) {
				for (i = 0; i < slotList.Items.Length; i++) {
					if (slotList.Items[i].Bone.ToString() == parametrs.param.dopBonesLinksActive[j].boneName && slotList.Items[i].A == 1) {
						activeBone = true;
					}
				}

				if (activeBone) {
					//foreach(Slot bon in skeletonRenderer.skeleton.slots) {
					for (i = 0; i < slotList.Items.Length; i++) {
						//foreach (string bonName in dopBonesLinksActive[j].boneNameLinks) {
						for (n = 0; n < parametrs.param.dopBonesLinksActive[j].boneNameLinks.Length; n++) {
							if (slotList.Items[i].Bone.ToString() == parametrs.param.dopBonesLinksActive[j].boneNameLinks[n] && slotList.Items[i].A <= 0) {
								slotList.Items[i].A = 1;
								CheckDopActivate(parametrs, parametrs.param.dopBonesLinksActive[j].boneNameLinks[n]);
							}
						}
					}
				}
			}
		}

	}

	public void SetOrder(int newOrder, int orderId) {
		if (mesh == null)
			return;

		mesh.sortingLayerID = orderId;
		mesh.sortingOrder = newOrder;
	}

	void CheckLinks(PratformDecorationParametrs parametrs) {

		if (parametrs.param.bonesLinksChecker.Count == 0)
			return;
		bool deactive = true;
		for (j = 0; j < parametrs.param.bonesLinksChecker.Count; j++) {

			deactive = true;

			if (parametrs.param.bonesLinksChecker[j].full) {
				deactive = false;
				for (i = 0; i < slotList.Items.Length; i++) {
					for (n = 0; n < parametrs.param.bonesLinksChecker[j].boneNameLinks.Length; n++) {
						if (slotList.Items[i].Bone.ToString() == parametrs.param.bonesLinksChecker[j].boneNameLinks[n] && slotList.Items[i].A <= 0) {
							deactive = true;
						}
					}
				}
			} else {
				for (i = 0; i < slotList.Items.Length; i++) {
					for (n = 0; n < parametrs.param.bonesLinksChecker[j].boneNameLinks.Length; n++) {
						if (slotList.Items[i].Bone.ToString() == parametrs.param.bonesLinksChecker[j].boneNameLinks[n] && slotList.Items[i].A > 0) {
							deactive = false;
						}
					}
				}
			}


			if (deactive) {
				for (i = 0; i < slotList.Items.Length; i++) {
					if (slotList.Items[i].Bone.ToString() == parametrs.param.bonesLinksChecker[j].boneName) {
						slotList.Items[i].A = 0;
					}
				}
			}

		}
	}

	#region Паучья сеть
	void DestroyNet() {
		if (netInst != null)
			Destroy(netInst);
	}

	public void GenerateNet() {
		if (netPref == null)
			return;
		netInst = Instantiate(netPref, transform.position, Quaternion.identity) as GameObject;
		netInst.transform.parent = transform.parent;
		netInst.SetActive(true);
	}
	#endregion

	#region UNITY_EDITOR

	public PlatformSpawner source;

	public void CopyParametrs() {

		pratformDecorationParametrs.Clear();

		foreach (Platform platform in source.platforms) {
			PratformDecorationParametrs param = new PratformDecorationParametrs();

			param.param.prefab = platform.prefab;
			param.param.bonesExcludingGroup = new List<bonesParameters>(platform.prefab.GetComponent<DecorController>().bonesExcludingGroup);
			param.param.bonesLinksChecker = new List<bonesLinksCheckers>(platform.prefab.GetComponent<DecorController>().bonesLinksChecker);
			param.param.decorObjects = new List<parametrArray>(platform.prefab.GetComponent<DecorController>().decorObjects);
			param.param.decorObjectsCheckOnly = platform.prefab.GetComponent<DecorController>().decorObjectsCheckOnly;
			param.param.dopBonesLinksActive = new List<bonesLinksCheckers>(platform.prefab.GetComponent<DecorController>().dopBonesLinksActive);
			param.param.dopExecForRandExac = new List<bonesDopExctParam>(platform.prefab.GetComponent<DecorController>().dopExecForRandExac);
			param.param.invertArray = platform.prefab.GetComponent<DecorController>().invertArray;
			param.param.randomActiveInCondition = new List<randomActiveParametrs>(platform.prefab.GetComponent<DecorController>().randomActiveInCondition);
			param.param.randomExec = platform.prefab.GetComponent<DecorController>().randomExec;
			param.param.randomExecInGroup = new List<bonesParametersExec>(platform.prefab.GetComponent<DecorController>().randomExecInGroup);
			param.param.rotateAngle = platform.prefab.GetComponent<DecorController>().rotateAngle;
			param.param.transperentParam = platform.prefab.GetComponent<DecorController>().transperentParam;
			pratformDecorationParametrs.Add(param);
		}
	}

	public void SetForm(int needNum) {

		if (Application.isPlaying) {
			Inicialize(needNum);
			return;
		}

		skeletonRenderer.Initialize(true);
		slotList = skeletonRenderer.skeleton.slots;
		
		GraphicSort();

		SetyTrsnsperent(needNum);
	}

	#endregion

}
