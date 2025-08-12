using UnityEngine;
using UnityEngine.UI;
using System.Collections;


/// <summary>
/// Контроллер больших ворот
/// </summary>
public class GateLagre : MonoBehaviour {

	[SerializeField]
	GatesLagreController parent;

	[SerializeField]
	GameObject keysPanel;
	Vector3 keyPanelStartPosition;
	[SerializeField]
	Transform cameraObject;
	[SerializeField]
	GameObject[] padLocks;
	[SerializeField]
	GameObject[] allchain;

	[SerializeField]
	int gateNum;

	int useKeys;
	int openGate;
	float speedCameraY = 0;

	bool isAllMuve;
	bool cameraMuve;

	Vector3 startCameraPosition;

	[SerializeField]
	float minYheight;
	float diffDisplay;
	bool _showOpenPackLock;
	float moveSpeed;

	[SerializeField]
	GameObject waitPanel;
	[SerializeField]
	GameObject shakleParticles;

	void OnEnable() {
		isSpeedy = false;
		shakleParticles.SetActive(false);
		waitPanel.SetActive(false);
		keyPanelStartPosition = keysPanel.transform.position;
		startCameraPosition = transform.localPosition;
		GatesLagreController.OnTapSpeedy += OnTapSpeedy;
		CalcSpeed();
		InitCounts();
	}

	void OnDisable() {
		GatesLagreController.OnTapSpeedy -= OnTapSpeedy;
	}

	bool isSpeedy;
	void OnTapSpeedy() {
		isSpeedy = true;

		if(IsInvoking("SetMuveCamera")) {
			CancelInvoke("SetMuveCamera");
			SetMuveCamera();
		}
		speedCameraY = -24;
	}


	void CalcSpeed() {
		Camera cam = cameraObject.GetComponent<Camera>();
		moveSpeed = (cam.ViewportToWorldPoint(new Vector3(0, 1, 10)).y - cam.ViewportToWorldPoint(new Vector3(0, 0, 10)).y) / Camera.main.pixelWidth;
	}

	public void ShowAnim(bool flag = true) {
		_showOpenPackLock = flag;
		SetOpenGates();
		if(flag)
			Invoke("SetMuveCamera", 1);
	}

	void Update() {

		if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved && !cameraMuve && !_showOpenPackLock) {
			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
			if(Mathf.Abs(touchDeltaPosition.x) < Mathf.Abs(touchDeltaPosition.y) && Mathf.Abs(touchDeltaPosition.y) > 1f) {
				TapMove(-touchDeltaPosition.y);

				if(keyPanelStartPosition.y > cameraObject.position.y + (diffDisplay / 6 * 5))
					keysPanel.transform.position = new Vector3(keysPanel.transform.position.x, cameraObject.position.y + (diffDisplay / 6 * 5), keysPanel.transform.position.z);
				else
					keysPanel.transform.position = keyPanelStartPosition;
			}
		}

		if(cameraMuve) {
			if(speedCameraY > -6) speedCameraY += -6 * Time.deltaTime;

			if(cameraObject.position.y > padLocks[useKeys - 1].transform.position.y && cameraObject.position.y > minYheight + diffDisplay)
				cameraObject.position += new Vector3(0, speedCameraY, 0) * Time.deltaTime;
			else {
				cameraMuve = false;
				if(isSpeedy)
					StartPadLockAnim();
				else
					Invoke("StartPadLockAnim", 0.5f);
			}

			if(keyPanelStartPosition.y > cameraObject.position.y + (diffDisplay / 6 * 5))
				keysPanel.transform.position = new Vector3(keysPanel.transform.position.x, cameraObject.position.y + (diffDisplay / 6 * 5), keysPanel.transform.position.z);
			else
				keysPanel.transform.position = keyPanelStartPosition;

		}
		if(isAllMuve) {
			if(speedCameraY > -24) speedCameraY += -24 * Time.deltaTime;
			if(padLocks[0].transform.position.y > minYheight - diffDisplay) {
				foreach(GameObject one in padLocks)
					one.transform.position += new Vector3(0, speedCameraY, 0) * Time.deltaTime;
				//foreach (GameObject one in allchain)
				//    one.transform.position += new Vector3(0, speedCameraY, 0) * Time.deltaTime;
			} else {
				isAllMuve = false;
				Invoke("ClosePanel", 1);
			}
		}
	}

	void InitCounts() {

		useKeys = UserManager.Instance.keys;
		openGate = PlayerPrefs.GetInt("openGate", 0);
#if UNITY_EDITOR
		//useKeys = 15;
#endif
		CheckGate(ref useKeys, ref openGate);

		Vector3 pointCenter = cameraObject.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f,0.5f,10));
		Vector3 pointDown =  cameraObject.GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f,0f,10));
		diffDisplay = Mathf.Abs(pointDown.y - pointCenter.y);
	}

	void SetOpenGates() {
		if(openGate < gateNum) return;

		if(_showOpenPackLock) {
			for(int i = 0; i < useKeys - 1; i++) padLocks[i].GetComponent<Animator>().SetTrigger("open");
		} else
			for(int i = 0; i < useKeys; i++) padLocks[i].GetComponent<Animator>().SetTrigger("open");

	}

	void CheckGate(ref int useKeys, ref int openGate) {
		if(useKeys > parent.needsKeys[(int)openGate] && openGate != parent.needsKeys.Length) {
			useKeys -= parent.needsKeys[(int)openGate];
			openGate++;
			CheckGate(ref useKeys, ref openGate);
		}
	}

	void SetMuveCamera() {
		cameraMuve = true;
	}

	void StartPadLockAnim() {

		if(_showOpenPackLock) {
			padLocks[useKeys - 1].GetComponent<Animator>().SetBool("speedy", isSpeedy);
			padLocks[useKeys - 1].GetComponent<Animator>().SetTrigger("openAnim");
			if(isSpeedy)
				Invoke("AllMuve", 1);
			else
				Invoke("AllMuve", 4);
		}
	}

	/// <summary>
	/// Запускаем процесс падения объектов или просто закрываем панель
	/// </summary>
	void AllMuve() {
		if(useKeys == padLocks.Length) {
			speedCameraY = 0;
			isAllMuve = true;
			parent.PlayChainDown();
			waitPanel.SetActive(true);
			waitPanel.GetComponent<Image>().color = new Color(1, 1, 1, 1);
			LightTween.SpriteColorTo(waitPanel.GetComponent<Image>(), new Color(1, 1, 1, 0), 0.5f);
			foreach(GameObject one in allchain) {
				GameObject clone = Instantiate(shakleParticles);
				clone.transform.position = one.transform.position;
				clone.SetActive(true);
			}
			foreach(GameObject one in allchain)
				one.SetActive(false);
		} else
			Invoke("ClosePanel", 1);
	}

	void ClosePanel() {
		parent.ButtonClose();
	}

	void TapMove(float deltaY) {

		if(cameraObject.position.y + deltaY * moveSpeed <= minYheight + diffDisplay && deltaY < 0)
			cameraObject.position = new Vector3(cameraObject.position.x, minYheight + diffDisplay, cameraObject.position.z);
		else if(cameraObject.position.y + deltaY * moveSpeed >= startCameraPosition.y && deltaY > 0)
			cameraObject.position = new Vector3(cameraObject.position.x, startCameraPosition.y, cameraObject.position.z);
		else
			cameraObject.position = new Vector3(cameraObject.position.x, cameraObject.position.y + deltaY * moveSpeed, cameraObject.position.z);
	}

}
