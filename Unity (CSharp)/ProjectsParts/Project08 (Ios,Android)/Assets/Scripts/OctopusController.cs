using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Spine;
using Spine.Unity;
using VoxelBusters.Utility;

public class OctopusController : Singleton<OctopusController> {

	private OctopusSpine _spine;
	private int idleCount = 0;

	private bool isShow = true;

	private int _losCount = 0;

	public Transform graphic;

	// Количество показанных предложений воспользоваться подсказками
	private int showHelpCount = 0;

	private int countFalse = 0;
	private int countConfirm = 0;

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnWordSelect))]
	public void OnSelectWord(ExEvent.GameEvents.OnWordSelect selectWord) {

		if (selectWord.word.Length <= 1) return;

		if (!isVisible || isPlayAnim) return;
		
		switch (selectWord.select) {
			case SelectWord.yes:
				_losCount = 0;
				countFalse = 0;
				countConfirm++;
				if (countConfirm >= 2) {
					countConfirm = 0;
					_spine.PlayAnim(OctopusSpine.yesAnim, false);
				}
				//StartWait();
				break;
			case SelectWord.repeat:
				_losCount++;
				countConfirm = 0;
				countFalse++;
				if (countFalse >= 2) {
					countFalse = 0;
					_spine.PlayAnim(OctopusSpine.noAnim, false);
				}
				break;
			case SelectWord.no:
				_losCount++;
				countConfirm = 0;
				countFalse++;
				if (countFalse >= 2) {
					countFalse = 0;
					_spine.PlayAnim(OctopusSpine.noAnim, false);
				}
				break;
			case SelectWord.conchRepeat:
				_losCount++;
				countConfirm = 0;
				countFalse++;
				if (countFalse >= 2) {
					countFalse = 0;
					_spine.PlayAnim(OctopusSpine.noAnim, false);
				}
				break;
			case SelectWord.conchYes:
				_losCount = 0;
				countFalse = 0;
				countConfirm++;
				if (countConfirm >= 2) {
					countConfirm = 0;
					_spine.PlayAnim(OctopusSpine.supriseAnim, false);
				}
				//StartWait();
				break;
		}

		if (PlayerManager.Instance.countGameHelper < 2 && _losCount >= 6 && showHelpCount < 2) {
			PlayerManager.Instance.countGameHelper++;
			_losCount = 0;
			ShowHelp();
			showHelpCount++;
		}

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnLetterLoaded))]
	private void OnLetterLoaded(ExEvent.GameEvents.OnLetterLoaded compl) {
		showHelpCount = 0;
	}

	private bool _dialogActive;
	private Coroutine activeWait;

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.ShowWordTranslate))]
	public void ShowWordTranslate(ExEvent.GameEvents.ShowWordTranslate word) {
		_dialogActive = false;
		PetDialogs.Instance.Close();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintAnyLetterReady))]
	public void HintEnyLetterReady(ExEvent.GameEvents.OnHintAnyLetterReady hint) {
		_dialogActive = false;
		PetDialogs.Instance.Close();
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnHintAnyLetterCompleted))]
	public void HintEnyLetterCompleted(ExEvent.GameEvents.OnHintAnyLetterCompleted hint) {
		_dialogActive = false;
		PetDialogs.Instance.Close();
	}

	private void ShowHelp() {

		if (PlayerManager.Instance.company.isBonusLevel || Tutorial.Instance.isTutorial) return;

		PlayGamePlay gp = (PlayGamePlay)UIManager.Instance.GetPanel(UiType.game);
		gp.miniMenu.Open();
		PetDialogs.Instance.ShowDialog(PetDialogType.getHelp, () => {
			_dialogActive = false;
			PetDialogs.Instance.Close();
			gp.miniMenu.Close();
		});
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnBattleChangePhase))]
	public void OnEndGame(ExEvent.GameEvents.OnBattleChangePhase eg) {

		/*
		if (eg.phase == BattlePhase.win)
			_spine.PlayAnim(OctopusSpine.happyAnim, true);
		if (eg.phase == BattlePhase.full)
			_spine.PlayAnim(OctopusSpine.sadAnim, true);
		*/
	}

	public void Happy() {
		_spine.PlayAnim(OctopusSpine.happyAnim, false);
	}
	public void Sad() {
		_spine.PlayAnim(OctopusSpine.sadAnim, false);
	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnChangeGamePhase))]
	public void OnChangeGamePhase(ExEvent.GameEvents.OnChangeGamePhase eg) {
		if (eg.last == GamePhase.game && eg.next != GamePhase.game) {

			if (!Tutorial.Instance.isTutorial && !PlayerManager.Instance.company.isBonusLevel)
				PlayHide();

			if (_dialogActive) {
				_dialogActive = false;
				PetDialogs.Instance.Close();
			}

			if (activeWait != null)
				StopCoroutine(activeWait);
		}
		if (eg.next == GamePhase.game) {
			_losCount = 0;
			ResetPositionOctopus();
			//StartWait();
		}
	}

	private void ResetPositionOctopus() {
		var sort = SortingLayer.layers.ToList().Find(x => x.name == "Default");
		GetComponent<OctopusSounds>().isUi = false;

		_spine.skeleton.GetComponent<MeshRenderer>().sortingLayerID = sort.id;
		_spine.skeleton.GetComponent<MeshRenderer>().sortingOrder = 0;
		graphic.localPosition = Vector3.zero;
	}

	public void ToFrontOrder() {

		var sort = SortingLayer.layers.ToList().Find(x => x.name == "UI");

		_spine.skeleton.GetComponent<MeshRenderer>().sortingLayerID = sort.id;
		_spine.skeleton.GetComponent<MeshRenderer>().sortingOrder = 100;
		GetComponent<OctopusSounds>().isUi = true;
	}

	public System.Action hideComplete;
	public System.Action showComplete;

	public void PlayHide(System.Action conplete = null) {
		if (!isShow) return;
		isShow = false;
		hideComplete = conplete;
		_spine.PlayAnim(OctopusSpine.downLightAnim, false);
	}

	public void PlayShow(System.Action conplete = null) {
		if (isShow) {
			PlayIdle();
			return;
		}
		isShow = true;
		showComplete = conplete;
		_spine.PlayAnim(OctopusSpine.upAnim, false);
	}

	private void OnEnable() {

		_spine = GetComponent<OctopusSpine>();
		_spine.OnCompleted = AnimCompleted;
		_spine.OnEnd = AnimEnd;
		_spine.OnStart = AnimStart;
		_spine.OnInterrupt = AnimInterrupt;
		_spine.OnDispose = AnimDispose;
		_spine.OnRebuild = () => {
		};
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
		
		if (trackName == OctopusSpine.upAnim && showComplete != null) {
			if (showComplete != null) showComplete();
			showComplete = null;
			return;
		}

		if (trackName == OctopusSpine.downLightAnim && hideComplete != null) {
			if (hideComplete != null) hideComplete();
			hideComplete = null;
			return;
		}

		if (trackName == OctopusSpine.idleAnim)
			idleCount++;
		if (idleCount == 6) {
			idleCount = 0;
			if (!PlayerManager.Instance.company.isBonusLevel && !Tutorial.Instance.isTutorial) {
				_spine.skeleton.Initialize(true);
				_spine.PlayAnim(OctopusSpine.sleepAnim, true);
			} else {
				PlayRandomAnimation();
			}
		}

		if (trackName != OctopusSpine.sleepAnim
			&& trackName != OctopusSpine.idleAnim
			&& trackName != OctopusSpine.downLightAnim) {
			_spine.PlayAnim(OctopusSpine.idleAnim, true);
			idleCount = 0;
		}

		if (trackName == OctopusSpine.downLightAnim && isVisible) {
			_spine.PlayAnim(OctopusSpine.upAnim, false);
		} else if (trackName == OctopusSpine.upAnim && !isVisible) {
			_spine.PlayAnim(OctopusSpine.downLightAnim, false);
		} else {
			isPlayAnim = false;
		}

	}

	private void PlayRandomAnimation() {
		int numAnim = Random.Range(0, 10);
		switch (numAnim) {
			case 0:
				_spine.PlayAnim(OctopusSpine.fartAnim, false);
				break;
			case 1:
				_spine.PlayAnim(OctopusSpine.upAnim, false);
				break;
			case 2:
				_spine.PlayAnim(OctopusSpine.singAnim, false);
				break;
			case 3:
				_spine.PlayAnim(OctopusSpine.sing2Anim, false);
				break;
			case 4:
				_spine.PlayAnim(OctopusSpine.sing3Anim, false);
				break;
			case 5:
				_spine.PlayAnim(OctopusSpine.supriseAnim, false);
				break;
			case 6:
				_spine.PlayAnim(OctopusSpine.wonderingAnim, false);
				break;
			case 7:
				_spine.PlayAnim(OctopusSpine.wondering2Anim, false);
				break;
			case 8:
				_spine.PlayAnim(OctopusSpine.wondering3Anim, false);
				break;
			case 9:
				_spine.PlayAnim(OctopusSpine.wondering4Anim, false);
				break;
		}
	}

	private void OnMouseDown() {
		Click();
	}
	
	public void Click() {
		ClickRandomAnimation();
	}

	private void ClickRandomAnimation() {
		if (_spine.actualAnimation != OctopusSpine.idleAnim && _spine.actualAnimation != OctopusSpine.sleepAnim && _spine.actualAnimation != OctopusSpine.sleepNoWaweAnim) return;
		int num = Random.Range(0, 12);
		switch (num) {
			case 0:
				_spine.PlayAnim(OctopusSpine.curiousAnim, false);
				break;
			case 1:
				_spine.PlayAnim(OctopusSpine.despondencyAnim, false);
				break;
			case 2:
				_spine.PlayAnim(OctopusSpine.disgustinglyAnim, false);
				break;
			case 3:
				_spine.PlayAnim(OctopusSpine.disgustingly2Anim, false);
				break;
			case 4:
				_spine.PlayAnim(OctopusSpine.reactAnim, false);
				break;
			case 5:
				_spine.PlayAnim(OctopusSpine.singAnim, false);
				break;
			case 6:
				_spine.PlayAnim(OctopusSpine.sing2Anim, false);
				break;
			case 7:
				_spine.PlayAnim(OctopusSpine.sing3Anim, false);
				break;
			case 8:
				_spine.PlayAnim(OctopusSpine.wonderingAnim, false);
				break;
			case 9:
				_spine.PlayAnim(OctopusSpine.wondering2Anim, false);
				break;
			case 10:
				_spine.PlayAnim(OctopusSpine.wondering3Anim, false);
				break;
			case 11:
				_spine.PlayAnim(OctopusSpine.wondering4Anim, false);
				break;
		}
	}


	private void AnimEnd(string trackName) {
	}

	private int checkCount = 0;
	private bool isVisible = true;
	private bool isPlayAnim = false;
	public void SetLineCheck() {

		return;

		if (isVisible) {
			if (!isPlayAnim) {
				_spine.PlayAnim(OctopusSpine.downLightAnim, false);
				isPlayAnim = true;
			}
			isVisible = false;
		}
		checkCount++;

	}

	public void UnSetLineCheck() {

		return;

		checkCount--;

		if (!isVisible && checkCount <= 0) {
			if (!isPlayAnim) {
				_spine.PlayAnim(OctopusSpine.upAnim, false);
				isPlayAnim = true;
			}
			isVisible = true;
		}
	}



}
