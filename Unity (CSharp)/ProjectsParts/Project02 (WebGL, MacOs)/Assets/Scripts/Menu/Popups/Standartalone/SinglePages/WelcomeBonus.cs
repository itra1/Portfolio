using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using it.Api;
using System.Linq;
using System;

namespace it.Main.SinglePages
{
	public class WelcomeBonus : SinglePage
	{
		[SerializeField] private Color _noActiveColor;
		[SerializeField] private Page[] _pages;
		[SerializeField] private List<GameObject> _infoPanels = new List<GameObject>();

		private GameObject _activePanel;

		[System.Serializable]
		private struct Page
		{
			public it.UI.Elements.FilterSwitchButtonUI Button;
			public GameObject Panel;
			public GameObject Lock;
			public int Level;
			public Image DaysTimerImage;
			public TextMeshProUGUI DaysTimeLabel;
			public TextMeshProUGUI[] ColoredText;
			public RectTransform ProgressImage;
			public TextMeshProUGUI[] Values;
			public GameObject[] Points;
			public int MaxDays;
		}

		private int _index = 0;

		public void OpenInfo(){
			var infoPane = _activePanel.transform.Find("InfoPanel");
			if(infoPane)
			infoPane.gameObject.SetActive(true);
		}

		private void OnDestroy()
		{
			com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.WelcomeBonusUpdate, WelcomeBonusUpdateEvent);
		}

		protected override void EnableInit()
		{
			base.EnableInit();

			_infoPanels.ForEach(x => x.gameObject.SetActive(false));
			com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.WelcomeBonusUpdate, WelcomeBonusUpdateEvent);


			for (int i = 0; i < _pages.Length; i++)
			{
				int index = i;
				var p = _pages[i];
				p.Button.OnClickPointer.RemoveAllListeners();
				p.Button.OnClickPointer.AddListener(() =>
				{
					_infoPanels.ForEach(x => x.gameObject.SetActive(false));
					VisiblePage(index);
				});
			}
			VisiblePage(0);

		}
		public void VisiblePage(int index)
		{
			_index = index;
			ConfirmData();
		}

		private void WelcomeBonusUpdateEvent(com.ootii.Messages.IMessage hendle)
		{
			ConfirmData();
		}

		private void ConfirmData()
		{
			var struc = _pages[_index];
			var data = UserController.ReferenceData.welcome_bonus[_index];
			var currentBonus = UserController.Instance.WelcomeBonus.last_bonus;

			WelcomeBonusData wb = UserController.Instance.WelcomeBonus;
			bool isDisactive = wb == null || wb.last_bonus == null || (bool)wb.last_bonus.finished;

			_activePanel = struc.Panel;
			for (int i = 0; i < _pages.Length; i++)
			{
				_pages[i].Button.IsSelect = _index == i;
				_pages[i].Panel.gameObject.SetActive(_index == i);
			}

			for(int i = 0; i < struc.Values.Length; i++){
				struc.Values[i].text = $"{(i+1)*20}%";
			}

			if(currentBonus == null || isDisactive)
			{

				//	struc.DaysTimeLabel.text = 0.ToString();
				struc.DaysTimerImage.fillAmount = 0;
				for (int i = 0; i < struc.ColoredText.Length; i++)
				{
					struc.ColoredText[i].color = _noActiveColor;
				}
				struc.Lock.SetActive(true);
				struc.ProgressImage.localScale = Vector3.zero;
				for (int i = 0; i < struc.Points.Length; i++)
				{
					struc.Points[i].gameObject.SetActive(false);
				}
				return;
			}


			if(struc.Level < currentBonus.level){
				struc.DaysTimeLabel.text = 0.ToString();
				struc.DaysTimerImage.fillAmount = 0;
				for(int i = 0; i < struc.ColoredText.Length;i++){
					struc.ColoredText[i].color = Color.white;
				}
				struc.Lock.SetActive(false);
				struc.ProgressImage.localScale = Vector3.one;
				for (int i = 0; i < struc.Points.Length; i++)
				{
					struc.Points[i].gameObject.SetActive(true);
				}
				struc.DaysTimeLabel.text = 0.ToString();
				struc.DaysTimerImage.fillAmount = 0;
			}
			if (struc.Level > currentBonus.level)
			{
			//	struc.DaysTimeLabel.text = 0.ToString();
				struc.DaysTimerImage.fillAmount = 0;
				for (int i = 0; i < struc.ColoredText.Length; i++)
				{
					struc.ColoredText[i].color = _noActiveColor;
				}
				struc.Lock.SetActive(true);
				struc.ProgressImage.localScale = Vector3.zero;
				struc.DaysTimeLabel.text = struc.MaxDays.ToString();
				struc.DaysTimerImage.fillAmount = 1;
				for (int i = 0; i < struc.Points.Length; i++)
				{
					struc.Points[i].gameObject.SetActive(false);
				}
			}
			if (struc.Level == currentBonus.level)
			{
				struc.DaysTimeLabel.text = (currentBonus.ExpiredDate - System.DateTime.Now).Days.ToString();
				struc.DaysTimerImage.fillAmount = (float)(currentBonus.ExpiredDate - System.DateTime.Now).Days / (float)struc.MaxDays;
				for (int i = 0; i < struc.ColoredText.Length; i++)
				{
					struc.ColoredText[i].color = Color.white;
				}
				struc.Lock.SetActive( false);
				struc.ProgressImage.localScale = new Vector3(Mathf.Min(1, (float)currentBonus.current_stage / (float)currentBonus.total_stages), 1, 1);
				for (int i = 0; i < struc.Points.Length; i++)
				{
					struc.Points[i].gameObject.SetActive(i <= currentBonus.current_stage);
				}
			}

			//struc.DaysTimeLabel.text = (currentBonus.ExpiredDate - System.DateTime.Now).Days.ToString();

		}


	}
}