using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(PlaneAnim))]
public class PlaneAnimEditor : Editor {

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if(GUILayout.Button("Set Position")) {
			((PlaneAnim)target).SetPosition();
		}

	}

}

#endif
[System.Serializable]
public class PlaneAnim : MonoBehaviour {

	[Range(0,1)]
	public float percent;

	public Action<int> OnFrame;
	public BezierCurve bezierCurve;
	public Transform planeInst;

	Vector3 lastPosition;

	bool isDestroy;

	public Transform mainCamera;
	Camera cameraComp;
	public Vector3 mainCameraPosition;
	public float cameraOrthographicSize;
	private Animation _animation;

	void Start() {
		_animation = GetComponent<Animation>();
		lastPosition = planeInst.transform.position;
		isDestroy = false;
		cameraComp = Camera.main;
		mainCamera = Camera.main.transform;
	}

	private void Update() {

		if(!isDestroy) {
			SetPosition();
			mainCamera.transform.position = new Vector3(planeInst.transform.position.x, planeInst.transform.position.y, mainCamera.transform.position.z);
			cameraComp.orthographicSize = cameraOrthographicSize;
		}

    CameraController.Instance.fog.GetComponent<FoW.FogOfWar>().Draw();

	}

	public void SetPosition() {
		planeInst.transform.position = bezierCurve.GetPointAt(percent);
		if(lastPosition != planeInst.transform.position) {
			Vector3 moveVector = planeInst.transform.position-lastPosition;
			float angle = Vector3.Angle(Vector3.up,moveVector);
			planeInst.localEulerAngles = new Vector3(0, 0, angle * -Mathf.Sign(moveVector.x));
			lastPosition = planeInst.transform.position;
		}
	}

	private void OnGUI() {
		if(!Application.isPlaying) {
			SetPosition();
		}
	}

	public void PlaneDestroy() {
		isDestroy = true;
		planeInst.transform.localEulerAngles = Vector3.zero;
	}

	public void Step(int step) {
		if (OnFrame != null) OnFrame(step);
		_animation["Plane"].speed = 0;
		//_animation.clip.frameRate
	}

	public void Play() {
		_animation["Plane"].speed = 1;
	}

}
