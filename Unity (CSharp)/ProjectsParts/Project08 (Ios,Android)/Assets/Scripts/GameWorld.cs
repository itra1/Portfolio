using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWorld : WorldAbstract {

	public LettersController lettersController;
	public GameIsland gameIsland;
	public OctopusController octopus;

	public GameObject islandObject;
	public SpriteRenderer spriteRenderer;

	public Sprite defBack;
	public Sprite bonusBack;

	public GameObject cloudParticle;
	public GameObject giftParticle;

	private void Start() {
		
		if((float)Camera.main.pixelWidth / (float)Camera.main.pixelHeight <= 0.56f)
			gameIsland.transform.localScale = Vector3.one * 0.8f;

	}
	
	public void OnHide() {
		octopus.PlayHide();
	}

	public ParticleSystem salut;
	public void PlaySalut() {
		salut.Play();
	}

	public void PauseSalut() {
		salut.Pause();
	}

	public void StopSalut() {
		salut.Stop();
	}

	public void Init() {
		GraphicReady();

		lettersController.SetData();
		gameIsland.InitData();


	}

	public void GraphicReady() {
		cloudParticle.gameObject.SetActive(!PlayerManager.Instance.company.isBonusLevel);
		giftParticle.gameObject.SetActive(PlayerManager.Instance.company.isBonusLevel);

		islandObject.SetActive(!(PlayerManager.Instance.company.isBonusLevel || Tutorial.Instance.isTutorial));
		if (PlayerManager.Instance.company.isBonusLevel || Tutorial.Instance.isTutorial) {
			spriteRenderer.sprite = bonusBack;
		} else {
			spriteRenderer.sprite = defBack;
		}
	}

	public void OnShow() {
		lettersController.ShowLetter();
		gameIsland.ShowLetter();
		octopus.PlayShow();

		if (PlayerManager.Instance.company.actualLocationNum == 0 && PlayerManager.Instance.company.GetActualSaveLevel().words.Count == 0) {

			switch (PlayerManager.Instance.company.actualLevelNum) {
				case 1:
					FirebaseManager.Instance.LogEvent("start_level","level","1.1");
					break;
				case 5:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.5");
					break;
				case 10:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.10");
					break;
				case 15:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.15");
					break;
				case 20:
          FirebaseManager.Instance.LogEvent("start_level", "level", "1.20");
					break;
			}

		}

		if (PlayerManager.Instance.company.actualLocationNum == 1 && PlayerManager.Instance.company.GetActualSaveLevel().words.Count == 0) {

			switch (PlayerManager.Instance.company.actualLevelNum) {
				case 1:
          FirebaseManager.Instance.LogEvent("start_level", "level", "2.1");
					break;
			}

		}
	}

}
