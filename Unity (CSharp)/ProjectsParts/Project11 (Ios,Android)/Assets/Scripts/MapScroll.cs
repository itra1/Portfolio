using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroll: MonoBehaviour {

  [HideInInspector]
  public float maxPosition;
  [HideInInspector]
  public float heightPixel;
  [HideInInspector]
  public float visibleHeight;

  private bool isScreenTap = false;
  private float screenDragSize = 0;
  private bool isBackBorder = false;

  public Transform startTransform;

  public Transform cameraTransform;

  private float elasticSpeed;

  private float scrollMin {
    get {
      return startTransform.position.y + (visibleHeight / 2);
    }
  }

  private float scrollMax {
    get {
      return maxPosition + 0.5f;
    }
  }

  private void Start() {
    screenDragSize = 0;
  }

  private void Update() {
    ElasticScroll();
  }

  public void SetPosition(float yPos) {
    ScrollCamera(yPos - cameraTransform.transform.position.y);
  }

  private float nextPos = 0;
  private float speedKoeff = 1;

  public void ScrollCamera(float deltaY) {
    screenDragSize = deltaY;

    if (isScreenTap) {

      if (screenDragSize < 0) {
        if (cameraTransform.position.y > scrollMax)
          speedKoeff = Mathf.Lerp(1, 0.1f, (cameraTransform.position.y - scrollMax)/1.5f);
        else {
          speedKoeff = 1;
        }
      }

      if (screenDragSize > 0) {
        if (cameraTransform.position.y < scrollMin)
          speedKoeff = Mathf.Lerp(1, 0.1f, (scrollMin - cameraTransform.position.y)/ 1.5f);
        else
          speedKoeff = 1;
      }

    }

    nextPos = cameraTransform.position.y - (deltaY * heightPixel * speedKoeff);
    cameraTransform.position = new Vector3(0, nextPos, cameraTransform.position.z);
    
    if (cameraTransform.position.y >= maxPosition-4) {
      Debug.Log((cameraTransform.position.y - maxPosition));
      MapManager.Instance.menu.SetCloudDiff((cameraTransform.position.y - (maxPosition-4)) / 1f);
    } else {
      MapManager.Instance.menu.SetCloudDiff(0);
    }
  }

  public void ScreenTapDown() {
    isScreenTap = true;
    isOutBound = false;
  }

  public void ScreenTapUp() {
    isScreenTap = false;
    elasticSpeed = Mathf.Abs(screenDragSize * 2);
  }

  private bool isOutBound = false;

  private void ElasticScroll() {

    if (isScreenTap) return;

    if(cameraTransform.position.y > scrollMax) {
      screenDragSize = 350;
      isOutBound = true;
    } else if(cameraTransform.position.y < scrollMin) {
      screenDragSize = -350;
      isOutBound = true;
    } else {

      if (screenDragSize >= 0) {

        screenDragSize -= elasticSpeed * Time.deltaTime;
        if (screenDragSize < 0)
          screenDragSize = 0;
      } else {
        screenDragSize += elasticSpeed * Time.deltaTime;
        if (screenDragSize > 0)
          screenDragSize = 0;

      }

    }

    if (screenDragSize != 0)
      ScrollCamera(screenDragSize);

    if (isOutBound) {
      if(screenDragSize > 0 && cameraTransform.position.y < scrollMax) {
        screenDragSize = 0;
        isOutBound = false;
        cameraTransform.position = new Vector3(cameraTransform.position.x, scrollMax-0.1f, cameraTransform.position.z);
      } else if (screenDragSize < 0 && cameraTransform.position.y > scrollMin) {
        screenDragSize = 0;
        isOutBound = false;
        cameraTransform.position = new Vector3(cameraTransform.position.x, scrollMin+0.1f, cameraTransform.position.z);
      }
    } else {

      if (screenDragSize > 0 && cameraTransform.position.y > scrollMax) {
        screenDragSize = 0;
        isOutBound = false;
        cameraTransform.position = new Vector3(cameraTransform.position.x, scrollMax - 0.1f, cameraTransform.position.z);
      } else if (screenDragSize > 0 && cameraTransform.position.y < scrollMin) {
        screenDragSize = 0;
        isOutBound = false;
        cameraTransform.position = new Vector3(cameraTransform.position.x, scrollMin + 0.1f, cameraTransform.position.z);
      }

    }

  }

}
