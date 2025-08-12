using UnityEngine;
using System.Collections;

/// <summary>
/// Родитель камеры
/// </summary>
public class ParentCamera : Singleton<ParentCamera> {
  
	public Transform parent;
	private Rigidbody2D rb;

	public Animation _animComponent;
	private Vector3 velocity;
	private bool _cameraStop;
  
	public static bool CameraStop {
		get { return Instance == null ? false : Instance._cameraStop; }
		set { if(Instance != null) Instance._cameraStop = value; RunnerController.cameraStop = value; }
	}

	protected override void Awake() {
    base.Awake();
		if(GameManager.isIPad) {
			parent.position += new Vector3(0, +1, -4);
			transform.GetComponentInChildren<CameraController>().RecalcDiff();
		}
	}
	
	private void Start() {
		transform.localPosition = Vector3.zero;
		RunnerController.OnChangeRunnerPhase += ChangePhase;
		rb = GetComponent<Rigidbody2D>();
	}

	private void OnEnable() {
		transform.localPosition = Vector3.zero;
	}

	private void OnDisable() {
		transform.localPosition = Vector3.zero;
	}

	protected override void OnDestroy() {
    base.OnDestroy();
		RunnerController.OnChangeRunnerPhase -= ChangePhase;
	}

	private void ChangePhase(RunnerPhase newPhase) {
		if(newPhase == RunnerPhase.boost)
			_animComponent.Play("Tremor");
		else
			_animComponent.Stop();
		
		if(newPhase == RunnerPhase.start)
			transform.localPosition = Vector3.zero;
		
	}

	private void Update() {

		if (RunnerController.Instance.runnerPhase == RunnerPhase.start) return;

		velocity.x = RunnerController.RunSpeed;

		if (!_cameraStop)
			rb.velocity = velocity;
	}
	
	//public void RecalcCamera() {
	//	bool zoom = _animator.GetBool("zoomFirst");

	//	if(!zoom) {
	//		CameraController.Instance.RecalcDiff();
	//	}
	//}

}
