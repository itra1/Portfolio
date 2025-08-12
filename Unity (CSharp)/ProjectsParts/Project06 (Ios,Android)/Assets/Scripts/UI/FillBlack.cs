using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(FillBlack))]
public class FullBlackEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Round Open")) {
			((FillBlack)target).PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.open, Vector2.zero);
		}

		if (GUILayout.Button("Round Close")) {
			((FillBlack)target).PlayAnim(FillBlack.AnimType.round, FillBlack.AnimVecor.close, Vector2.zero);
		}

		if (GUILayout.Button("Fill Open")) {
			((FillBlack)target).PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.open, Vector2.zero);
		}

		if (GUILayout.Button("Fill Close")) {
			((FillBlack)target).PlayAnim(FillBlack.AnimType.full, FillBlack.AnimVecor.close, Vector2.zero);
		}

	}

}

#endif

public class FillBlack : UiDialog {

	public enum AnimType {
		round,
		full
	}

	public enum AnimVecor {
		close,
		open
	}

	AnimType animType;
	AnimVecor animVector;

	System.Action OnCmplited;

	System.Action UpdateAnim;
	bool isUnscale;

	Material uiBlack;
	Vector3 targetCenter;
	Image bg;

	public Material roundMaterial;
	public Material fullMaterial;

	float needValue;
	float thisValue;
	float increment;

	float _localScale;

	Vector4 pointValue;

	private float koefscale {
		get {
			if (Camera.main.orthographic)
				return Camera.main.orthographicSize / 150;
			else
				return _localScale;
		}
	}

	void OnEnable() {
		_localScale = transform.parent.localScale.x;
		bg = GetComponent<Image>();
		uiBlack = GetComponent<Image>().material;
		uiBlack.SetVector("_Point", new Vector4(transform.position.x, transform.position.y, transform.position.z, 100));
	}

	void Update() {
		if (UpdateAnim != null)
			UpdateAnim();
	}

	public void PlayAnim(AnimType animType, AnimVecor animVector) {
		PlayAnim(animType, animVector, Vector3.zero, false, null);
	}
	public void PlayAnim(AnimType animType, AnimVecor animVector, System.Action OnCmplited = null) {
		PlayAnim(animType, animVector, Vector3.zero, false, OnCmplited);
	}

	public void PlayAnim(AnimType animType, AnimVecor animVector, Vector3 targetCenter, bool isUnscale = false, System.Action OnCmplited = null) {

		this.isUnscale = isUnscale;
		this.animType = animType;
		this.animVector = animVector;
		this.OnCmplited = OnCmplited;
		if (targetCenter != Vector3.zero)
			this.targetCenter = targetCenter;
		else {
			try {
				this.targetCenter = new Vector3(GuiCamera.instance.transform.position.x, GuiCamera.instance.transform.position.y, transform.position.z);
			} catch {
				this.targetCenter = new Vector3(CameraController.Instance.transform.position.x, CameraController.Instance.transform.position.y, transform.position.z);
			}
		}

		if (this.animType == AnimType.full && this.animVector == AnimVecor.close) {
			bg.color = new Color(0, 0, 0, 0);
			bg.material = fullMaterial;
			UpdateAnim = AnimFillClose;
		} else if (this.animType == AnimType.full && this.animVector == AnimVecor.open) {
			bg.color = new Color(0, 0, 0, 1);
			bg.material = fullMaterial;
			UpdateAnim = AnimFillOpen;
		} else if (this.animType == AnimType.round && this.animVector == AnimVecor.close) {
			needValue = 0;
			increment = 10000f * koefscale;
			thisValue = 2500f * koefscale;
			bg.material = roundMaterial;
			UpdateAnim = AnimRoundClose;
		} else if (this.animType == AnimType.round && this.animVector == AnimVecor.open) {
			needValue = 5000f * koefscale;
			increment = 12f * koefscale;
			thisValue = 0;
			bg.material = roundMaterial;
			UpdateAnim = AnimRoundOpen;
		}
	}

	public void CloseScene(Vector3 targets, System.Action OnCmplited = null) {
		CloseScene(targets, 30, 50, OnCmplited);
	}

	// Сокрытие изображения
	public void CloseScene(Vector3 targets, float startValue = 30, float incrementValue = 50, System.Action OnCmplited = null) {
		targetCenter = targets;
		needValue = 0;
		increment = incrementValue;
		thisValue = startValue;
		UpdateAnim = AnimRoundClose;
		this.OnCmplited = OnCmplited;
	}

	public void OpenScene(Vector3 targets, System.Action OnCmplited = null) {
		OpenScene(targets, 0, 0.3f, OnCmplited);
	}

	public void OpenScene(Vector3 targets, float startValue = 0, float incrementValue = 0.3f, System.Action OnCmplited = null) {

		uiBlack.SetVector("_Point", new Vector4(transform.position.x, transform.position.y, transform.position.z, 0));

		targetCenter = targets;
		needValue = 50;
		increment = incrementValue;
		thisValue = startValue;
		UpdateAnim = AnimRoundOpen;
		this.OnCmplited = OnCmplited;
	}

	void AnimRoundOpen() {
		if (thisValue < needValue) {

			if (targetCenter == Vector3.zero) {
				pointValue = transform.position;
			} else {
				pointValue = targetCenter;
			}

			increment += (increment * 5) * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

			float tempw = thisValue + increment * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

			if (tempw > needValue)
				tempw = needValue;
			thisValue = tempw;
			pointValue.w = thisValue;

			uiBlack.SetVector("_Point", pointValue);
			if (thisValue >= needValue) {
				UpdateAnim = null;
				if (OnCmplited != null) {
					OnCmplited();
					OnCmplited = null;
				}
			}
		}
	}


	void AnimRoundClose() {
		if (thisValue > needValue) {

			if (targetCenter == Vector3.zero) {
				pointValue = transform.position;
			} else {
				pointValue = targetCenter;
			}

			increment -= 2f * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

			float tempw = thisValue - increment * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime);

			if (tempw < 0)
				tempw = 0;
			thisValue = tempw;
			pointValue.w = thisValue;

			uiBlack.SetVector("_Point", pointValue);

			if (thisValue <= needValue) {
				UpdateAnim = null;
				if (OnCmplited != null) {
					OnCmplited();
					OnCmplited = null;
				}
			}
		}
	}

	void AnimFillClose() {

		bg.color = new Color(0, 0, 0, bg.color.a + 1 * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime));

		if (bg.color.a >= 1) {
			if (OnCmplited != null)
				OnCmplited();
			UpdateAnim = null;
		}
	}

	void AnimFillOpen() {

		bg.color = new Color(0, 0, 0, bg.color.a - 1 * (isUnscale ? Time.unscaledDeltaTime : Time.deltaTime));

		if (bg.color.a <= 0) {
			if (OnCmplited != null)
				OnCmplited();
			UpdateAnim = null;
		}
	}

}
