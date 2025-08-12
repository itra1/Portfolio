using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class LevelsWievElement : ExEvent.EventBehaviour {

	public TextMeshPro text;
	public ActivateObject island;
	public List<ActivateObject> stars;
	public SpriteRenderer back;
	public SpriteRenderer decor;

	private bool _isOpen;
	public bool isOpen { get { return _isOpen; } }

	public int num;

	public Action OnSelect;

	private GameCompany.Level _level;
	private GameCompany.Save.Level _saveLevel;

	public Animation anim;

	public GameObject vawes;

	public GameCompany.Level level { get { return _level; } }

	[System.Serializable]
	public struct ActivateObject {
		public GameObject active;
		public GameObject deactive;
	}

	public void SetInfo(int num, GameCompany.Level level) {
		this.num = num;
		_level = level;

		//var grph = GraphicManager.Instance.link.GetGraphic(PlayerManager.Instance.company.actualLocationNum / 3 + 1);
		//back.sprite = grph.levelBack;
		//decor.sprite = grph.levelLockedDecor;

		CheckOpen();
	}
	
	private bool _loadProcess = false;

	public void Click() {

		if (_loadProcess) return;


		if (!_isOpen) {
			AudioManager.Instance.library.PlayClickInactiveAudio();
			anim.Play("lockedShake");
			return;
		}
		AudioManager.Instance.library.PlayClickAudio();

#if UNITY_ANDROID

		Play();

		//if (PlayerManager.Instance.company.CheckByeLocation(PlayerManager.Instance.company.actualLocationNum)) {
		//	Play();
		//}
		//else {
		//	AndroidPlayGame();
		//}

		
		//LocationsWorld lw = (LocationsWorld)WorldManager.Instance.GetWorld(WorldType.locations);

		//if (!lw.islandList[PlayerManager.Instance.company.actualLocationNum].isBye) {
		//	UnityAdsVideo.Instance.PlayVideo(() => {
		//		Play();
		//	});
		//} else {
		//	Play();
		//}

#else
		Play();
#endif



		//UnityAdsVideo.Instance.PlayVideo(() => {
		//	Play();
		//});
		//Play();
	}

	private void AndroidPlayGame() {

		if (!NetManager.Instance.internetAwalable) {

			InternetNotFoundGui noInternet = UIManager.Instance.GetPanel<InternetNotFoundGui>();
			noInternet.gameObject.SetActive(true);

			noInternet.OnCancel = () => { };
			noInternet.OnRepeat = () => {
				AndroidPlayGame();
			};
			
			return;
		}
		
		_loadProcess = true;
    
		GameManager.Instance.CheckAndPlayLevel(num, (res) => {
			_loadProcess = false;
			if (res)
				Play();
		});
	}

	private void Play() {
		if (OnSelect != null) OnSelect();
	}

	private void CheckOpen() {

		_saveLevel = PlayerManager.Instance.company.GetSaveLevel(PlayerManager.Instance.company.actualCompany, num);

		if (_saveLevel == null)
			_saveLevel = new GameCompany.Save.Level();

		text.text = (num+1).ToString();

		_isOpen = PlayerManager.Instance.company.CheckOpenLevel(num);

		island.active.SetActive(_isOpen);
		island.deactive.SetActive(!_isOpen);
		text.gameObject.SetActive(_isOpen);

	}

	[ExEvent.ExEventHandler(typeof(ExEvent.GameEvents.OnBattleChangePhase))]
	public void EndCompleted(ExEvent.GameEvents.OnBattleChangePhase onLevelComplited) {
        if(onLevelComplited.phase != BattlePhase.game)
		    CheckOpen();
	}

	public void SetWaves(bool isActive) {
		vawes.gameObject.SetActive(isActive);
	}

}