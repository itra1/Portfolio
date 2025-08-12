using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegionsSurvival: Regions {

  private float? nextChangeDistance = 0;                               // Дистанция для изменения декораций

  public override void Init() {
    base.Init();
    GetNextChangeRegion();
  }

  void GetNextChangeRegion() {

    if (GameManager.Instance.mapRun.Count <= 0) {
      nextChangeDistance = null;
      return;
    }

    MapRegion mr = GameManager.Instance.mapRun.Find(x => x.distanceStart >= RunnerController.playerDistantion);

    if (mr == null) {
      nextChangeDistance = null;
      return;
    }

    nextChangeDistance = mr.distanceStart;
    
  }

  private void Update() {
    if (nextChangeDistance != null)
      RegionUpdate();
  }

  public void RegionUpdate() {
    if (nextChangeDistance <= RunnerController.playerDistantion) {
      SetRegion();
      GetNextChangeRegion();
    }
  }


  /// <summary>
  /// Применение перехода
  /// </summary>
  public void NowChange(bool force = false) {

    if (cryptoBg != null && (type == RegionType.Crypt || type == RegionType.Forest)) {
      cryptoBg.SetActive(true);
      cryptoBg.transform.position = new Vector3(CameraController.displayDiff.transform.position.x, cryptoBg.transform.position.y, cryptoBg.transform.position.y);
      RunSpawner.Instance.StartCoroutine(AlphaChange(force));
    }

    if (type == RegionType.Jungle && cryptoBgFull.activeInHierarchy) {
      cryptoBg.SetActive(true);
      cryptoBg.transform.position = new Vector3(CameraController.displayDiff.transform.position.x, cryptoBg.transform.position.y, cryptoBg.transform.position.y);
      RunSpawner.Instance.StartCoroutine(AlphaChangeDec(force));
    }

  }
  /// <summary>
  /// Переход на голубой фон
  /// </summary>
  /// <returns></returns>
  IEnumerator AlphaChange(bool force = false) {
    SpriteRenderer spr = cryptoBg.GetComponent<SpriteRenderer>();

    if (!force) {
      spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0);
      while (spr.color.a < 1) {
        yield return new WaitForSeconds(0.1f);
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, spr.color.a + 0.01f);
      }
    }
    cryptoBgFull.SetActive(true);
    cryptoBgFull.transform.position = new Vector3(CameraController.displayDiff.transform.position.x, cryptoBg.transform.position.y, cryptoBg.transform.position.y);
    if (!force) {
      while (spr.color.a > 0) {
        yield return new WaitForSeconds(0.1f);
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, spr.color.a - 0.01f);
      }
    }
    cryptoBg.SetActive(false);

  }

  /// <summary>
  /// Обратный переход 
  /// </summary>
  /// <returns></returns>
  IEnumerator AlphaChangeDec(bool force = false) {

    SpriteRenderer spr = cryptoBg.GetComponent<SpriteRenderer>();
    if (!force) {
      spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, 0);
      while (spr.color.a < 1) {
        yield return new WaitForSeconds(0.1f);
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, spr.color.a + 0.01f);
      }
    }
    cryptoBgFull.SetActive(false);
    if (!force) {
      while (spr.color.a > 0) {
        yield return new WaitForSeconds(0.1f);
        spr.color = new Color(spr.color.r, spr.color.g, spr.color.b, spr.color.a - 0.03f);
      }
    }
    cryptoBg.SetActive(false);
  }


  /// <summary>
  /// Инициализация стартового реиона
  /// </summary>
  protected override void SetRegion() {
    base.SetRegion();

    for (int i = 0; i < GameManager.Instance.mapRun.Count; i++) {
      if (GameManager.Instance.mapRun[i].distanceStart == nextChangeDistance.Value) {
        type = GameManager.Instance.mapRun[i].mapType;
        NowChange(true);
      }
    }

  }


}
