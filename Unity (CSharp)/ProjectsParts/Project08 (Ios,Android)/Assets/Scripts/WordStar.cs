using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class WordStar : MonoBehaviour {

	public GameObject activeGraphic;
	public GameObject deactiveGraphic;

	public GameObject graph;

	public ParticleSystem effectsIscri;

	private void OnEnable() {
		graph.SetActive(false);
	}

	public bool isOpen;
	
	public void SetOpen(bool isOpen, bool isFirst = false, bool isEffect = true) {
		this.isOpen = isOpen;

		if (isFirst) {
			activeGraphic.SetActive(false);
			deactiveGraphic.SetActive(true);
			return;
		}
		activeGraphic.SetActive(isOpen);
		deactiveGraphic.SetActive(!isOpen);


		if (isEffect) PlayIskr();
	}

	public void Visual() {
		graph.SetActive(true);
		GetComponent<Animation>().Play("show");
	}

	public void PlayIskr() {
		effectsIscri.Play();
	}

}
