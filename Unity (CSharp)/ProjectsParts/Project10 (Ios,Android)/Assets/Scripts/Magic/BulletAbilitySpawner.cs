using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletAbilitySpawner: AbilitySpawner {

  public GameObject prefabPirat;
  public AudioClip[] bombShootAudio;

  private int count = 0;
  public int targetBulletCount = 3;
  private bool bloomActive;
  private float nextTimeBullet;

  public override bool Activate() {
    if (isActive) return false;
    return base.Activate();
  }

  //private void Update() {

  //  if (!isActive) return;

  //  if (step == 1) {
      
  //  }

  //  if (step == 2 && timeStep - 1f <= Time.time) {
  //    // Световой эффект
  //    CameraController.Instance.GetComponent<Animator>().SetTrigger("magicBoom");
  //    step++;
  //  }

  //  if (step == 3 && timeStep <= Time.time) {
  //    if (count < targetBulletCount && nextTimeBullet < Time.time) {
  //      Vector3 pos = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right - 1f,
  //                                        CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top, 0);
  //      GameObject obj;


  //      if (Random.value <= 0.1f)
  //        obj = Instantiate(prefabPirat, pos, Quaternion.identity) as GameObject;
  //      else
  //        obj = Instantiate(prefab, pos, Quaternion.identity) as GameObject;

  //      obj.transform.parent = transform;
  //      obj.GetComponent<BulletController>().magic = this;
  //      count++;
  //      nextTimeBullet = Time.time + Random.Range(0, 0.5f);
  //    }

  //    if (count == targetBulletCount) {
  //      Deactive();
  //    }
  //  }
  //}

  private void Step1() {
    // Звуки выстрелов
    StartCoroutine(BobmAudio());
    count = 0;
    step++;
    bloomActive = false;
    Invoke(Step2, 0.5f);
    Invoke(()=> {
      StartCoroutine(Step3());
    }, 1.5f);
  }

  private void Step2() {
    CameraController.Instance.GetComponent<Animator>().SetTrigger("magicBoom");
  }

  private IEnumerator Step3() {

    while(count < targetBulletCount) {
      Vector3 pos = new Vector3(CameraController.displayDiff.transform.position.x + CameraController.displayDiff.right - 1f,
                                CameraController.displayDiff.transform.position.y + CameraController.displayDiff.top,
                                0);
      
      GameObject obj = (Random.value <= 0.1f)
                     ? obj = Instantiate(prefabPirat, pos, Quaternion.identity)
                     : obj = Instantiate(prefab, pos, Quaternion.identity);

      obj.transform.parent = transform;
      obj.GetComponent<BulletController>().magic = this;
      count++;
      nextTimeBullet = Random.Range(0, 0.5f);
      yield return new WaitForSeconds(nextTimeBullet);
    }
    Deactive();
  }

  /// <summary>
  /// Визуализация взрыва
  /// </summary>
  public void BloomBomb() {
    if (bloomActive) return;
    bloomActive = true;
    CameraController.Instance.GetComponent<Animator>().SetTrigger("magicBoom2");
  }

  /// <summary>
  /// Звуки выстрелов пушек
  /// </summary>
  /// <returns></returns>
  IEnumerator BobmAudio() {
    for (int i = 0; i < count; i++) {
      yield return new WaitForSeconds(Random.Range(0f, 0.5f));
      AudioManager.PlayEffect(bombShootAudio[Random.Range(0, bombShootAudio.Length)], AudioMixerTypes.runnerEffect);
    }
  }

}
