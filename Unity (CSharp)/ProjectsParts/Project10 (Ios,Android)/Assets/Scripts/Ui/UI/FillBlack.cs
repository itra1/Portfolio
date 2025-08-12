using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FillBlack))]
public class FullBlackEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

    if (GUILayout.Button("Round Swich")) {
      ((FillBlack)target).PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.swich, Vector2.zero, () => { }, () => { }, () => { });
    }

    if (GUILayout.Button("Round Open")) {
			((FillBlack)target).PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.open, Vector2.zero, ()=> { }, () => { }, () => { });
		}

		if (GUILayout.Button("Round Close")) {
			((FillBlack)target).PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.close, Vector2.zero, () => { }, () => { }, () => { });
    }

    if (GUILayout.Button("Fill Swich")) {
      ((FillBlack)target).PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.swich, Vector2.zero, () => { }, () => { }, () => { });
    }

    if (GUILayout.Button("Fill Open")) {
			((FillBlack)target).PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.open, Vector2.zero, () => { }, () => { }, () => { });
		}

		if (GUILayout.Button("Fill Close")) {
			((FillBlack)target).PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.close, Vector2.zero, () => { }, () => { }, () => { });
		}

	}

}

#endif

public class FillBlack : Singleton<FillBlack> {

  public Image roundImage;
  public Image fillImage;
  public Material roundMaterial;

  public enum AnimType {
		round,
		full
	}

	public enum AnimVecor {
		close,
		open,
    swich
	}

  private int maxSize {
    get {

      return Screen.height > 1500
           ? 500000
           : 300000;
    }
  }

  public void PlayAnim(AnimType animType, AnimVecor animVector, Vector3 targetCenter, Action OnFullHide = null, Action OnStart = null, Action OnEnd = null, bool isUnscale = false) {

    StopAllCoroutines();

    switch (animType) {
      case AnimType.full: {

        switch (animVector) {
          case AnimVecor.swich: {
            StartCoroutine(FillSwitch(OnFullHide, OnStart, OnEnd));
            break;
          }
          case AnimVecor.close: {
            StartCoroutine(FillClose(OnFullHide));
            break;
          }
          case AnimVecor.open: {
            StartCoroutine(FillOpen(OnFullHide));
            break;
          }
        }

        break;
      }
      case AnimType.round: {
        switch (animVector) {
          case AnimVecor.swich: {
            StartCoroutine(RoundSwitch(OnFullHide, OnStart, OnEnd));
            break;
          }
          case AnimVecor.close: {
            StartCoroutine(RoundClose(OnFullHide));
            break;
          }
          case AnimVecor.open: {
            StartCoroutine(RoundOpen(OnFullHide));
            break;
          }
        }
        break;
      }
    }
    
  }
  
	public void CloseScene(Vector3 targets, System.Action OnCmplited = null) {
		CloseScene(targets, 30, 50, OnCmplited);
	}

	// Сокрытие изображения
	public void CloseScene(Vector3 targets, float startValue = 30, float incrementValue = 50, System.Action OnCmplited = null) {
		//targetCenter = targets;
		//needValue = 0;
		//increment = incrementValue;
		//thisValue = startValue;
		//UpdateAnim = AnimRoundClose;
		//this.OnCmplited = OnCmplited;
	}

	public void OpenScene(Vector3 targets, System.Action OnCmplited = null) {
		OpenScene(targets, 0, 0.3f, OnCmplited);
	}

	public void OpenScene(Vector3 targets, float startValue = 0, float incrementValue = 0.3f, System.Action OnCmplited = null) {

		//uiBlack.SetVector("_Point", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0));

		//targetCenter = targets;
		//needValue = 50;
		//increment = incrementValue;
		//thisValue = startValue;
		//UpdateAnim = AnimRoundOpen;
		//this.OnCmplited = OnCmplited;
	}

	//void AnimRoundOpen() {
	//	if (thisValue < needValue) {

	//		if (targetCenter == Vector3.zero) {
	//			pointValue = transform.position;
	//		} else {
	//			pointValue = targetCenter;
	//		}

	//		increment += (increment * 5) * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

	//		float tempw = thisValue + increment * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

	//		if (tempw > 100)
	//			tempw = 100;
	//		thisValue = tempw;
	//		pointValue.w = thisValue;

	//		uiBlack.SetVector("_Point", pointValue);
	//		if (thisValue >= needValue) {
	//			UpdateAnim = null;
	//			if (OnCmplited != null) {
	//				OnCmplited();
	//				OnCmplited = null;
	//			}
	//		}
	//	}
	//}
	
	//void AnimRoundClose() {
	//	if (thisValue > needValue) {

	//		if (targetCenter == Vector3.zero) {
	//			pointValue = transform.position;
	//		} else {
	//			pointValue = targetCenter;
	//		}

	//		increment -= 2f * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

	//		float tempw = thisValue - increment * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

	//		if (tempw < 0)
	//			tempw = 0;
	//		thisValue = tempw;
	//		pointValue.w = thisValue;

	//		uiBlack.SetVector("_Point", pointValue);

	//		if (thisValue <= needValue) {
	//			UpdateAnim = null;
	//			if (OnCmplited != null) {
	//				OnCmplited();
	//				OnCmplited = null;
	//			}
	//		}
	//	}
	//}
  
  private IEnumerator RoundSwitch(Action OnFullHide, Action OnStart, Action OnEnd) {

    if (OnStart != null) OnStart();

    yield return RoundClose(OnFullHide);
    yield return new WaitForFixedUpdate();
    yield return new WaitForFixedUpdate();
    yield return RoundOpen(OnEnd);

  }

  private IEnumerator RoundClose(Action OnFullHide) {
    roundImage.gameObject.SetActive(true);

    Vector4 scale = new Vector4(0, 0, 0, maxSize);
    roundMaterial.SetVector("_Point", scale);

    while (scale.w > 0) {
      scale.w -= maxSize * Time.deltaTime * 4.5f;
      roundMaterial.SetVector("_Point", scale);
      yield return null;
    }

    scale.w = 0;
    roundMaterial.SetVector("_Point", scale);

    if (OnFullHide != null) OnFullHide();

  }

  private IEnumerator RoundOpen(Action OnEnd) {

    Vector4 scale = new Vector4(0, 0, 0, 0);
    while (scale.w < maxSize) {
      scale.w += maxSize * Time.deltaTime * 1.5f;
      roundMaterial.SetVector("_Point", scale);
      yield return null;
    }

    roundImage.gameObject.SetActive(false);
    if (OnEnd != null) OnEnd();

  }

  private IEnumerator FillSwitch(Action OnFullHide, Action OnStart, Action OnEnd, bool isUnscale = false) {

    if (OnStart != null) OnStart();

    yield return FillClose(OnFullHide, isUnscale);
    yield return new WaitForFixedUpdate();
    yield return new WaitForFixedUpdate();
    yield return FillOpen(OnEnd, isUnscale);

  }

  private IEnumerator FillClose(Action OnFullHide, bool isUnscale = false) {

    fillImage.color = new Color(0, 0, 0, 0);
    fillImage.gameObject.SetActive(true);

    while (fillImage.color.a < 1) {
      fillImage.color = new Color(0, 0, 0, fillImage.color.a + 1 * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime));
      yield return null;
    }

    fillImage.color = new Color(0, 0, 0, 1);

    if (OnFullHide != null) OnFullHide();

  }

  private IEnumerator FillOpen(Action OnEnd, bool isUnscale = false) {

    fillImage.gameObject.SetActive(true);
    fillImage.color = new Color(0, 0, 0, 1);

    while (fillImage.color.a > 0) {
      fillImage.color = new Color(0, 0, 0, fillImage.color.a - 1 * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime));
      yield return null;
    }

    fillImage.color = new Color(0, 0, 0, 0);
    fillImage.gameObject.SetActive(false);

    if (OnEnd != null) OnEnd();
  }

}
