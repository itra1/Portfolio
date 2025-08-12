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
			((BlackRound)target).Play();
		}

	}
}

#endif


public class BlackRound : Singleton<BlackRound> {
  
  public Image image;
  private Material _material;
  private Material material {
    get {
      if(_material == null)
        _material = image.material;
      return _material;
    }
  }

  protected override void Awake() {
    base.Awake();
  }

  private int maxSize;

	private void Start() {

		if (Camera.main.pixelHeight > 1500) {
			maxSize = 500000;
		}
		else {
			maxSize = 300000;
		}

	}

	public void Play(Action OnFullHide = null, Action OnStart = null, Action OnEnd = null) {
    
		StartCoroutine(PlayChange(OnFullHide, OnStart, OnEnd));
	}

	IEnumerator PlayChange(Action OnFullHide, Action OnStart, Action OnEnd) {

		if (OnStart != null) OnStart();
		image.gameObject.SetActive(true);

		Vector4 scale = new Vector4(0, 0, 0, maxSize);
		material.SetVector("_Point", scale);

		while (scale.w > 0) {
			scale.w -= maxSize * Time.deltaTime * 4.5f;
			material.SetVector("_Point", scale);
			yield return null;
		}

		scale.w = 0;
		material.SetVector("_Point", scale);
		yield return null;
		if (OnFullHide != null) OnFullHide();
		yield return new WaitForFixedUpdate();

		while (scale.w < maxSize) {
			scale.w += maxSize * Time.deltaTime * 4.5f;
			material.SetVector("_Point", scale);
			yield return null;
		}

		image.gameObject.SetActive(false);
		if (OnEnd != null) OnEnd();

	}
  
}
