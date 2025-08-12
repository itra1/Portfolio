using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(BlackRound))]
public class BlackRoundEditor: Editor
{
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if (GUILayout.Button("Play")) {
			((BlackRound) target).Play();
		}

	}
}

#endif


public class BlackRound : UiPanel {

	public Image image;
	private Material _material;

	private int maxSize;

	private void Start() {
		_material = image.material;


		if (Camera.main.pixelHeight > 1500) {
			maxSize = 500000;
		}
		else {
			maxSize = 300000;
		}

	}

	public void Play(Action OnFullHide = null, Action OnStart = null, Action OnEnd = null) {
		PlayClickAudio();
		StartCoroutine(PlayChange(OnFullHide, OnStart, OnEnd));
	}

	IEnumerator PlayChange(Action OnFullHide, Action OnStart, Action OnEnd) {
		if (OnStart != null) OnStart();
		image.gameObject.SetActive(true);

		Vector4 scale = new Vector4(0, 0, 0, maxSize);
		_material.SetVector("_Point", scale);

		while (scale.w > 0) {
			scale.w -= maxSize * Time.deltaTime * 4.5f;
			_material.SetVector("_Point", scale);
			yield return null;
		}

		scale.w = 0;
		_material.SetVector("_Point", scale);
		yield return null;
		if (OnFullHide != null) OnFullHide();
		yield return new WaitForFixedUpdate();

		while (scale.w < maxSize) {
			scale.w += maxSize * Time.deltaTime * 4.5f;
			_material.SetVector("_Point", scale);
			yield return null;
		}

		image.gameObject.SetActive(false);
		if (OnEnd != null) OnEnd();

	}

	public AudioClipData openAudio;

	public void PlayClickAudio() {
		AudioManager.PlayEffects(openAudio, AudioMixerTypes.effectUi);
	}

	public override void ManagerClose() {
		//throw new NotImplementedException();
	}
}
