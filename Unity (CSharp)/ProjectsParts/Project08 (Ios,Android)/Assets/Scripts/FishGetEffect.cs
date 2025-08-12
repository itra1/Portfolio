using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishGetEffect : MonoBehaviour {

	public List<CoinMoverFish> coinList;

	private void OnEnable() {
		int coinsValue = Random.Range(1, 3);
		PlayClickAudio();


		for (int i = 0; i < coinsValue; i++) {
			coinList[i].gameObject.SetActive(true);
		}
		Invoke("Deactive", 3);
	}

	public void SetCoinsTransform(Transform target) {
		coinList.ForEach(x => x.SetTarget(target));
	}

	void Deactive() {
		gameObject.SetActive(false);
	}

	private void OnDisable() {
		coinList.ForEach(x => x.gameObject.SetActive(false));
	}

	public AudioClipData openAudio;

	public void PlayClickAudio() {
		AudioManager.PlayEffects(openAudio, AudioMixerTypes.effectUi);
	}

}
