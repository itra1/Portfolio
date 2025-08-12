using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusLocations : ExEvent.EventBehaviour {

	private OctopusSpine _spine;
	public Transform parent;
	public Transform _graphic;

	private bool isShow = true;

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeGamePhase))]
	public void OnChangeGamePhase(ExEvent.GameEvents.OnChangeGamePhase eg) {
		if (eg.last == GamePhase.locations && eg.next != GamePhase.locations)
			PlayHide();
		if (eg.last != GamePhase.locations && eg.next == GamePhase.locations)
			PlayShow();
	}

	private void OnEnable() {
		_spine = GetComponent<OctopusSpine>();
		_spine.OnCompleted = AnimCompleted;
		_spine.OnEnd = AnimEnd;
		_spine.OnStart = AnimStart;
		_spine.OnInterrupt = AnimInterrupt;
		_spine.OnDispose = AnimDispose;
		_spine.OnRebuild = () => {
			//PlayHide();
		};
		_floatPhase = FloatPhase.none;
	}

	public void PlayShow() {
		if (isShow) {
			PlayIdle();
			return;
		}
		isShow = true;
		_spine.PlayAnim(OctopusSpine.upAnim, false);
	}

	public void PlayHide() {
		if (!isShow) return;
		isShow = false;
		_spine.PlayAnim(OctopusSpine.downLightAnim, false);
	}

	public void PlayIdle() {
		_spine.PlayAnim(OctopusSpine.idleAnim, true);
	}

	private void AnimStart(string trackName) {
	}

	private void AnimInterrupt(string trackName) {
	}

	private void AnimDispose(string trackName) {
	}

	private void AnimCompleted(string trackName) {
		if (trackName != OctopusSpine.sleepAnim
			&& trackName != OctopusSpine.idleAnim
			&& trackName != OctopusSpine.floatsStartAnim
			&& trackName != OctopusSpine.floatsIdleAnim
			&& trackName != OctopusSpine.downLightAnim) {

			if(trackName == OctopusSpine.floatsFinishAnim)
				_spine.skeleton.Initialize(true);

			if (_floatPhase == FloatPhase.finish) {
				_spine.PlayAnim(OctopusSpine.idleAnim, true);
			}
		}

		if (trackName == OctopusSpine.floatsStartAnim) {
			isMove = true;
			_startFloat = false;
			//_spine.skeleton.Initialize(true);
			PlayIdleFloatAudio();
			_floatPhase = FloatPhase.idle;
			_spine.PlayAnim(OctopusSpine.floatsIdleAnim, true);
		}
	}

	private void AnimEnd(string trackName) {
	}

	private void Update() {
		if (isMove) {
			Move();
		}
		
	}

	private void Move() {
		float moveVectorX = Mathf.Sign(targetLocalPosition - transform.localPosition.x);

		Vector3 newPosition = transform.localPosition + Vector3.right * moveVectorX * 15 * Time.deltaTime;
		if (Mathf.Abs(newPosition.x - transform.localPosition.x) < Mathf.Abs(targetLocalPosition - transform.localPosition.x)) {
			transform.localPosition = newPosition;
		}
		else {
			transform.localPosition = new Vector3(targetLocalPosition, transform.localPosition.y, transform.localPosition.z);
			isMove = false;
			PlayEndFloatAudio();
			_floatPhase = FloatPhase.finish;
			_spine.PlayAnim(OctopusSpine.floatsFinishAnim, false);
			_graphic.localScale = new Vector3(1, 1, 1);
		}
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.SwipeComplete))]
	private void OnChangeSwipe(ExEvent.GameEvents.SwipeComplete sw) {
		targetLocalPosition = Mathf.Abs(parent.transform.position.x);

		if (Mathf.Abs(targetLocalPosition - transform.localPosition.x) < 0.5f) return;

		if (targetLocalPosition < transform.localPosition.x) {
			_graphic.localScale = new Vector3(-1, 1, 1);
		} else {
			_graphic.localScale = new Vector3(1, 1, 1);
		}
		PlayStartFloatAudio();
		_startFloat = true;
		
		if (_floatPhase == FloatPhase.finish) {
			_spine.skeleton.Initialize(true);
		}

		_floatPhase = FloatPhase.start;
		_spine.PlayAnim(OctopusSpine.floatsStartAnim, false);

	}

	private bool _startFloat;

	private float targetLocalPosition = 0;

	private bool _isMove;

	private FloatPhase _floatPhase;

	public enum FloatPhase {
		none, start, idle, finish
	}

	public bool isMove {
		get { return _isMove; }
		set {
			_isMove = value;
		}
	}

	private AudioPoint audioPoint;

	public AudioClipData floatStartAudio;

	public void PlayStartFloatAudio() {
		return; // пока отключаем музыку
		AudioManager.PlayEffects(floatStartAudio, AudioMixerTypes.effectUi);
	}

	public AudioClipData floatIdleAudio;

	public void PlayIdleFloatAudio() {
		return;
		audioPoint = AudioManager.PlayEffects(floatIdleAudio, AudioMixerTypes.effectUi, audioPoint);
	}

	public AudioClipData floatEndAudio;

	public void PlayEndFloatAudio() {
		if (GameManager.gamePhase != GamePhase.locations) return;
		//audioPoint.Stop();
		//return; // пока отключаем музыку
		AudioManager.PlayEffects(floatEndAudio, AudioMixerTypes.effectUi);
	}


}
