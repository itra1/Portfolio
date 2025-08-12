using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using GameCompany;
using UnityEngine;

public class IslandLocationsWorld : EventBehaviour {
	
	public Action OnComplited;
	public Action OnShowIsland;
	public Action<IslandLocationsWorld> OnDownIsland;
	public Action<IslandLocationsWorld> OnUpIsland;

	public int id;
	public int num;

	public bool isShare;

	public GameCompany.Location location;
	public GameCompany.Save.Location saveLocation;

	public List<GameObject> islandbackList;

	public Animation animComponent;
	
	private string _toLevelAnim = "toLevels";
	public string _fromLevelsAnim = "backLocation";
	
	public GameObject lockedIsland;

	public GameObject islandBlock;
	public GameObject rateBlock;

	public Animation lockedAnim;
	private bool _isBye;
	private bool _beforeComplete;
	private bool _isComplete;

	public bool isBye {
		get { return _isBye; }
	}
	public bool isComplete {
		get { return _isComplete; }
	}
	public bool beforeComplete {
		get { return _beforeComplete; }
	}

	public bool isOpen {
		get {
//			if (!isBye) {
//#if UNITY_ANDROID
//				return beforeComplete && this.saveLocation.isFreePlay;
//#else
//				return false;
//#endif
//			}
			//else {
				return isBye && beforeComplete;
			//}


		}
	}

	public void SetData(int num, GameCompany.Location location) {
		isShare = false;
		this.num = num;
		this.location = location;
		this.saveLocation = PlayerManager.Instance.company.GetSaveLocation(PlayerManager.Instance.company.actualCompany, num);

		for (int i = 0; i < islandbackList.Count; i++) {
			islandbackList[i].SetActive(num == i);
		}

		islandBlock.gameObject.SetActive(true);
		rateBlock.gameObject.SetActive(false);

		//_isBye = PlayerManager.Instance.company.CheckByeLocation(this.num);
		_isBye = true;
		_beforeComplete = PlayerManager.Instance.company.CheckBeforeLocationComplete(this.num);
		_isComplete = PlayerManager.Instance.company.CheckCompleteLocation(this.num);

		lockedIsland.gameObject.SetActive(!isOpen);
		//if (isOpen) {
		//	lockedIsland.gameObject.SetActive(false);
		//}
		//else {
		//	if (this.saveLocation != null) {
		//		lockedIsland.gameObject.SetActive(!isOpen);
		//		return;
		//	}
		//}
		

		
		//lockedIsland.gameObject.SetActive(isOpen);
	}
	/*
	[ExEvent.ExEventHandler(typeof(ExEvent.PlayerEvents.OnByeLocation))]
	void OnLoadPlayer(ExEvent.PlayerEvents.OnByeLocation onLoad) {
		if (isShare) return;
		if (PlayerManager.Instance.company.CheckAccessLocation(this.num)) {
			isOpen = true;
			lockedIsland.gameObject.SetActive(false);
			return;
		}
	}
	*/

	public void SetIsShare(int num) {
		this.num = num;
		isShare = true;
		islandBlock.gameObject.SetActive(false);
		rateBlock.gameObject.SetActive(true);
		location = new Location();
		location.title = "Coming soon";
	}

	public void ReadyLevels() {
		if (OnComplited != null) OnComplited();
	}

	public void Click() {
		OnDownIsland(this);
	}

	public void OnDown() {
		if (GameManager.gamePhase != GamePhase.locations) return;
		OnDownIsland(this);
	}

	public void OnUp() {
		if (GameManager.gamePhase != GamePhase.locations) return;
		OnUpIsland(this);
	}

	public void PlayToLevel() {
		animComponent.Play(_toLevelAnim);
	}

	public void FromLevels(Action OnShowIsland) {
		animComponent.Play(_fromLevelsAnim);
		this.OnShowIsland = OnShowIsland;
	}

	public void OnFromLevels() {
		if (OnShowIsland != null) OnShowIsland();
	}

	public void EndToLevels() {
		//gameObject.SetActive(false);
	}

	public void PlayLocked() {
		lockedAnim.Play("lockedShake");
	}


}
