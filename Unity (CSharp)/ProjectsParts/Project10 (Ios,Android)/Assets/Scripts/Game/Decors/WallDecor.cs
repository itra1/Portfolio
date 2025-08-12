using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(WallDecor))]
[CanEditMultipleObjects]
public class WallDecorEditor : Editor {

  int configNum;

  public override void OnInspectorGUI() {
    base.OnInspectorGUI();

    if(GUILayout.Button("Copy")) {
      ((WallDecor)target).CopyParametrs();
    }

    EditorGUILayout.BeginHorizontal();
    configNum = EditorGUILayout.IntField("Form", configNum);
    if(GUILayout.Button("Set param")) {
			foreach(WallDecor script in targets)
				script.SetForm(configNum);
    }
    EditorGUILayout.EndHorizontal();
    if(GUILayout.Button("Reset Spine")) {
			foreach(WallDecor script in targets)
				script.skeletonRenderer.Initialize(true);
    }
  }
}

#endif

[System.Serializable]
public struct WallDecorationParametrs {
  public List<WallType> types;
  public List<WallCategory> categories;
  public List<RegionType> setting;
  public WallBoneParametrs param;
}

[System.Serializable]
public struct WallBoneParametrs {
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

public class WallDecor : MonoBehaviour {

  public List<WallDecorationParametrs> decorationParametrs;

  public WallType type;
  public WallCategory category;
  public RegionType setting;

	public Transform graphic;

  private List<string> allNeedsBone = new List<string>();
  public delegate void OnDisableNow(DecorController my);
  public OnDisableNow onDisableNow;

  public SkeletonRenderer skeletonRenderer;                   // Рендер кости
  public MeshRenderer mesh;
  private Bone bone;

  public int countThis;
  public int sortingOrderKoef;
  private int sortingOrderOriginal;

  public GameObject netPref;                                  // Префаб паутины для паука
  private GameObject netInst;                                         // Экземпляр паутины
	
  public List<GameObject> decorObjects;

  void OnEnable() {
		skeletonRenderer.enabled = true;
		allNeedsBone = new List<string>();
    DeactiveTorch();
    sortingOrderOriginal = mesh.sortingOrder;
		StartCoroutine(Initing());

		graphic.localScale = new Vector3((GameManager.activeLevelData.moveVector == MoveVector.left ? -1 : 1),1,1);

		// Сортировка мешей, что бы не перепрыгивала графика
		if (sortingOrderKoef != 0 && transform.parent.GetComponent<DecorController>()) {
      if(transform.parent.GetComponent<DecorController>().countThis > 0 && sortingOrderKoef != 0) {
        mesh.sortingOrder = sortingOrderOriginal + ((transform.parent.GetComponent<DecorController>().countThis % sortingOrderKoef) * (int)Mathf.Sign(sortingOrderKoef));
      }
    }
  }

  ExposedList<Slot> slotList;
  //ExposedList<Bone> boneList;

  public void Init(WallType newType, WallCategory newCategory) {
    type = newType;
    category = newCategory;
    setting = Regions.type;
  }

	IEnumerator Initing() {
		yield return new WaitForEndOfFrame();
		slotList = skeletonRenderer.skeleton.slots;
		//boneList = skeletonRenderer.skeleton.bones;
		SetyTrsnsperent();
		yield return new WaitForEndOfFrame();
		skeletonRenderer.enabled = false;
	}
	
  void OnDisable() {
    mesh.sortingOrder = sortingOrderOriginal;
    DestroyNet();
    DeactiveTorch();
  }
	
  int i;
  int j;
  int n;

  void SetyTrsnsperent(int needNumber = -1) {

    decorObjects.ForEach(x => x.SetActive(false));

    WallDecorationParametrs parametrs = new WallDecorationParametrs();
		
    if(needNumber >= 0)
      parametrs = decorationParametrs[needNumber];
    else
      parametrs = decorationParametrs.Find(x => x.categories.Exists(y => y == category) && x.types.Exists(y => y == type) && x.setting.Exists(y => y == setting));

    if(parametrs.param.transperentParam.min == 0 && parametrs.param.transperentParam.max == 0) {
      parametrs.param.transperentParam.min = 1;
      parametrs.param.transperentParam.max = 1;
    }

    transform.eulerAngles = new Vector3(transform.localRotation.x, transform.localRotation.y, Random.Range(parametrs.param.rotateAngle.min, parametrs.param.rotateAngle.max));

    if(parametrs.param.bonesExcludingGroup.Count != 0) {

      int num = Random.Range(0 , parametrs.param.bonesExcludingGroup.Count);
      bool inArray;
      float alpha;

      for(i = 0; i < slotList.Items.Length; i++) {

        alpha = Random.Range(parametrs.param.transperentParam.min, parametrs.param.transperentParam.max);

        if(alpha == 1) {
          alpha = (parametrs.param.invertArray ? 0 : 1);
        }

        slotList.Items[i].A = alpha;
        inArray = false;
				
				for (j = 0; j < parametrs.param.bonesExcludingGroup[num].boneName.Count; j++) {
          if(slotList.Items[i].Bone.ToString() == parametrs.param.bonesExcludingGroup[num].boneName[j]) {
						
						inArray = true;
            slotList.Items[i].A = (parametrs.param.invertArray ? 1 : 0);
          }
        }

        try {
          if(!inArray) {
            skeletonRenderer.skeleton.SetAttachment(slotList.Items[i].Data.Name, slotList.Items[i].Data.Name);
          }
        } catch {
          slotList.Items[i].A = alpha;
        }

      }
    }
    if(parametrs.param.randomExec.boneName.Count > 0) {
      for(i = 0; i < slotList.Items.Length; i++) {
        for(j = 0; j < parametrs.param.randomExec.boneName.Count; j++) {
          if(slotList.Items[i].Bone.ToString() == parametrs.param.randomExec.boneName[j]) {
            slotList.Items[i].A = (Random.value <= 0.5f ? 1 : 0);
          }
        }
      }
		}

		if (parametrs.param.randomExecInGroup.Count > 0) {
      string execBone;
      for(i = 0; i < parametrs.param.randomExecInGroup.Count; i++) {
        if(parametrs.param.randomExecInGroup[i].probility >= Random.value) {
          execBone = parametrs.param.randomExecInGroup[i].boneName[Random.Range(0, parametrs.param.randomExecInGroup[i].boneName.Length)];
          for(j = 0; j < slotList.Count; j++) {
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
      for(i = 0; i < parametrs.param.dopExecForRandExac.Count; i++) {

        exec = false;

        for(j = 0; j < slotList.Items.Length; j++) {
          if(slotList.Items[j].Bone.ToString() == parametrs.param.dopExecForRandExac[i].boneName && slotList.Items[j].A <= 0) {
            exec = true;
          }
        }

        if(exec && parametrs.param.dopExecForRandExac[i].probility >= Random.value) {
          execBone = parametrs.param.dopExecForRandExac[i].boneNameLinks[Random.Range(0, parametrs.param.dopExecForRandExac[i].boneNameLinks.Length)];
          for(j = 0; j < slotList.Items.Length; j++) {
            if(slotList.Items[j].Bone.ToString() == execBone) {
              slotList.Items[j].A = 0;
            }
          }
        }
      }
		}

		if (parametrs.param.randomActiveInCondition.Count > 0) {
      for(i = 0; i < parametrs.param.randomActiveInCondition.Count; i++) {
        if(parametrs.param.randomActiveInCondition[i].probility >= Random.value) {
          for(j = 0; j < slotList.Count; j++) {
            for(n = 0; n < parametrs.param.randomActiveInCondition[i].boneName.Length; n++) {
              if(slotList.Items[j].Bone.ToString() == parametrs.param.randomActiveInCondition[i].boneName[n]) {
                slotList.Items[j].A = 1;
              }
            }
          }
        }
      }
    }

    if(parametrs.param.dopBonesLinksActive.Count > 0) {
      for(i = 0; i < parametrs.param.dopBonesLinksActive.Count; i++) {
        // рекурсивный вызов для полной проверки
        CheckDopActivate(parametrs, parametrs.param.dopBonesLinksActive[i].boneName);
      }
    }

    CheckLinks(parametrs);

    if(parametrs.param.decorObjects.Count > 0) {

      for(j = 0; j < parametrs.param.decorObjects.Count; j++)
        for(n = 0; n < parametrs.param.decorObjects[j].prefabs.Length; n++)
          parametrs.param.decorObjects[j].prefabs[n].prefab.SetActive(false);

      bool fixedDecor;
      float scaleX = 1;
      float scaleY = 1;
      int needNum;

      for(i = 0; i < parametrs.param.decorObjects.Count; i++)
        if(parametrs.param.decorObjects[i].generate && Random.value <= parametrs.param.decorObjects[i].probility) {
          fixedDecor = true;
          needNum = Random.Range(0, parametrs.param.decorObjects[i].prefabs.Length);

          scaleX = 1;
          scaleY = 1;

          for(j = 0; j < slotList.Items.Length; j++) {

            if(!parametrs.param.decorObjects[i].prefabs[needNum].disableBoneScale) {
              if(slotList.Items[j].Bone.ToString() == parametrs.param.decorObjects[i].prefabs[needNum].boneName) {
                scaleX = slotList.Items[j].bone.ScaleX * slotList.Items[j].bone.worldSignX;//( bon.bone. ? -1 : 1);
                scaleY = slotList.Items[j].bone.ScaleY * slotList.Items[j].bone.worldSignY;//( bon.bone. ? -1 : 1 );
              }
            } else {
              scaleX = Random.Range(parametrs.param.decorObjects[i].prefabs[needNum].sceles.min, parametrs.param.decorObjects[i].prefabs[needNum].sceles.max);
              scaleY = scaleX;
            }

            for(n = 0; n < parametrs.param.decorObjects[i].prefabs[needNum].NeedBone.Length; n++) {
              if(slotList.Items[j].Bone.ToString() == parametrs.param.decorObjects[i].prefabs[needNum].NeedBone[n]) {
                if(!parametrs.param.decorObjectsCheckOnly) {
                  slotList.Items[j].A = 1;
                  allNeedsBone.Add(parametrs.param.decorObjects[i].prefabs[needNum].NeedBone[n]);
                } else {
                  if(slotList.Items[j].A == 0)
                    fixedDecor = false;
                }
              }

            }

            for(n = 0; n < parametrs.param.decorObjects[i].prefabs[needNum].ExecBone.Length; n++) {

              if(!parametrs.param.decorObjectsCheckOnly && allNeedsBone.Exists(x => x == parametrs.param.decorObjects[i].prefabs[needNum].ExecBone[n])) {
                fixedDecor = false;
                continue;
              }

              if(slotList.Items[j].Bone.ToString() == parametrs.param.decorObjects[i].prefabs[needNum].ExecBone[n]) {
                if(!parametrs.param.decorObjectsCheckOnly)
                  slotList.Items[j].A = 0;
                else {
                  if(slotList.Items[j].A > 0)
                    fixedDecor = false;
                }
              }
            }
          }

          if(fixedDecor) {
            parametrs.param.decorObjects[i].prefabs[needNum].prefab.SetActive(true);
            parametrs.param.decorObjects[i].prefabs[needNum].prefab.transform.localScale = new Vector3(scaleX, scaleY, 1);

            if(parametrs.param.decorObjects[i].prefabs[needNum].boneName != "") {
              parametrs.param.decorObjects[i].prefabs[needNum].prefab.GetComponent<BoneFollower>().boneName = parametrs.param.decorObjects[i].prefabs[needNum].boneName;
              parametrs.param.decorObjects[i].prefabs[needNum].prefab.GetComponent<BoneFollower>().Reset();
            }
          }
        }
    }

    CheckLinks(parametrs);

  }

  public void SetColor(Color newColor) {

    for(i = 0; i < slotList.Items.Length; i++) {
      slotList.Items[i].R = newColor.r;
      slotList.Items[i].G = newColor.g;
      slotList.Items[i].B = newColor.b;
    }
  }

  void CheckDopActivate(WallDecorationParametrs parametrs, string boneNames) {

    bool activeBone;

    //foreach (bonesLinksCheckers one in dopBonesLinksActive) {
    for(j = 0; j < parametrs.param.dopBonesLinksActive.Count; j++) {
      if(parametrs.param.dopBonesLinksActive[j].boneName == boneNames) {
        activeBone = false;

        //foreach(Slot bon in skeletonRenderer.skeleton.slots) {
        for(i = 0; i < slotList.Items.Length; i++) {
          if(slotList.Items[i].Bone.ToString() == parametrs.param.dopBonesLinksActive[j].boneName && slotList.Items[i].A == 1) {
            activeBone = true;
          }
        }

        if(activeBone) {
          //foreach(Slot bon in skeletonRenderer.skeleton.slots) {
          for(i = 0; i < slotList.Items.Length; i++) {
            //foreach (string bonName in dopBonesLinksActive[j].boneNameLinks) {
            for(n = 0; n < parametrs.param.dopBonesLinksActive[j].boneNameLinks.Length; n++) {
              if(slotList.Items[i].Bone.ToString() == parametrs.param.dopBonesLinksActive[j].boneNameLinks[n] && slotList.Items[i].A <= 0) {
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
    if(mesh == null)
      return;

    mesh.sortingLayerID = orderId;
    mesh.sortingOrder = newOrder;
  }

  void CheckLinks(WallDecorationParametrs parametrs) {

    if(parametrs.param.bonesLinksChecker.Count == 0) return;
    bool deactive = true;
    for(j = 0; j < parametrs.param.bonesLinksChecker.Count; j++) {

      deactive = parametrs.param.bonesLinksChecker[j].full;

			//for (i = 0; i < slotList.Items.Length; i++) {
			//	for (n = 0; n < parametrs.param.bonesLinksChecker[j].boneNameLinks.Length; n++) {
			//		if (slotList.Items[i].Bone.ToString() == parametrs.param.bonesLinksChecker[j].boneNameLinks[n] && slotList.Items[i].A <= 0) {
			//			if(slotList.Items[i].A <= 0)
			//				deactive = true;
			//			else if(slotList.Items[i].A > 0)
			//				deactive = false;
			//		}
			//	}
			//}

			deactive = false;
			if (parametrs.param.bonesLinksChecker[j].full) {
				deactive = false;
				for (i = 0; i < slotList.Count; i++) {
					for (n = 0; n < parametrs.param.bonesLinksChecker[j].boneNameLinks.Length; n++) {
						if (slotList.Items[i].Bone.ToString() == parametrs.param.bonesLinksChecker[j].boneNameLinks[n] && slotList.Items[i].A <= 0) {
							deactive = true;
						}
					}
				}
			} else {
				for (i = 0; i < slotList.Count; i++) {
					for (n = 0; n < parametrs.param.bonesLinksChecker[j].boneNameLinks.Length; n++) {
						if (slotList.Items[i].Bone.ToString() == parametrs.param.bonesLinksChecker[j].boneNameLinks[n] && slotList.Items[i].A > 0) {
							deactive = false;
						}
					}
				}
			}


			if (deactive) {
        for(i = 0; i < slotList.Items.Length; i++) {
          if(slotList.Items[i].Bone.ToString() == parametrs.param.bonesLinksChecker[j].boneName) {
            slotList.Items[i].A = 0;
          }
        }
      }

    }
  }

  #region Паучья сеть
  void DestroyNet() {
    if(netInst != null)
      Destroy(netInst);
  }

  public void GenerateNet() {
    if(netPref == null)
      return;
    netInst = Instantiate(netPref, transform.position, Quaternion.identity) as GameObject;
    netInst.transform.parent = transform.parent;
    netInst.SetActive(true);
  }
  #endregion

  #region UNITY_EDITOR

  public WallsSpawner source;

  public void CopyParametrs() {

    //decorationParametrs.Clear();

    //foreach(Wall wall in source.walls) {
    //  WallDecorationParametrs param = new WallDecorationParametrs();

    //  if(wall.prefab.name == "Torch") continue;

    //  param.param.prefab = wall.prefab;
    //  param.param.bonesExcludingGroup = new List<bonesParameters>(wall.prefab.GetComponent<DecorController>().bonesExcludingGroup);
    //  param.param.bonesLinksChecker = new List<bonesLinksCheckers>(wall.prefab.GetComponent<DecorController>().bonesLinksChecker);
    //  param.param.decorObjects = new List<parametrArray>(wall.prefab.GetComponent<DecorController>().decorObjects);
    //  param.param.decorObjectsCheckOnly = wall.prefab.GetComponent<DecorController>().decorObjectsCheckOnly;
    //  param.param.dopBonesLinksActive = new List<bonesLinksCheckers>(wall.prefab.GetComponent<DecorController>().dopBonesLinksActive);
    //  param.param.dopExecForRandExac = new List<bonesDopExctParam>(wall.prefab.GetComponent<DecorController>().dopExecForRandExac);
    //  param.param.invertArray = wall.prefab.GetComponent<DecorController>().invertArray;
    //  param.param.randomActiveInCondition = new List<randomActiveParametrs>(wall.prefab.GetComponent<DecorController>().randomActiveInCondition);
    //  param.param.randomExec = wall.prefab.GetComponent<DecorController>().randomExec;
    //  param.param.randomExecInGroup = new List<bonesParametersExec>(wall.prefab.GetComponent<DecorController>().randomExecInGroup);
    //  param.param.rotateAngle = wall.prefab.GetComponent<DecorController>().rotateAngle;
    //  param.param.transperentParam = wall.prefab.GetComponent<DecorController>().transperentParam;
    //  decorationParametrs.Add(param);
    //}

  }

  public void SetForm(int needNum) {
    skeletonRenderer.Initialize(true);
    slotList = skeletonRenderer.skeleton.slots;


		//for (int xx = 0; xx < slotList.Items.Length; xx++) {
		//	Debug.Log(slotList.Items[xx].Bone.ToString());
		//	//Debug.Log(slotList.Items[xx].Data.Name.ToString());
		//}

		//boneList = skeletonRenderer.skeleton.bones;
    SetyTrsnsperent(needNum);
  }

  #endregion

  #region Факел


  public GameObject torchPref;                                // Префаб факела
  [HideInInspector]
  public GameObject torchInst;

  public torchParamPosition[] torchPosition;

  bool needTorch;

  public bool SetTorch() {
    if(torchPref == null || torchPosition.Length <= 0) return false;
    Invoke("TorchActivate", 0.5f);
    return true;
  }

  void TorchActivate() {
    bool fixedTorch = false;
    int iteratieon = 0;
    int torchNum;
    string needBone;

    DeactiveTorch();

    while(!fixedTorch && iteratieon <= 5) {
      iteratieon++;

      torchNum = Random.Range(0, torchPosition.Length);

      needBone = torchPosition[torchNum].needBoneName;
      foreach(Slot bon in skeletonRenderer.skeleton.slots) {
        if(needBone.Length <= bon.Data.Name.Length && bon.Data.Name.Substring(bon.Data.Name.Length - needBone.Length) == needBone && bon.A == 1) {
          fixedTorch = true;

          torchInst = (GameObject)Instantiate(torchPref, transform.position, Quaternion.identity);
          torchInst.transform.localScale = new Vector3((GameManager.isIPad ? 0.8f : 1), (GameManager.isIPad ? 0.8f : 1), (GameManager.isIPad ? 0.8f : 1));
          torchInst.transform.parent = transform;

          BoneFollower torchBone = torchInst.AddComponent<BoneFollower>();
          torchBone.skeletonRenderer = skeletonRenderer;
          torchBone.boneName = torchPosition[torchNum].boneName;
          torchBone.followBoneRotation = true;
          torchBone.followZPosition = true;
          torchBone.Reset();
        }
      }
    }
  }

  void DeactiveTorch() {
    if(torchInst != null)
      Destroy(torchInst);
  }

  #endregion

}
