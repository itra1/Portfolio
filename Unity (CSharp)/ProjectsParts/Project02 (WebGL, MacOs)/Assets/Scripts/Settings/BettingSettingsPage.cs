using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System.Net.Sockets;

public class BettingSettingsPage : MonoBehaviour
{
	[SerializeField] private it.UI.Elements.GraphicButtonUI _appluButton;
	private List<DataPerc> _percParametrs = new List<DataPerc> {
	new DataPerc() { Value = 50 }
	, new DataPerc() { Value = 60 }
	, new DataPerc() { Value = 70 }
	, new DataPerc() { Value = 80 }
	, new DataPerc() { Value = 90 }
	, new DataPerc() { Value = 100 }
	};

	public class DataPerc
	{
		public string Name => Value + "%";
		public int Value;
	}

	[SerializeField] private TMP_Dropdown[] _percInputs;

	private UserProfilePost _pp;

	private void OnEnable()
	{
		com.ootii.Messages.MessageDispatcher.AddListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);
		_pp = UserController.User.user_profile.PostProfile;

		if (_pp.betting == null)
			_pp.betting = new Betting();


		FillInputs(true);
		FillInputs(false);
		AddListeners();
		if (_appluButton != null)
			_appluButton.interactable = false;
	}

	private void OnDisable()
	{
		com.ootii.Messages.MessageDispatcher.RemoveListener(EventsConstants.UserProfileUpdate, SettingsUpdateEvent);
#if !UNITY_STANDALONE
		ApplyButton();
#endif
	}
	private void SettingsUpdateEvent(com.ootii.Messages.IMessage handle)
	{
		_pp = UserController.User.user_profile.PostProfile;
	}

	private void AddListeners()
	{

		for (int i = 0; i < _percInputs.Length; i++)
		{
			_percInputs[i].onValueChanged.RemoveAllListeners();
			_percInputs[i].onValueChanged.AddListener((ind) =>
			{
				for (int ii = 0; ii < _percInputs.Length; ii++)
					_percInputs[ii].onValueChanged.RemoveAllListeners();

				if (_appluButton != null)
					_appluButton.interactable = true;
				FillInputs(false);
				AddListeners();
			});
		}
	}

	private void FillInputs(bool isStart = false)
	{

		var dt = _pp.betting;

		string beforeVal = "";

		string[] selectedData = new string[] { "-1", "-1", "-1" };

		for (int i = 0; i < _percInputs.Length; i++)
		{
			if (_percInputs[i].options.Count > 0)
				selectedData[i] = _percInputs[i].options[_percInputs[i].value].text;
		}

		for (int i = 0; i < _percInputs.Length; i++)
		{
			TMP_Dropdown input = _percInputs[i];

			input.ClearOptions();

			List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

			for (int ii = 0; ii < _percParametrs.Count; ii++)
			{
				//bool exists = false;
				//for (int iii = 0; iii < selectedData.Length; iii++)
				//	if (iii != ii && selectedData[iii] == _percParametrs[ii].Name)
				//		exists = true;

				if (!System.Array.Exists(selectedData, x => selectedData.Contains(_percParametrs[ii].Name))
				||
				(selectedData[i] == _percParametrs[ii].Name)
				)
					options.Add(new TMP_Dropdown.OptionData() { text = _percParametrs[ii].Name });
			}
			input.AddOptions(options);

			BettingButton bb = null;
			DataPerc p = null;

			if (isStart)
			{
				switch (i)
				{
					case 0:
						{
							bb = dt.button1;
							break;
						}
					case 1:
						{
							bb = dt.button2;
							break;
						}
					case 2:
						{
							bb = dt.button3;
							break;
						}
				}
				if (bb != null)
					p = _percParametrs.Find(y => y.Name == bb.normal_bet_size && !System.Array.Exists(selectedData, x => selectedData.Contains(y.Name)));

			}
			if (p == null)
			{
				p = _percParametrs.Find(y => y.Name == selectedData[i]);
			}
			if (p == null)
				p = _percParametrs.Find(y => !System.Array.Exists(selectedData, x => selectedData.Contains(y.Name)));

			selectedData[i] = p.Name;

			var ind = 0;

			for (int ii = 0; ii < options.Count; ii++)
				if (options[ii].text == p.Name)
					ind = ii;

			if (input.value == ind) continue;
			input.value = ind;

		}

	}

	public void ApplyButton()
	{
		_pp.betting.button1.normal_bet_size = _percInputs[0].options[_percInputs[0].value].text;
		_pp.betting.button2.normal_bet_size = _percInputs[1].options[_percInputs[1].value].text;
		_pp.betting.button3.normal_bet_size = _percInputs[2].options[_percInputs[2].value].text;
		UserController.Instance.UpdateProfile(_pp);

	}

	public void CancelButton()
	{

	}
}
