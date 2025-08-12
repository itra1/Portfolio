using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using Random = UnityEngine.Random;

[System.Serializable]
public struct prefabNeedsBone {
  public GameObject prefab;                               // Вероятность генерации группы


  public bool disableBoneScale;
  public FloatSpan sceles;

  [SpineBone(dataField: "skeletonRenderer")]
  public string boneName;
  [SpineBone(dataField: "skeletonRenderer")]
  public string[] NeedBone;
  [SpineBone(dataField: "skeletonRenderer")]
  public string[] ExecBone;
}
[System.Serializable]
public struct parametrArray {
  public bool generate;                                   // Разрешение на выбор группы
  [Range(0,1)]
  public float probility;                                 // Вероятность генерации группы
  public prefabNeedsBone[] prefabs;                       // Объекты для генерации
}
[System.Serializable]
public struct bonesParameters {
  [SpineBone(dataField: "skeletonRenderer")]
  public List<string> boneName;
}
[System.Serializable]
public struct bonesParametersExec {
  public bool inverce;
  [Range(0,1)]
  public float probility;
  [SpineBone(dataField: "skeletonRenderer")]
  public string[] boneName;
}
[System.Serializable]
public struct randomActiveParametrs {
  [Range(0,1)]
  public float probility;
  public FloatSpan distance;
  [SpineBone(dataField: "skeletonRenderer")]
  public string[] boneName;
}
[System.Serializable]
public struct bonesLinksCheckers {
  public bool full;
  [SpineBone(dataField: "skeletonRenderer")]
  public string boneName;
  [SpineBone(dataField: "skeletonRenderer")]
  public string[] boneNameLinks;
}
[System.Serializable]
public struct torchParamPosition {
  [SpineBone(dataField: "skeletonRenderer")]
  public string boneName;
  [SpineBone(dataField: "skeletonRenderer")]
  public string needBoneName;
}
[System.Serializable]
public struct bonesDopExctParam {
  [Range(0,1)]
  public float probility;
  [SpineBone(dataField: "skeletonRenderer")]
  public string boneName;
  [SpineBone(dataField: "skeletonRenderer")]
  public string[] boneNameLinks;
}

public class DecorController : MonoBehaviour {

	public bool deactiveSpineRender;

  List<string> allNeedsBone;
  public Action<DecorController> onDisableNow;

  public SkeletonRenderer skeletonRenderer;                   // Рендер кости
  public MeshRenderer mesh;
  public bonesParameters[] bonesExcludingGroup;               // Группы для исключения
  public bool invertArray;                                    // Инвертировать список, тоесть включать только указанные кости
  public bonesParameters randomExec;                          // Рандомное отключение
  public randomActiveParametrs[] randomActiveInCondition;     // Рандомное включение в зависимости от параметров
  public bonesLinksCheckers[] dopBonesLinksActive;            // Проверка на обязательную связь
  public bonesParametersExec[] randomExecInGroup;             // Рандомное исключение из группы с вероятностью
  public bonesDopExctParam[] dopExecForRandExac;

  public bonesLinksCheckers[] bonesLinksChecker;                // Проверка на обязательную связь

  Bone bone;
  public FloatSpan rotateAngle;
  public FloatSpan transperentParam;                          // Настройка прозрачности

  public parametrArray[] decorObjects;
  public bool decorObjectsCheckOnly;                          // Только проверять

  public int countThis;
  public int sortingOrderKoef;
  int sortingOrderOriginal;

  public GameObject netPref;                                  // Префаб паутины для паука
  GameObject netInst;                                         // Экземпляр паутины
	
  void OnEnable() {

		if (deactiveSpineRender)
			skeletonRenderer.enabled = true;

		allNeedsBone = new List<string>();
    DeactiveTorch();

    if(transperentParam.min == 0 && transperentParam.max == 0) {
      transperentParam.min = 1;
      transperentParam.max = 1;
    }
		
    sortingOrderOriginal = mesh.sortingOrder;
		StartCoroutine(Init());

		// Сортировка мешей, что бы не перепрыгивала графика
		if (sortingOrderKoef != 0 && transform.parent.GetComponent<DecorController>()) {
      if(transform.parent.GetComponent<DecorController>().countThis > 0 && sortingOrderKoef != 0) {
        mesh.sortingOrder = sortingOrderOriginal + ((transform.parent.GetComponent<DecorController>().countThis % sortingOrderKoef) * (int)Mathf.Sign(sortingOrderKoef));
      }
    }
  }

  ExposedList<Slot> slotList;

	IEnumerator Init() {
		yield return new WaitForEndOfFrame();
		slotList = skeletonRenderer.skeleton.slots;
		SetyTrsnsperent();
		yield return new WaitForEndOfFrame();

		if (deactiveSpineRender)
			skeletonRenderer.enabled = false;

	}

	//void Update() {
 //   if(lateUpdateWait) {
 //     lateUpdateWait = false;
 //     slotList = skeletonRenderer.skeleton.slots;
 //     SetyTrsnsperent();
 //   }

 // }

  void OnDisable() {
    mesh.sortingOrder = sortingOrderOriginal;
    if(onDisableNow != null)
      onDisableNow(this);
    DeactiveTorch();
    DestroyNet();
  }
	
  void SetyTrsnsperent() {

    transform.eulerAngles = new Vector3(transform.localRotation.x, transform.localRotation.y, Random.Range(rotateAngle.min, rotateAngle.max));

    if(bonesExcludingGroup.Length != 0) {

      int num = Random.Range(0 , bonesExcludingGroup.Length);
      bool inArray;
      float alpha;

      for(int i = 0; i < slotList.Count; i++) {

        alpha = Random.Range(transperentParam.min, transperentParam.max);

        if(alpha == 1) {
          alpha = (invertArray ? 0 : 1);
        }

        slotList.Items[i].A = alpha;
        inArray = false;

        for(int j = 0; j < bonesExcludingGroup[num].boneName.Count; j++) {
          if(bonesExcludingGroup[num].boneName[j].Length <= slotList.Items[i].Data.Name.Length && slotList.Items[i].Data.Name.Substring(slotList.Items[i].Data.Name.Length - bonesExcludingGroup[num].boneName[j].Length) == bonesExcludingGroup[num].boneName[j]) {
            inArray = true;
            slotList.Items[i].A = (invertArray ? 1 : 0);
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

    if(randomExec.boneName.Count > 0) {
      for(int i = 0; i < slotList.Count; i++) {
        for(int j = 0; j < randomExec.boneName.Count; j++) {
          if(randomExec.boneName[j].Length <= slotList.Items[i].Data.Name.Length && slotList.Items[i].Data.Name.Substring(slotList.Items[i].Data.Name.Length - randomExec.boneName[j].Length) == randomExec.boneName[j]) {
            slotList.Items[i].A = (Random.value <= 0.5f ? 1 : 0);
          }
        }
      }
    }
    ///****
    if(randomExecInGroup.Length > 0) {
      string execBone;
      for(int i = 0; i < randomExecInGroup.Length; i++) {
        if(randomExecInGroup[i].probility >= Random.value) {
          execBone = randomExecInGroup[i].boneName[Random.Range(0, randomExecInGroup[i].boneName.Length)];
          for(int j = 0; j < slotList.Count; j++) {
            if(execBone.Length <= slotList.Items[j].Data.Name.Length && slotList.Items[j].Data.Name.Substring(slotList.Items[j].Data.Name.Length - execBone.Length) == execBone) {
              slotList.Items[j].A = 0;
            }
          }
        }
      }
    }
    ///*****
    if(dopExecForRandExac.Length > 0) {

      bool exec = false;
      string execBone;
      for(int i = 0; i < dopExecForRandExac.Length; i++) {

        exec = false;

        for(int j = 0; j < slotList.Count; j++) {
          if(dopExecForRandExac[i].boneName.Length <= slotList.Items[j].Data.Name.Length && slotList.Items[j].Data.Name.Substring(slotList.Items[j].Data.Name.Length - dopExecForRandExac[i].boneName.Length) == dopExecForRandExac[i].boneName && slotList.Items[j].A <= 0) {
            exec = true;
          }
        }

        if(exec && dopExecForRandExac[i].probility >= Random.value) {
          execBone = dopExecForRandExac[i].boneNameLinks[Random.Range(0, dopExecForRandExac[i].boneNameLinks.Length)];
          for(int j = 0; j < slotList.Count; j++) {
            if(execBone.Length <= slotList.Items[j].Data.Name.Length && slotList.Items[j].Data.Name.Substring(slotList.Items[j].Data.Name.Length - execBone.Length) == execBone) {
              slotList.Items[j].A = 0;
            }
          }
        }
      }
    }
    //******
    // Рандомное отключение/включение костей по условиям
    if(randomActiveInCondition.Length > 0) {

      for(int i = 0; i < randomActiveInCondition.Length; i++) {
        if((RunnerController.distanceInRegion >= randomActiveInCondition[i].distance.min && RunnerController.distanceInRegion < randomActiveInCondition[i].distance.max) && randomActiveInCondition[i].probility >= Random.value) {
          for(int j = 0; j < slotList.Count; j++) {
            for(int n = 0; n < randomActiveInCondition[i].boneName.Length; n++) {
              if(randomActiveInCondition[i].boneName[n].Length <= slotList.Items[j].Data.Name.Length && slotList.Items[j].Data.Name.Substring(slotList.Items[j].Data.Name.Length - randomActiveInCondition[i].boneName[n].Length) == randomActiveInCondition[i].boneName[n]) {
                slotList.Items[j].A = 1;
              }
            }
          }
        }
      }
    }
    /////

    if(dopBonesLinksActive.Length > 0) {
      for(int i = 0; i < dopBonesLinksActive.Length; i++) {
        // рекурсивный вызов для полной проверки
        CheckDopActivate(dopBonesLinksActive[i].boneName);
      }
    }
    //////

    CheckLinks();

    if(decorObjects.Length > 0) {

      for(int j = 0; j < decorObjects.Length; j++)
        for(int n = 0; n < decorObjects[j].prefabs.Length; n++)
          decorObjects[j].prefabs[n].prefab.SetActive(false);

      bool fixedDecor;
      float scaleX = 1;
      float scaleY = 1;
      int needNum;

      for(int i = 0; i < decorObjects.Length; i++)
        if(decorObjects[i].generate && Random.value <= decorObjects[i].probility) {
          fixedDecor = true;
          needNum = Random.Range(0, decorObjects[i].prefabs.Length);

          scaleX = 1;
          scaleY = 1;

          for(int j = 0; j < slotList.Count; j++) {

            if(!decorObjects[i].prefabs[needNum].disableBoneScale) {
              if(decorObjects[i].prefabs[needNum].boneName.Length <= slotList.Items[j].Data.Name.Length && slotList.Items[j].Data.Name.Substring(slotList.Items[j].Data.Name.Length - decorObjects[i].prefabs[needNum].boneName.Length) == decorObjects[i].prefabs[needNum].boneName) {
                scaleX = slotList.Items[j].bone.ScaleX * slotList.Items[j].bone.worldSignX;//( bon.bone. ? -1 : 1);
                scaleY = slotList.Items[j].bone.ScaleY * slotList.Items[j].bone.worldSignY;//( bon.bone. ? -1 : 1 );
              }
            } else {
              scaleX = Random.Range(decorObjects[i].prefabs[needNum].sceles.min, decorObjects[i].prefabs[needNum].sceles.max);
              scaleY = scaleX;
            }

            for(int n = 0; n < decorObjects[i].prefabs[needNum].NeedBone.Length; n++) {
              if(decorObjects[i].prefabs[needNum].NeedBone[n].Length <= slotList.Items[j].Data.Name.Length && slotList.Items[j].Data.Name.Substring(slotList.Items[j].Data.Name.Length - decorObjects[i].prefabs[needNum].NeedBone[n].Length) == decorObjects[i].prefabs[needNum].NeedBone[n]) {
                if(!decorObjectsCheckOnly) {
                  slotList.Items[j].A = 1;
                  allNeedsBone.Add(decorObjects[i].prefabs[needNum].NeedBone[n]);
                } else {
                  if(slotList.Items[j].A == 0)
                    fixedDecor = false;
                }
              }

            }

            for(int n = 0; n < decorObjects[i].prefabs[needNum].ExecBone.Length; n++) {

              if(!decorObjectsCheckOnly && allNeedsBone.Exists(x => x == decorObjects[i].prefabs[needNum].ExecBone[n])) {
                fixedDecor = false;
                continue;
              }

              if(decorObjects[i].prefabs[needNum].ExecBone[n].Length <= slotList.Items[j].Data.Name.Length && slotList.Items[j].Data.Name.Substring(slotList.Items[j].Data.Name.Length - decorObjects[i].prefabs[needNum].ExecBone[n].Length) == decorObjects[i].prefabs[needNum].ExecBone[n]) {
                if(!decorObjectsCheckOnly)
                  slotList.Items[j].A = 0;
                else {
                  if(slotList.Items[j].A > 0)
                    fixedDecor = false;
                }
              }

            }
          }


          if(fixedDecor) {
            decorObjects[i].prefabs[needNum].prefab.SetActive(true);
            decorObjects[i].prefabs[needNum].prefab.transform.localScale = new Vector3(scaleX, scaleY, 1);

            decorObjects[i].prefabs[needNum].prefab.GetComponent<BoneFollower>().boneName = decorObjects[i].prefabs[needNum].boneName;
            decorObjects[i].prefabs[needNum].prefab.GetComponent<BoneFollower>().Reset();
          }
        }
    }

    CheckLinks();

  }

  public void SetColor(Color newColor) {

    //foreach (Slot bon in skeletonRenderer.skeleton.slots) {
    for(int i = 0; i < slotList.Count; i++) {
      slotList.Items[i].R = newColor.r;
      slotList.Items[i].G = newColor.g;
      slotList.Items[i].B = newColor.b;
    }
  }


  void CheckDopActivate(string boneNames) {

    bool activeBone;

    //foreach (bonesLinksCheckers one in dopBonesLinksActive) {
    for(int j = 0; j < dopBonesLinksActive.Length; j++) {
      if(dopBonesLinksActive[j].boneName == boneNames) {
        activeBone = false;

        //foreach(Slot bon in skeletonRenderer.skeleton.slots) {
        for(int i = 0; i < slotList.Count; i++) {
          if(dopBonesLinksActive[j].boneName.Length <= slotList.Items[i].Data.Name.Length && slotList.Items[i].Data.Name.Substring(slotList.Items[i].Data.Name.Length - dopBonesLinksActive[j].boneName.Length) == dopBonesLinksActive[j].boneName && slotList.Items[i].A == 1) {
            activeBone = true;
          }
        }

        if(activeBone) {
          //foreach(Slot bon in skeletonRenderer.skeleton.slots) {
          for(int i = 0; i < slotList.Count; i++) {
						//foreach (string bonName in dopBonesLinksActive[j].boneNameLinks) {
							for(int n = 0; n < dopBonesLinksActive[j].boneNameLinks.Length; n++) {
								if(dopBonesLinksActive[j].boneNameLinks[n].Length <= slotList.Items[i].Data.Name.Length && slotList.Items[i].Data.Name.Substring(slotList.Items[i].Data.Name.Length - dopBonesLinksActive[j].boneNameLinks[n].Length) == dopBonesLinksActive[j].boneNameLinks[n] && slotList.Items[i].A <= 0) {
									slotList.Items[i].A = 1;
									CheckDopActivate(dopBonesLinksActive[j].boneNameLinks[n]);
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
  
  void CheckLinks() {

    if(bonesLinksChecker.Length == 0)
      return;
    bool deactive = true;
    for(int j = 0; j < bonesLinksChecker.Length; j++) {

      deactive = true;

      if(bonesLinksChecker[j].full) {
        deactive = false;
        for(int i = 0; i < slotList.Count; i++) {
          for(int n = 0; n < bonesLinksChecker[j].boneNameLinks.Length; n++) {
            if(bonesLinksChecker[j].boneNameLinks[n].Length <= slotList.Items[i].Data.Name.Length && slotList.Items[i].Data.Name.Substring(slotList.Items[i].Data.Name.Length - bonesLinksChecker[j].boneNameLinks[n].Length) == bonesLinksChecker[j].boneNameLinks[n] && slotList.Items[i].A <= 0) {
              deactive = true;
            }
          }
        }
      } else {
        for(int i = 0; i < slotList.Count; i++) {
          for(int n = 0; n < bonesLinksChecker[j].boneNameLinks.Length; n++) {
            if(bonesLinksChecker[j].boneNameLinks[n].Length <= slotList.Items[i].Data.Name.Length && slotList.Items[i].Data.Name.Substring(slotList.Items[i].Data.Name.Length - bonesLinksChecker[j].boneNameLinks[n].Length) == bonesLinksChecker[j].boneNameLinks[n] && slotList.Items[i].A > 0) {
              deactive = false;
            }
          }
        }
      }


      if(deactive) {
        for(int i = 0; i < slotList.Count; i++) {
          if(bonesLinksChecker[j].boneName.Length <= slotList.Items[i].Data.Name.Length && slotList.Items[i].Data.Name.Substring(slotList.Items[i].Data.Name.Length - bonesLinksChecker[j].boneName.Length) == bonesLinksChecker[j].boneName) {
            slotList.Items[i].A = 0;
          }
        }
      }

    }
  }

  public GameObject torchPref;                                // Префаб факела
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

  public void DecrementDecoration(int valueDecrement) {

    for(int i = 0; i < randomActiveInCondition.Length; i++) {
      randomActiveInCondition[i].distance.min = (randomActiveInCondition[i].distance.min - valueDecrement < 0 ? 0 : randomActiveInCondition[i].distance.min - valueDecrement);
      randomActiveInCondition[i].distance.max = (randomActiveInCondition[i].distance.max - valueDecrement < 0 ? 0 : randomActiveInCondition[i].distance.max - valueDecrement);
    }

  }



}
