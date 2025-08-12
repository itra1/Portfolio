using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

public class FishBehaviour : MonoBehaviour {

	public SkeletonAnimation spineAnimation;
	[HideInInspector]
	public Rect rect;

	public Transform graphic;

	private void OnEnable() {
		spineAnimation.skeletonDataAsset = GraphicManager.Instance.link.fishList[Random.Range(0, GraphicManager.Instance.link.fishList.Count)];
		spineAnimation.Initialize(true);
		spineAnimation.OnRebuild += Init;
		regionCheck = StartCoroutine(CheckerRegion());
		CalcMove();
	}

	private void OnDisable() {
		spineAnimation.OnRebuild -= Init;

		if(regionCheck != null)
			StopCoroutine(regionCheck);
	}
	
	private void Init(SkeletonRenderer skl) {
		StartCoroutine(InitCon());
	}

	IEnumerator InitCon() {
		yield return null;
		spineAnimation.AnimationState.SetAnimation(0, "idle", true);
	}
	
	public void Click() {
		FishManager.Instance.ClickFish(this);
	}

	private void Update() {

		Move();

		if (moveVector != targetMoveVector) {
			IncrementSpeed();
		}

		if (CheckOutRegion()) {
			CalcMove();
		}

	}

	private Coroutine regionCheck;
	IEnumerator CheckerRegion() {
		while (true) {
			float timeWait = Random.Range(2f, 5f);
			yield return new WaitForSeconds(timeWait);
				CalcMove();
		}
	}

	private bool CheckOutRegion() {
		return  (transform.position.x < rect.x && targetMoveVector.x < 0)
						|| (transform.position.x > rect.x + rect.width && targetMoveVector.x > 0)
		       || (transform.position.y < rect.y && targetMoveVector.y < 0)
					 || (transform.position.y > rect.y + rect.height && targetMoveVector.y > 0);
	}

	private float _speedDef = 2f;
	private float minSpeed = 0.1f;
	private float maxSpeed = 1f;
	public Vector3 moveVector = Vector3.zero;
	public Vector3 targetMoveVector = Vector3.zero;
	private void Move() {
		transform.position += moveVector * Time.deltaTime;
	}

	private bool _minX = false;
	private bool _minY = false;
	private void IncrementSpeed() {
		_minX = targetMoveVector.x < moveVector.x;
		_minY = targetMoveVector.y < moveVector.y;

		if (_minX) {
			moveVector.x -= _speedChange * Time.deltaTime;
			if (moveVector.x < targetMoveVector.x)
				moveVector.x = targetMoveVector.x;
		} else {
			moveVector.x += _speedChange * Time.deltaTime;
			//moveVector.x -= targetMoveVector.x * Time.deltaTime;
			if (moveVector.x > targetMoveVector.x)
				moveVector.x = targetMoveVector.x;
		}

		//moveVector.y += Mathf.Sign(targetMoveVector.y)* _speedChange * Time.deltaTime;
		if (_minY) {
				moveVector.y -= _speedChange * Time.deltaTime;
			if (moveVector.y < targetMoveVector.y)
				moveVector.y = targetMoveVector.y;
		} else {
			moveVector.y += _speedChange * Time.deltaTime;
			//moveVector.y -= targetMoveVector.y * Time.deltaTime;
			if (moveVector.y > targetMoveVector.y)
				moveVector.y = targetMoveVector.y;
		}
		graphic.localScale = new Vector3(-Mathf.Sign(moveVector.x), 1, 1);
	}

	private void CalcMove() {

		if (transform.position.x < rect.x && moveVector.x < 0) {
			targetMoveVector.x = Random.Range(0.2f,1f) * Random.Range(minSpeed, maxSpeed);
		}else if (transform.position.x > rect.x + rect.width && moveVector.x > 0) {
			targetMoveVector.x = Random.Range(-1f,0.2f) * Random.Range(minSpeed, maxSpeed);
		} else {
			targetMoveVector.x = Random.Range(-1f, 1f) * Random.Range(minSpeed, maxSpeed);
		}

		if (transform.position.y < rect.y && moveVector.y < 0) {
			targetMoveVector.y = Random.Range(0.2f, 1f) * Random.Range(minSpeed, maxSpeed);
		} else if (transform.position.y > rect.y + rect.height && moveVector.y > 0) {
			targetMoveVector.y = Random.Range(-1f, 0.2f) * Random.Range(minSpeed, maxSpeed);
		} else {
			targetMoveVector.y = Random.Range(-1f, 1f) * Random.Range(minSpeed, maxSpeed);
		}
		spineAnimation.timeScale = targetMoveVector.magnitude*2;

		_speedChange = Random.Range(1f, 3f);

	}
	private float _speedChange = 0;


}
