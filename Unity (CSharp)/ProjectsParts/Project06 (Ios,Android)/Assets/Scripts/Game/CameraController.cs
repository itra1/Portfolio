using UnityEngine;
using System.Collections;

public class CameraController : Singleton<CameraController> {
  public static Vector3 leftPointWorld;
  public static Vector3 rightPointWorld;
  public static Vector3 leftTopPointWorld;
  public static Vector3 middleTopPointWorld;
  public static Vector3 leftBottom;
  public static Vector3 rightTop;
	public Camera fog;
  
  public static float distanseX {
    get {
      return Vector3.Distance(leftPointWorld , rightPointWorld);
    }
  }
  public static float distanseY {
    get {
      return Vector3.Distance(leftPointWorld , leftTopPointWorld) * 2;
    }
  }

  public static Vector3 rightPoint {
    get {
      return rightPointWorld;
    }
  }

  public static Vector3 leftPointX {
    get {
      return leftPointWorld;
    }
  }
  public static Vector3 leftTopPointWorldX {
    get {
      return leftTopPointWorld;
    }
  }

  public static Vector3 middleTopPointWorldX {
    get {
      return middleTopPointWorld;
    }
  }
  
	protected override void Awake() {
    base.Awake();
		CalcGameView();
  }

  void Start() {
    ChangeCameraSize();
    GameManager.OnChangeFullScreen += ChangeCameraSizeInvoke;
  }

  protected override void OnDestroy() {
    base.OnDestroy();
    GameManager.OnChangeFullScreen -= ChangeCameraSizeInvoke;

  }

  void CalcGameView() {
    leftBottom = Camera.main.ViewportToWorldPoint(new Vector3(0f , 0f , 10f));
    rightTop = Camera.main.ViewportToWorldPoint(new Vector3(1f , 1f , 10f));
    leftPointWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f , 0.5f , 10f));
    leftTopPointWorld = Camera.main.ViewportToWorldPoint(new Vector3(0f , 1f , 10f));
    middleTopPointWorld = Camera.main.ViewportToWorldPoint(new Vector3(0.5f , 1f , 10f));
    rightPointWorld = Camera.main.ViewportToWorldPoint(new Vector3(1f , 0.5f , 10f));
  }

  int pixelHeight;
  int pixelWidth;

  void ChangeCameraSizeInvoke() {
    Invoke("ChangeCameraSize", 1);
  }


  void ChangeCameraSize() {
    
    pixelHeight = Screen.height;
    pixelWidth = Screen.width;
    //Debug.Log(pixelHeight + " : " + pixelWidth + " : " + ((float)pixelHeight / (float)pixelWidth));
    CalcGameView();

    if((float)pixelHeight / (float)pixelWidth < 0.55f) {
      Camera.main.orthographicSize = 7.5f;
		} else {
      Camera.main.orthographicSize = 7.5f / (rightTop.x * 2) * 24;
		}
		if (fog != null)
			fog.orthographicSize = Camera.main.orthographicSize;

	}

  void FixedUpdate() {
    if (pixelWidth != Screen.width || pixelHeight != Screen.height)
      ChangeCameraSize();
  }
	
}
