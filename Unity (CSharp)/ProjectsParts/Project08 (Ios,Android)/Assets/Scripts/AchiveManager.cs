using System;
using System.Collections;
using System.Collections.Generic;
using ExEvent;
using UnityEngine;

[System.Serializable]
public class AchiveManager {

	//public event System.Action<int> OnAdded;

	public List<int> openAchives = new List<int>();
	
	public void Init() {
		//NPBinding.GameServices.LoadAchievementDescriptions((_descriptions, _error) => { });
	}

	public void AddAchive(int num) {
		if (openAchives.Contains(num)) return;

		try {
			GameService.Instance.ReportAchives(String.Format("loc{0}", num));
			//NPBinding.GameServices.ReportProgressWithGlobalID(String.Format("loc{0}", num), 1f, (_success, _error) => { });
		}
		catch {
			
		}
		openAchives.Add(num);
		PlayerEvents.OnAddDecor.Call(num);
		ExEvent.PlayerEvents.OnChangeCompany.Call(PlayerManager.Instance.company.actualCompany);
		//if (OnAdded != null) OnAdded(num);
		PlayerManager.Instance.Save();
	}

	public bool CheckExistsAchive(int num) {
		return openAchives.Contains(num);
	}

	public List<int> Save() {
		return openAchives;
	}

	public void Load(List<int> achiveList) {
		openAchives = achiveList;
	}

	public void ClickIcon(int num) {
		var panel = UIManager.Instance.GetPanel(UiType.decor) as DecorAchiveUi;
		panel.gameObject.SetActive(true);
		panel.SetAchive(num);

		panel.OnTake = (takeNum) => {

			GameManager.Instance.BackFromTakeReward(() => {
				var gw = (LevelsWorld)WorldManager.Instance.GetWorld(WorldType.levels);
				gw.StopSalut();
				panel.gameObject.SetActive(false);
			}, () => {
				AddAchive(takeNum);
			});
		};

	}

}

