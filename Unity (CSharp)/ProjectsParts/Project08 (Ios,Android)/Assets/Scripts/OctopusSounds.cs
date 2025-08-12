using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OctopusSounds : ExEvent.EventBehaviour {

	public GamePhase gamePhase;
	public bool isUi;

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeGamePhase))]
	private void OnChangeGamePhase(ExEvent.GameEvents.OnChangeGamePhase phases) {
		if (gamePhase != phases.next) {
			StopSleepAudio();
		}
	}

	private AudioMixerTypes GetMix() {
		if(isUi)
			return AudioMixerTypes.effectUi;
		else
			return AudioMixerTypes.effectPlay;
	}

	public List<AudioClipData> waterDownLightAudio;

	public void PlayWaterDownLightAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(waterDownLightAudio[Random.Range(0, waterDownLightAudio.Count)], GetMix());
	}

	public List<AudioClipData> waterDownHardAudio;

	public void PlayWaterDownHardAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(waterDownHardAudio[Random.Range(0, waterDownHardAudio.Count)], GetMix());
	}

	public List<AudioClipData> bubbleAudio;

	public void PlayBubbleAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(bubbleAudio[Random.Range(0, bubbleAudio.Count)], GetMix());
	}

	public List<AudioClipData> happyAudio;

	public void PlayHappyAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(happyAudio[Random.Range(0, happyAudio.Count)], GetMix());
	}

	public List<AudioClipData> idleAudio;
	private AudioPoint idlePoint;

	public void PlayIdleAudio() {
		return;
		if (GameManager.gamePhase != gamePhase) return;
		idlePoint = AudioManager.PlayEffects(idleAudio[Random.Range(0, idleAudio.Count)], GetMix(), idlePoint);
	}

	public void StopIdleAudio() {
		if (sleepPoint != null) {
			idlePoint.Stop();
			idlePoint = null;
		}
	}

	public List<AudioClipData> happyCryAudio;

	public void PlayHappyCryAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(happyCryAudio[Random.Range(0, happyCryAudio.Count)], GetMix());
	}

	public List<AudioClipData> sadAudio;

	public void PlaySadAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(sadAudio[Random.Range(0, sadAudio.Count)], GetMix());
	}

	public List<AudioClipData> sleepAudio;
	private AudioPoint sleepPoint;

	public void PlaySleepAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		sleepPoint = AudioManager.PlayEffects(sleepAudio[Random.Range(0, sleepAudio.Count)], GetMix());
	}

	public void StopSleepAudio() {
		if (sleepPoint != null) {
			sleepPoint.Stop();
			sleepPoint = null;
		}
	}

	public List<AudioClipData> surpriseAudio;

	public void PlaySurpriseAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(surpriseAudio[Random.Range(0, surpriseAudio.Count)], GetMix());
	}

	public List<AudioClipData> waterUpAudio;

	public void PlayWaterUpAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(waterUpAudio[Random.Range(0, waterUpAudio.Count)], GetMix());
	}

	public List<AudioClipData> wordCorrectAudio;

	public void PlayWordCorrectAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(wordCorrectAudio[Random.Range(0, wordCorrectAudio.Count)], GetMix());
	}

	public List<AudioClipData> wordWrongAudio;

	public void PlayWordWrongAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(wordWrongAudio[Random.Range(0, wordWrongAudio.Count)], GetMix());
	}


	////

	public List<AudioClipData> curiousAudio;

	public void PlayCuriousAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(curiousAudio[Random.Range(0, curiousAudio.Count)], GetMix());
	}

	public AudioClipData despondencyAudio;

	public void PlayDespondencyAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(despondencyAudio, GetMix());
	}

	public AudioClipData disgustinglyAudio;

	public void PlayDisgustinglyAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(disgustinglyAudio, GetMix());
	}

	public AudioClipData disgustingly2Audio;

	public void PlayDisgustingly2Audio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(disgustingly2Audio, GetMix());
	}

	public AudioClipData reactAudio;

	public void PlayReactAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(reactAudio, GetMix());
	}

	public AudioClipData singAudio;

	public void PlaySingAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(singAudio, GetMix());
	}

	public AudioClipData sing2Audio;

	public void PlaySing2Audio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(sing2Audio, GetMix());
	}

	public AudioClipData sing3Audio;

	public void PlaySing3Audio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(sing3Audio, GetMix());
	}

	public AudioClipData wonderingAudio;

	public void PlayWonderingAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(wonderingAudio, GetMix());
	}

	public AudioClipData wondering2Audio;

	public void PlayWondering2Audio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(wondering2Audio, GetMix());
	}

	public AudioClipData wondering3Audio;

	public void PlayWondering3Audio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(wondering3Audio, GetMix());
	}

	public AudioClipData wondering4Audio;

	public void PlayWondering4Audio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(wondering4Audio, GetMix());
	}

	public List<AudioClipData> eyeAudio;

	public void PlayEyeAudio() {
		if (GameManager.gamePhase != gamePhase) return;

		AudioManager.PlayEffects(eyeAudio[Random.Range(0, eyeAudio.Count)], GetMix());
	}
	
}
