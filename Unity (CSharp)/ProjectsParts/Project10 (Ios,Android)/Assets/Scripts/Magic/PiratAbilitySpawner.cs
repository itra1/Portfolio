using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiratAbilitySpawner: AbilitySpawner {

  public override bool Activate() {
    if (isActive) return false;

    Invoke(Step1, 1.5f);

    return base.Activate();


  }

  //private void Update() {

  //  if (!isActive) return;

  //  if (step == 1) {
  //    // Звуки выстрелов
  //    timeStep = Time.time + 1.5f;
  //    step++;
  //  }
  //  if (step == 2 && timeStep <= Time.time) {
  //    Vector3 pos = new Vector3(CameraController.displayDiff.rightDif(1.5f), CameraController.displayDiff.transform.position.y, 0);
  //    GameObject obj = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
  //    obj.transform.parent = transform;
  //    obj.GetComponent<PiratController>().magic = this;
  //    obj.SetActive(true);
  //    step++;
  //    timeStep = Time.time + Random.Range(0.1f, 0.4f);
  //  }
  //}

  private void Step1() {
    Vector3 pos = new Vector3(CameraController.displayDiff.rightDif(1.5f), CameraController.displayDiff.transform.position.y, 0);
    GameObject obj = Instantiate(prefab, pos, Quaternion.identity) as GameObject;
    obj.transform.parent = transform;
    obj.GetComponent<PiratController>().magic = this;
    obj.SetActive(true);
    step++;
    timeStep = Time.time + Random.Range(0.1f, 0.4f);
  }

  //private void Step2() {

  //}
  
  
  /// <summary>
   /// Визуализация появления пиратов
   /// </summary>
   /// <param name="flag"></param>
  public void PiratEffect(bool flag) {
    CameraController.Instance.GetComponent<Animator>().SetBool("pirats", flag);
  }



}
